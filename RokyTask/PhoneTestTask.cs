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
        KEY_PCU_INIT = 1,
        KEY1_BIND = 2,
        KEY2_BIND = 3,
        KEY1_CHECK =4,
        KEY2_CHECK = 5,
    }

    public class PhoneTestTask : ITaskManager
    {
        #region 常量
        public int KeyNumber { get; set; }
        public bool bTaskRunning { get; set; }
        public string mSN { get; set; }
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
                        SetValidSN(sender, INFO_LEVEL.PASS);
                        SetMainText(sender, "清除已绑定钥匙...", "", INFO_LEVEL.PROCESS);
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
           
            mBindKeyTask = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mBindKeyParam = mBindKeyTask.GetRequestEntity();
            mBindKeyParam.ack_device = Const.CCU;
            mBindKeyParam.server_mode = 0x20;//绑定钥匙
            mBindKeyTask.RetryMaxCnts = 0;
            mBindKeyTask.Timerout = 20*1000;
            mBindKeyTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {                    
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
                                    if (level == Task_Level.TRUE)
                                    {
                                        mBindSteps = BindSteps.KEY_PCU_INIT;
                                        mBindKeyParam.ack_device = Const.PCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        bExcuted = true;
                                        SetMainText(sender, "异常操作！", "", INFO_LEVEL.FAIL);
                                    }
                                    else if(level == Task_Level.REPEAT)
                                    {
                                        mBindSteps = BindSteps.KEYS_CLEAR;
                                    }
                                    break;
                                case BindSteps.KEY_PCU_INIT:
                                    level = KEY_PCU_INIT(sender, mEventArgs.Data);
                                    if(level == Task_Level.TRUE)
                                    {
                                        SetMainText(sender, "请按要绑定的钥匙...", "", INFO_LEVEL.PROCESS);
                                        mBindSteps = BindSteps.KEY1_BIND;
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                    }
                                    else if(level == Task_Level.FALSE)
                                    {
                                        bExcuted = true;
                                        SetMainText(sender, "PCU遥控初始化失败!", "", INFO_LEVEL.PROCESS);
                                    }                                    
                                    break;
                                case BindSteps.KEY1_BIND:
                                    level = KEY1_BIND(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetMainText(sender, "请再按一次绑定钥匙，进行确认绑定...", "", INFO_LEVEL.PROCESS);
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x10;//检查钥匙
                                        mBindSteps = BindSteps.KEY1_CHECK;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {

                                    }
                                    else if (level == Task_Level.REPEAT)
                                    {
                                        SetMainText(sender, "此钥匙已绑定，请按另一个把钥匙...", "", INFO_LEVEL.PROCESS);
                                        mBindSteps = BindSteps.KEY1_BIND;
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                    }
                                    break;
                                case BindSteps.KEY1_CHECK:
                                    level = KEY1_CHECK(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetBindKey1(sender, INFO_LEVEL.PASS);
                                        SetMainText(sender, "此钥匙绑定成功！", "", INFO_LEVEL.PASS);
                                        bExcuted = true;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        SetMainText(sender, "此钥匙绑定失败！", "", INFO_LEVEL.FAIL);
                                        bExcuted = true;
                                    }                                    
                                    break;
                            }
                            break;
                        case 2:
                            switch(mBindSteps)
                            {
                                case BindSteps.KEYS_CLEAR:
                                    level = KEYS_CLEAR(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {                   
                                        mBindSteps = BindSteps.KEY_PCU_INIT;
                                        mBindKeyParam.ack_device = Const.PCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        bExcuted = true;
                                        SetMainText(sender, "异常操作！", "", INFO_LEVEL.FAIL);
                                    }
                                    else if (level == Task_Level.REPEAT)
                                    {
                                        mBindSteps = BindSteps.KEYS_CLEAR;
                                    }
                                    break;
                                case BindSteps.KEY_PCU_INIT:
                                    level = KEY_PCU_INIT(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetMainText(sender, "请按要绑定的钥匙...", "", INFO_LEVEL.PROCESS);
                                        mBindSteps = BindSteps.KEY1_BIND;
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {

                                    }
                                    break;
                                case BindSteps.KEY1_BIND:
                                    level = KEY1_BIND(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetMainText(sender, "请按第二把要绑定的钥匙...", "", INFO_LEVEL.PROCESS);
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                        mBindSteps = BindSteps.KEY2_BIND;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {

                                    }
                                    else if (level == Task_Level.REPEAT)
                                    {
                                        SetMainText(sender, "此钥匙重复绑定，请按另一把...", "", INFO_LEVEL.PROCESS);
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                        mBindSteps = BindSteps.KEY1_BIND;
                                    }
                                    break;
                                case BindSteps.KEY2_BIND:
                                    level = KEY2_BIND(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetMainText(sender, "按任意键，进行确认绑定 第一把 钥匙...", "", INFO_LEVEL.PROCESS);
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x10;//检查钥匙
                                        mBindSteps = BindSteps.KEY1_CHECK;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        
                                    }
                                    else if (level == Task_Level.REPEAT)
                                    {
                                        SetMainText(sender, "此钥匙重复绑定，请按另一把...", "", INFO_LEVEL.PROCESS);
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                        mBindSteps = BindSteps.KEY2_BIND;
                                    }
                                    break;
                                case BindSteps.KEY1_CHECK:
                                    level = KEY1_CHECK(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetBindKey1(sender, INFO_LEVEL.PASS);
                                        SetMainText(sender, "按任意键，进行确认绑定 第二把 钥匙...", "", INFO_LEVEL.PROCESS);
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x10;//检查钥匙
                                        mBindSteps = BindSteps.KEY2_CHECK;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        SetBindKey1(sender, INFO_LEVEL.FAIL);
                                        SetMainText(sender, "此钥匙绑定确认失败！", "", INFO_LEVEL.FAIL);
                                        bExcuted = true;
                                    }
                                    break;
                                case BindSteps.KEY2_CHECK:
                                    level = KEY2_CHECK(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetBindKey2(sender, INFO_LEVEL.PASS);
                                        SetMainText(sender, "此钥匙绑定成功！", "", INFO_LEVEL.PASS);
                                        bExcuted = true;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        SetBindKey2(sender, INFO_LEVEL.FAIL);
                                        SetMainText(sender, "此钥匙绑定确认失败！", "", INFO_LEVEL.FAIL);
                                        bExcuted = true;
                                    }                                    
                                    break;
                            }
                            break;
                        default:
                            bExcuted = true;
                            SetMainText(sender, "只能绑定一把或两把钥匙！", "", INFO_LEVEL.FAIL);
                            break;
                    }

                    //判断
                    if(!bExcuted)
                    {
                        Thread.Sleep(500);
                        mBindKeyTask.Excute();
                    }
                    else
                    {
                        StopTask();
                        return;
                    }
                }
                else
                {
                    SetMainText(sender, "未收到绑定钥匙指令或异常操作！", "", INFO_LEVEL.FAIL);
                    StopTask();
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

        #region PCU初始化
        private Task_Level KEY_PCU_INIT(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            if(mAskDevice == Const.PCU)
            {
                return Task_Level.TRUE;
            }
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
                    return Task_Level.TRUE;
                }
                else if(mAskResult == 0x24)
                {
                    return Task_Level.REPEAT;
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
                    return Task_Level.TRUE;
                }
                else if(mAskResult == 0x24)
                {
                    return Task_Level.REPEAT;
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
                    return Task_Level.TRUE;
                }
                else if (mAskResult == 0x23)
                {
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
                if (mAskResult == 0x21)//绑定成功
                {
                    return Task_Level.TRUE;
                }
                else if (mAskResult == 0x23)//绑定失败
                {
                    return Task_Level.FALSE;
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
            mBroadCastTask.ClearAllEvent();
            mStopSyncTask.ClearAllEvent();
            mMonitorTask.ClearAllEvent();

            mGetDevInfoTask.EnableTimeOutHandler = false;
            mBindKeyTask.EnableTimeOutHandler = false;
            mBroadCastTask.EnableTimeOutHandler = false;
            mStopSyncTask.EnableTimeOutHandler = false;
            mMonitorTask.EnableTimeOutHandler = false;
        }
        #endregion

        #region 初始化参数
        private void InitTask()
        {
            
            mGetDevInfoTask.ClearAllEvent();
            mBindKeyTask.ClearAllEvent();
            mBroadCastTask.ClearAllEvent();
            mStopSyncTask.ClearAllEvent();
            mMonitorTask.ClearAllEvent();
            mGetDevInfoTask.EnableTimeOutHandler = true;
            mBindKeyTask.EnableTimeOutHandler = true;
            mBroadCastTask.EnableTimeOutHandler = true;
            mStopSyncTask.EnableTimeOutHandler = true;
            mMonitorTask.EnableTimeOutHandler = true;
        }
        #endregion

        //执行任务
        public void ExcuteTask()
        {
            if(KeyNumber == 0)
            {
                SetMainText(this, "选择至少绑定一把钥匙！", "", INFO_LEVEL.FAIL);
                return;
            }
            else if(KeyNumber > 2)
            {
                SetMainText(this, "绑定钥匙不能多于2把！", "", INFO_LEVEL.FAIL);
                return;
            }
            

            InitTask();
            bTaskRunning = true;
            SetMainText(this, "等待设备上电中...", "", INFO_LEVEL.PROCESS);
            mBroadCastTask.Excute();
        }
    }
}
