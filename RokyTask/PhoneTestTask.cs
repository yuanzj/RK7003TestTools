using CommonUtils;
using Roky;
using RokyTask.Entity.Protocols.request;
using RokyTask.Entity.Protocols.response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RokyTask
{
    public enum BindSteps
    {
        KEYS_CLEAR = 0,
        KEY1_BIND = 1,
        KEY2_BIND = 2,
        KEY1_CHECK =3,
        KEY2_CHECK = 4,
    }

    public class PhoneTestTask : ITaskManager
    {
        #region 常量
        public int KeyNumber { get; set; }
        public bool bTaskRunning { get; set; }
        public string mSN { get; set; }
        private int nCurrentKey { get; set; }
        private BindSteps mBindSteps;
        #endregion

        #region 注册事件
        public event EventHandler UpdateWorkStatusHandler;
        public event EventHandler UpdateValidSNHandler;
        public event EventHandler BindKey1Handler;
        public event EventHandler BindKey2Handler;
        public event EventHandler ListViewHandler;
        #endregion

        #region 注册串口事件
        SimpleSerialPortTask<getDevinfoReq, getDevinfoRsp> mGetDevInfoTask;
        getDevinfoReq mDevInfoParam;
        //CCU 绑定
        SimpleSerialPortTask<get7001Result, get7001ResultRsp> mBindKeyTask;
        get7001Result mBindKeyParam;

        SimpleSerialPortTask<get7001Result, get7001ResultRsp> mClearKeysTask;
        get7001Result mClearKeysParam;
        //CCU check
        SimpleSerialPortTask<get7001Result, get7001ResultRsp> mCheckKeyTask;
        get7001Result mCheckKeyParam;
        //PCU Init
        SimpleSerialPortTask<get7001Result, NullEntity> mPcuInitTask;
        get7001Result mPcuInitParam;
        //同步报文
        SimpleSerialPortTask<NullEntity, pcTakeoverReq> mBroadCastTask;
        //停止同步报文
        SimpleSerialPortTask<pcTakeOverRsp, NullEntity> mStopSyncTask;
        pcTakeOverRsp mStopSyncParam;
        //监控报文
        SimpleSerialPortTask<NullEntity, pcTakeoverReq> mMonitorTask;
        #endregion

        #region 构造函数
        public PhoneTestTask()
        {
            TaskBuilder();
        }
        #endregion

        #region 构造任务
        private void TaskBuilder()
        {
            //等待同步报文
            //监听0x06
            mBroadCastTask = new SimpleSerialPortTask<NullEntity, pcTakeoverReq>();
            mBroadCastTask.RetryMaxCnts = 0;
            mBroadCastTask.Timerout = 30 * 1000;
            mBroadCastTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<pcTakeoverReq> mEventArgs = e as SerialPortEventArgs<pcTakeoverReq>;
                if (mEventArgs.Data != null)
                {
                    //PC接管报文的响应
                    mStopSyncTask.Excute();
                    //启动监控，防止没有发成功，直到由PC接管为止
                    mMonitorTask.Excute();
                }
                else
                {
                    SetMainText(sender, "设备未上电！", "", INFO_LEVEL.FAIL);
                    StopTask();
                }
            };
            //监控同步报文
            mMonitorTask = new SimpleSerialPortTask<NullEntity, pcTakeoverReq>();
            mMonitorTask.RetryMaxCnts = 0;
            mMonitorTask.Timerout = 1000;
            mMonitorTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<pcTakeoverReq> mEventArgs = e as SerialPortEventArgs<pcTakeoverReq>;
                if (mEventArgs.Data != null)
                {
                    mStopSyncTask.Excute();
                    //再继续监控
                    mMonitorTask.Excute();
                }
                else
                {
                    SetMainText(sender, "获取设备SN号中...", "", INFO_LEVEL.PROCESS);
                    mGetDevInfoTask.Excute();
                }
            };

            //停止同步报文
            mStopSyncTask = new SimpleSerialPortTask<pcTakeOverRsp, NullEntity>();
            mStopSyncParam = mStopSyncTask.GetRequestEntity();
            mStopSyncTask.RetryMaxCnts = 0;
            mStopSyncTask.Timerout = 1000;
            mStopSyncParam.DeviceType = 0xF1;
            mStopSyncParam.HardWareID = 0xF1;
            mStopSyncParam.FirmID = 0;
            
            //获取SN号
            mGetDevInfoTask = new SimpleSerialPortTask<getDevinfoReq, getDevinfoRsp>();
            mDevInfoParam = mGetDevInfoTask.GetRequestEntity();
            mGetDevInfoTask.RetryMaxCnts = 10;
            mGetDevInfoTask.Timerout = 1000;
            mDevInfoParam.devType = 0X08;
            mGetDevInfoTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<getDevinfoRsp> mEventArgs = e as SerialPortEventArgs<getDevinfoRsp>;
                if (mEventArgs.Data != null)
                {
                    string devSN = ByteProcess.byteArrayToString(mEventArgs.Data.devSN);
                    if (mSN != devSN)
                    {
                        SetMainText(sender, "标签和设备SN号不匹配！", "", INFO_LEVEL.FAIL);
                        SetValidSN(sender, INFO_LEVEL.FAIL);
                        StopTask();
                    }
                    else
                    {
                        SetMainText(sender, "清空已配对钥匙...", "", INFO_LEVEL.PROCESS);
                        mClearKeysTask.Excute();
                    }
                }
                else
                {
                    SetMainText(sender, "未收到设备信息！", "", INFO_LEVEL.FAIL);
                    StopTask();
                }
            };

            //PCU进入绑定模式
            mPcuInitTask = new SimpleSerialPortTask<get7001Result, NullEntity>();
            mPcuInitParam = mPcuInitTask.GetRequestEntity();
            mPcuInitTask.RetryMaxCnts = 0;
            mPcuInitTask.Timerout = 1000;
            mPcuInitParam.ack_device = Const.PCU;//PCU
            mPcuInitParam.server_mode = 0x08;
            mClearKeysTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {
                    byte mAskDevice = (byte)mEventArgs.Data.ack_device;//应答设备
                    byte mAskResult = (byte)mEventArgs.Data.ack_value;//响应
                    if (mAskDevice == Const.PCU)
                    {
                        string msg = String.Format("请按第{0}把钥匙的任意键进行绑定...", nCurrentKey);
                        SetMainText(sender, msg, "", INFO_LEVEL.PROCESS);
                        mBindKeyParam.ack_device = Const.CCU;
                        mBindKeyParam.server_mode = 0x20;//先清除钥匙
                        mBindSteps = BindSteps.KEYS_CLEAR;
                        mBindKeyTask.Excute();
                    }
                }
                else
                {
                    SetMainText(sender, "未收到设备信息！", "", INFO_LEVEL.FAIL);
                    StopTask();
                }
            };

            mClearKeysTask = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mClearKeysParam = mClearKeysTask.GetRequestEntity();
            mClearKeysParam.ack_device = Const.CCU;
            mClearKeysParam.server_mode = 0x20;//清空所有已配对钥匙
            mClearKeysTask.RetryMaxCnts = 0;
            mClearKeysTask.Timerout = 20 * 1000;
            mClearKeysTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {
                    byte mAskDevice = (byte)mEventArgs.Data.ack_device;//应答设备
                    byte mAskResult = (byte)mEventArgs.Data.ack_value;//响应
                    if(mAskDevice == Const.CCU && mAskResult == 0x25)
                    {                        
                        mPcuInitTask.Excute();                        
                    }
                }
                else
                {
                    SetMainText(sender, "清空钥匙失败", "", INFO_LEVEL.FAIL);
                    StopTask();
                }
            };

            mBindKeyTask = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mBindKeyParam = mBindKeyTask.GetRequestEntity();
            mBindKeyParam.ack_device = Const.CCU;
            mBindKeyParam.server_mode = 0x08;//绑定钥匙
            mBindKeyTask.RetryMaxCnts = 0;
            mBindKeyTask.Timerout = 20*1000;
            mBindKeyTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {
                    /*
                    byte mAskDevice = (byte)mEventArgs.Data.ack_device;//应答设备
                    byte mAskResult = (byte)mEventArgs.Data.ack_value;//响应
                    if (mAskDevice == Const.CCU)
                    {
                        if (mAskResult == 0x21)//绑定成功
                        {
                            if (nCurrentKey < KeyNumber)
                            {
                                nCurrentKey++;//绑定下一把
                                string msg = String.Format("请按第{0}把钥匙的任意键进行绑定...", nCurrentKey);
                                SetMainText(sender, msg, "", INFO_LEVEL.PROCESS);
                                mBindKeyTask.Excute();
                            }
                            else
                            {
                                //开始Check是否绑定成功
                                nCurrentKey = 1;
                                string msg = String.Format("请确认第{0}把钥匙是否绑定成功...", nCurrentKey);
                                SetMainText(sender, msg, "", INFO_LEVEL.PROCESS);
                                mCheckKeyTask.Excute();
                            }
                        }
                        else if (mAskResult == 0x24)//绑定重复
                        {
                            SetMainText(sender, "此钥匙已绑定过，重新绑定...", "", INFO_LEVEL.PROCESS);
                            mBindKeyTask.Excute();
                        }
                    }
                    */
                    bool bExcuted = false;
                    Task_Level level = Task_Level.FALSE;
                    //根据绑定钥匙数量来
                    switch (KeyNumber)
                    {
                        case 1:
                            switch(mBindSteps)
                            {
                                case BindSteps.KEYS_CLEAR:
                                    level = KEYS_CLEAR(sender, mEventArgs.Data);
                                    break;
                                case BindSteps.KEY1_BIND:
                                    level = KEY1_BIND(sender, mEventArgs.Data);
                                    break;
                                case BindSteps.KEY1_CHECK:
                                    level = KEY1_CHECK(sender, mEventArgs.Data);
                                    break;
                            }
                            break;
                        case 2:
                            switch(mBindSteps)
                            {
                                case BindSteps.KEYS_CLEAR:
                                    level = KEYS_CLEAR(sender, mEventArgs.Data);
                                    break;
                                case BindSteps.KEY1_BIND:
                                    level = KEY1_BIND(sender, mEventArgs.Data);
                                    break;
                                case BindSteps.KEY2_BIND:
                                    level = KEY2_BIND(sender, mEventArgs.Data);
                                    break;
                                case BindSteps.KEY1_CHECK:
                                    level = KEY1_CHECK(sender, mEventArgs.Data);
                                    break;
                                case BindSteps.KEY2_CHECK:
                                    level = KEY2_CHECK(sender, mEventArgs.Data);
                                    break;
                            }
                            break;
                        default:
                            bExcuted = true;
                            break;
                    }

                    //判断
                    if(bExcuted)
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    SetMainText(sender, "绑定钥匙失败！", "", INFO_LEVEL.FAIL);
                    StopTask();
                }
            };

            mCheckKeyTask = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mCheckKeyParam = mCheckKeyTask.GetRequestEntity();
            mCheckKeyParam.ack_device = Const.CCU;
            mCheckKeyParam.server_mode = 0x10;//检查钥匙
            mCheckKeyTask.RetryMaxCnts = 0;
            mCheckKeyTask.Timerout = 20*1000;
            mCheckKeyTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {
                    if (mEventArgs.Data != null)
                    {
                        byte mAskDevice = (byte)mEventArgs.Data.ack_device;//应答设备
                        byte mAskResult = (byte)mEventArgs.Data.ack_value;//响应
                        if (mAskDevice == Const.CCU)
                        {                           
                            if(mAskResult == 0x21)
                            {
                                if (nCurrentKey < KeyNumber)
                                {
                                    string msg = String.Format("请确认第{0}把钥匙是否绑定成功...", nCurrentKey);
                                    SetMainText(sender, msg, "", INFO_LEVEL.PROCESS);
                                    SetBindKey1(sender, INFO_LEVEL.PASS);
                                    mCheckKeyTask.Excute();
                                }
                                else
                                {
                                    SetBindKey2(sender, INFO_LEVEL.PASS);
                                    string msg = String.Format("此设备成功绑定{0}把钥匙", KeyNumber);
                                    SetMainText(sender, msg, "", INFO_LEVEL.PASS);
                                }
                            }
                            else if(mAskResult == 0x23)
                            {
                                SetBindKey1(sender, INFO_LEVEL.FAIL);
                                SetBindKey2(sender, INFO_LEVEL.FAIL);
                                SetMainText(sender, "绑定失败！", "", INFO_LEVEL.FAIL);
                                StopTask();
                                return;
                            }
                        }
                    }
                    else
                    {
                        SetMainText(sender, "绑定钥匙失败！", "", INFO_LEVEL.FAIL);
                        StopTask();
                    }
                }
            };
        }
        #endregion

        #region 清理钥匙    
        private Task_Level KEYS_CLEAR(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            if (mAskDevice == Const.CCU && mAskResult == 0x25)
            {
                return Task_Level.TRUE;
            }
            else if(mAskDevice == Const.PCU || mAskDevice == Const.TESTSERVER)
                return Task_Level.REPEAT;

            return Task_Level.FALSE;
        }
        #endregion

        #region 绑定钥匙1
        private Task_Level KEY1_BIND(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            if(mAskDevice == Const.CCU)
            {
                if (mAskResult == 0x21)//绑定成功)
                {
                    
                }
                else if(mAskResult == 0x24)
                {

                }
            }
            
            return Task_Level.FALSE;
        }
        #endregion

        #region 绑定钥匙2
        private Task_Level KEY2_BIND(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            if(mAskDevice == Const.CCU)
            {
                if(mAskResult == 0x21)
                {

                }
                else if(mAskResult == 0x24)
                {

                }
            }

            return Task_Level.FALSE;
        }
        #endregion

        #region Key1 check确认
        private Task_Level KEY1_CHECK(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            if (mAskDevice == Const.CCU)
            {
                if (mAskResult == 0x21)//确认成功
                {
                    SetMainText(sender, "请按第二把钥匙，进行确认...", "", INFO_LEVEL.PROCESS);
                    return Task_Level.TRUE;
                }
                else if (mAskResult == 0x23)
                {
                    SetMainText(sender, "第一把钥匙确认失败！", "", INFO_LEVEL.FAIL);
                    return Task_Level.FALSE;
                }
            }

            return Task_Level.FALSE;
        }
        #endregion

        #region Key2 check确认
        private Task_Level KEY2_CHECK(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            if (mAskDevice == Const.CCU)
            {
                if (mAskResult == 0x21)//绑定成功)
                {
                    
                }
                else if (mAskResult == 0x23)
                {

                }
            }

            return Task_Level.FALSE;
        }
        #endregion

        #region 更新主页显示状态
        private void SetMainText(object sender, string _msg, string _submsg, INFO_LEVEL _level)
        {
            if (_level == INFO_LEVEL.FAIL || _level == INFO_LEVEL.PASS)
                bTaskRunning = false;

            if(UpdateWorkStatusHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.msg = _msg;
                mArgs.submsg = _submsg;
                mArgs.level = _level;
                UpdateWorkStatusHandler(sender, mArgs);
            }
        }
        #endregion

        #region 设置SN号
        private void SetValidSN(object sender, INFO_LEVEL _level)
        {
            if(UpdateValidSNHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = _level;
                UpdateValidSNHandler(sender, mArgs);
            }
        }
        #endregion

        #region 绑定钥匙1
        private void SetBindKey1(object sender, INFO_LEVEL _level)
        {
            if(BindKey1Handler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = _level;
                BindKey1Handler(sender, mArgs);
            }
        }
        #endregion

        #region 绑定钥匙2
        private void SetBindKey2(object sender, INFO_LEVEL _level)
        {
            if(BindKey2Handler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = _level;
                BindKey2Handler(sender, mArgs);
            }
        }
        #endregion

        #region 设置ListView
        private void SetListView(object sender, string msg, string submsg)
        {
            if(ListViewHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.msg = msg;
                mArgs.submsg = submsg;
                ListViewHandler(sender, mArgs);
            }
        }
        #endregion

        #region 停止Task
        private void StopTask()
        {
            mGetDevInfoTask.ClearAllEvent();
            mBindKeyTask.ClearAllEvent();
            mCheckKeyTask.ClearAllEvent();
            mPcuInitTask.ClearAllEvent();
            mBroadCastTask.ClearAllEvent();
            mStopSyncTask.ClearAllEvent();
            mMonitorTask.ClearAllEvent();

            mGetDevInfoTask.EnableTimeOutHandler = false;
            mBindKeyTask.EnableTimeOutHandler = false;
            mCheckKeyTask.EnableTimeOutHandler = false;
            mPcuInitTask.EnableTimeOutHandler = false;
            mBroadCastTask.EnableTimeOutHandler = false;
            mStopSyncTask.EnableTimeOutHandler = false;
            mMonitorTask.EnableTimeOutHandler = false;
        }
        #endregion

        #region 初始化参数
        private void InitTask()
        {
            nCurrentKey = 1;
            mGetDevInfoTask.ClearAllEvent();
            mBindKeyTask.ClearAllEvent();
            mCheckKeyTask.ClearAllEvent();
            mPcuInitTask.ClearAllEvent();
            mBroadCastTask.ClearAllEvent();
            mStopSyncTask.ClearAllEvent();
            mMonitorTask.ClearAllEvent();

            mGetDevInfoTask.EnableTimeOutHandler = true;
            mBindKeyTask.EnableTimeOutHandler = true;
            mCheckKeyTask.EnableTimeOutHandler = true;
            mPcuInitTask.EnableTimeOutHandler = true;
            mBroadCastTask.EnableTimeOutHandler = true;
            mStopSyncTask.EnableTimeOutHandler = true;
            mMonitorTask.EnableTimeOutHandler = true;
        }
        #endregion

        //执行任务
        public void ExcuteTask()
        {
            InitTask();
            bTaskRunning = true;
            SetMainText(this, "等待设备上电中...", "", INFO_LEVEL.PROCESS);
            mBroadCastTask.Excute();
        }
    }
}
