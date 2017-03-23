using CommonUtils;
using Roky;
using RokyTask.Entity.Protocols.request;
using RokyTask.Entity.Protocols.response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RokyTask
{
    public enum BindSteps
    {
        WAIT_POWER = 0,
        SN_VALID = 1,
        KEY1_BIND = 2,
        KEY2_BIND = 3,
        WRITE_NV = 4
    }

    public enum KeyType
    {
        NONE_KEY = 0,
        BIND_KEY1 = 1,
        BIND_KEY2 = 2,
        WRITE_NV = 3,
    }

    public enum STEP_LEVEL
    {
        NONE = 0,
        WAIT_POWER = 1,
        CHECK_SN = 2,
        BIND_KEY1 = 3,
        BIND_KEY2 = 4,
        WRITE_NV = 5,
        PASS = 6,
        FAIL = 7,
        BIND_TIMEOUT = 8,
    }

    public class StepArgs : EventArgs
    {
        public STEP_LEVEL level { get; set; }
        public string msg { get; set; }
        public string submsg { get; set; }
    }

    public class KeyValueArgs : EventArgs
    {
        public KeyType type { get; set; }
        public int value { get; set; }
    }

    public class KeyManager
    {
        public int Key1Value { get; set; }
        public int Key2Value { get; set; }
        public bool Key1Flag { get; set; }
        public bool Key2Flag { get; set; }
    }

    public class PhoneTestTask : ITaskManager
    {
        #region 常量
        public int KeyNumber { get; set; }
        public int TryCnts { get; set; }
        public int keysNumber { get; set; }
        public bool bTaskRunning { get; set; }
        public string mSN { get; set; }
        private int mKey1Value { get; set; }
        private int mKey2Value { get; set; }
        private int mTimeout { get; set; }
        Hashtable ht;//存储钥匙的键和值

        #endregion

        #region 注册事件
        public event EventHandler UpdateWorkStatusHandler;
        public event EventHandler UpdateValidSNHandler;
        public event EventHandler BindKey1Handler;
        public event EventHandler BindKey2Handler;
        public event EventHandler WriteNVHandler;
        public event EventHandler ListViewHandler;
        public event EventHandler KeyValueHandler;
        #endregion

        #region 注册串口事件
        SimpleSerialPortTask<getDevinfoReq, getDevinfoRsp> mGetDevInfoTask;
        getDevinfoReq mDevInfoParam;
        //CCU 绑定
        SimpleSerialPortTask<get7001Result, get7001ResultRsp> mBindKey1Task;
        get7001Result mBindKey1Param;
        SimpleSerialPortTask<get7001Result, get7001ResultRsp> mBindKey2Task;
        get7001Result mBindKey2Param;

        SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp> mWriteKeyTask;
        writeKeyAddrReq mWriteKeyParam;
        //同步报文
        SimpleSerialPortTask<NullEntity, pcTakeoverReq> mBroadCastTask;
        //停止同步报文
        SimpleSerialPortTask<pcTakeOverRsp, NullEntity> mStopSyncTask;
        pcTakeOverRsp mStopSyncParam;
        //监控报文
        SimpleSerialPortTask<NullEntity, pcTakeoverReq> mMonitorTask;
        //恢复同步报文
        SimpleSerialPortTask<ResetEcuReq, NullEntity> mRecoverTask;
        ResetEcuReq mRecoverParam;
        
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
            //回复同步
            mRecoverTask = new SimpleSerialPortTask<ResetEcuReq, NullEntity>();
            mRecoverParam = mRecoverTask.GetRequestEntity();
            mRecoverTask.RetryMaxCnts = 1;
            mRecoverTask.Timerout = 1000;
            mRecoverParam.deviceType = 0XF1;
            mRecoverParam.cmdCode = 0x02;
            mRecoverParam.param1 = 0;
            mRecoverParam.param2 = 0;
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
                    SetListView(sender, "上电超时！", "设备未上电或通讯有异常");
                    SetItemFail(sender, BindSteps.WAIT_POWER);
                    SetMainText(sender,  STEP_LEVEL.FAIL);
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
                    SetMainText(sender,  STEP_LEVEL.CHECK_SN);
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
                        string temp = String.Format("设备读取的SN号：{0}", devSN);
                        SetListView(sender, "标签和设备SN号不匹配", temp);
                        SetMainText(sender, STEP_LEVEL.FAIL);
                        SetItemFail(sender, BindSteps.SN_VALID);
                        mRecoverTask.Excute();
                        StopTask();
                    }
                    else
                    {
                        SetValidSN(sender, INFO_LEVEL.PASS);
                        SetMainText(sender, STEP_LEVEL.BIND_KEY1);
                        mBindKey1Param.ack_device = Const.PCU;
                        mBindKey1Task.Excute();
                        mTimeout = 100;
                    }
                }
                else
                {
                    SetListView(sender, "未收到读取设备信息", "设备未上电或通讯有异常");
                    SetMainText(sender, STEP_LEVEL.FAIL);
                    SetValidSN(sender, INFO_LEVEL.FAIL);
                    mRecoverTask.Excute();
                    StopTask();
                }
            };
           
            mBindKey1Task = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mBindKey1Param = mBindKey1Task.GetRequestEntity();
            mBindKey1Param.ack_device = Const.PCU;
            mBindKey1Param.server_mode = 0x08;//绑定钥匙
            mBindKey1Param.ecu_status = 0x34;
            mBindKey1Param.level_ctrl = 0x0000;
            mBindKey1Task.RetryMaxCnts = 0;
            mBindKey1Task.Timerout = 10*1000;
            mBindKey1Task.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {         
                    byte mAskDevice = (byte)mEventArgs.Data.ack_device;//应答设备
                    byte mAskResult = (byte)mEventArgs.Data.ack_value;//响应
                    int temp = (byte)mEventArgs.Data.CutError_1 |
                                          mEventArgs.Data.CutError_2 << 8  |
                                          mEventArgs.Data.ShortError_1 << 16;
                    int iAddrCode = temp >> 4;
                    if (mAskDevice == Const.PCU)
                    {
                        // 如果不为0， 说明有按下，把地址码键值存入List
                        if (iAddrCode != 0)
                        {
                            // 先判断是否含有某个Key
                            if (ht.ContainsKey(iAddrCode))
                            {
                                int iCnt = (int)ht[iAddrCode];
                                ht[iAddrCode] = iCnt + 1;
                            }
                            else//如果不存在此KEY
                            {
                                ht.Add(iAddrCode, 0);
                            }
                        }
                    }
                    //判断哈希的最大值
                    int valueMax = 0;
                    foreach (int value in ht.Values)
                    {
                        if (value > valueMax)
                            valueMax = value;
                    }
                    //判断次数是否超过某一值
                    if(valueMax >= this.TryCnts)
                    {
                        foreach(int key in ht.Keys)
                        {
                            if((int)ht[key] == valueMax)
                            {
                                SetBindKey1(sender, INFO_LEVEL.PASS);
                                //保存Key1按键
                                mKey1Value = key;
                                mTimeout = 100;
                                byte[] temp1 = new byte[3];
                                temp1[0] = (byte)(mKey1Value >> 16 & 0xFF);
                                temp1[1] = (byte)(mKey1Value >> 8 & 0xFF);
                                temp1[2] = (byte)(mKey1Value& 0xFF);
                                mWriteKeyParam.Key1Value = temp1;
                                SetKeyValue(sender, KeyType.BIND_KEY1, mKey1Value);
                                ht.Clear();
                                //写NV
                                if (KeyNumber == 2)
                                {
                                    SetMainText(sender, STEP_LEVEL.BIND_KEY2);
                                    mBindKey2Task.Excute();
                                }
                                else
                                {
                                    //写NV
                                    byte[] temp2 = new byte[3];
                                    mWriteKeyParam.Key2Value = temp2;
                                    mWriteKeyTask.Excute();
                                }
                                return;
                            }
                        }
                    }   
                    if(mTimeout-- > 0)
                    {
                        Thread.Sleep(50);
                        mBindKey1Task.Excute();
                    }
                    else
                    {
                        SetListView(sender, "绑定超时", "未在5秒内，有效按下按键");
                        SetItemFail(sender, BindSteps.KEY1_BIND);
                        SetMainText(sender, STEP_LEVEL.BIND_TIMEOUT);
                        mRecoverTask.Excute();
                        StopTask();
                    }                                           
                }
                else
                {
                    SetListView(sender, "未收到钥匙的绑定和检查的响应", "设备未上电或通讯有异常");
                    SetItemFail(sender, BindSteps.KEY1_BIND);
                    SetMainText(sender, STEP_LEVEL.FAIL);
                    mRecoverTask.Excute();
                    StopTask();
                }
            };

            mBindKey2Task = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mBindKey2Param = mBindKey2Task.GetRequestEntity();
            mBindKey2Param.ack_device = Const.PCU;
            mBindKey2Param.server_mode = 0x08;//绑定钥匙
            mBindKey2Param.ecu_status = 0x34;
            mBindKey2Param.level_ctrl = 0x0000;
            mBindKey2Task.RetryMaxCnts = 0;
            mBindKey2Task.Timerout = 10 * 1000;
            mBindKey2Task.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {
                    byte mAskDevice = (byte)mEventArgs.Data.ack_device;//应答设备
                    byte mAskRessult = (byte)mEventArgs.Data.ack_value;//响应
                    int temp = (byte)mEventArgs.Data.CutError_1 |
                                          mEventArgs.Data.CutError_2 << 8 |
                                          mEventArgs.Data.ShortError_1 << 16;
                    int iAddrCode = temp >> 4;                    
                    if (mAskDevice == Const.PCU)
                    {                        
                            // 如果不为0， 说明有按下，把地址码键值存入List
                        if (iAddrCode != 0 && iAddrCode != mKey1Value)
                        {
                            // 先判断是否含有某个Key
                            if (ht.ContainsKey(iAddrCode))
                            {
                                int iCnt = (int)ht[iAddrCode];
                                ht[iAddrCode] = iCnt + 1;
                            }
                            else//如果不存在此KEY
                            {
                                ht.Add(iAddrCode, 0);
                            }
                        }
                    }
                    //判断哈希的最大值
                    int valueMax = 0;
                    foreach (int value in ht.Values)
                    {
                        if (value >= valueMax)
                            valueMax = value;
                    }
                    //判断次数是否超过某一值
                    if (valueMax > this.TryCnts)
                    {
                        foreach (int key in ht.Keys)
                        {
                            if ((int)ht[key] == valueMax)
                            {
                                SetBindKey2(sender, INFO_LEVEL.PASS);
                                //保存Key1按键
                                mKey2Value = key;
                                byte[] temp1 = new byte[3];
                                temp1[0] = (byte)(mKey2Value >> 16 & 0xFF);
                                temp1[1] = (byte)(mKey2Value >> 8 & 0xFF);
                                temp1[2] = (byte)(mKey2Value & 0xFF);
                                mWriteKeyParam.Key2Value = temp1;
                                SetKeyValue(sender, KeyType.BIND_KEY2, mKey2Value);
                                mWriteKeyTask.Excute();
                                ht.Clear();
                                return;
                            }
                        }
                    }

                    if (mTimeout-- > 0)
                    {
                        Thread.Sleep(50);
                        mBindKey2Task.Excute();
                    }
                    else
                    {
                        SetListView(sender, "绑定超时", "未在5秒内，有效按下按键");
                        SetItemFail(sender, BindSteps.KEY2_BIND);
                        SetMainText(sender, STEP_LEVEL.BIND_TIMEOUT);
                        mRecoverTask.Excute();
                        StopTask();
                    }
                }
                else
                {
                    SetListView(sender, "未收到钥匙的绑定和检查的响应", "设备未上电或通讯有异常");
                    SetItemFail(sender, BindSteps.KEY2_BIND);
                    SetMainText(sender, STEP_LEVEL.FAIL);
                    mRecoverTask.Excute();
                    StopTask();
                }
            };

            mWriteKeyTask = new SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp>();
            mWriteKeyParam = mWriteKeyTask.GetRequestEntity();
            mWriteKeyParam.Key1Index = 0;
            mWriteKeyParam.Key2Index = 1;
            mWriteKeyParam.KeyNumber = 2;
            mWriteKeyParam.DeviceType = Const.CCU;
            mWriteKeyTask.RetryMaxCnts = 0;
            mWriteKeyTask.Timerout = 10 * 1000;
            mWriteKeyTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<writeKeyAddrRsp> mEventArgs = e as SerialPortEventArgs<writeKeyAddrRsp>;
                if (mEventArgs.Data != null)
                {
                    byte mResult = (byte)mEventArgs.Data.Result;
                    if(mResult == 0)
                    {
                        ht.Clear();
                        SetWriteNV(sender, INFO_LEVEL.PASS);                   
                        SetMainText(sender, STEP_LEVEL.PASS);
                        mRecoverTask.Excute();
                        StopTask();
                    }
                }
                else
                {
                    SetListView(sender, "写钥匙地址到设备失败", "设备异常或通讯有异常");
                    SetItemFail(sender, BindSteps.WRITE_NV);
                    SetMainText(sender, STEP_LEVEL.FAIL);
                    mRecoverTask.Excute();
                    StopTask();
                }
            };

        }
        #endregion
      
        #region 设置失败
        private void SetItemFail(object sender, BindSteps step)
        {
            switch(step)
            {
                case BindSteps.WAIT_POWER:
                    SetValidSN(sender, INFO_LEVEL.INIT);
                    SetBindKey1(sender, INFO_LEVEL.INIT);
                    SetBindKey2(sender, INFO_LEVEL.INIT);
                    SetWriteNV(sender, INFO_LEVEL.INIT);
                    break;
                case BindSteps.SN_VALID:
                    SetValidSN(sender, INFO_LEVEL.FAIL);
                    SetBindKey1(sender, INFO_LEVEL.INIT);
                    SetBindKey2(sender, INFO_LEVEL.INIT);
                    SetWriteNV(sender, INFO_LEVEL.INIT);
                    break;
                case BindSteps.KEY1_BIND:
                    SetBindKey1(sender, INFO_LEVEL.FAIL);
                    SetBindKey2(sender, INFO_LEVEL.INIT);
                    SetWriteNV(sender, INFO_LEVEL.INIT);
                    break;
                case BindSteps.KEY2_BIND:
                    SetBindKey2(sender, INFO_LEVEL.FAIL);
                    SetWriteNV(sender, INFO_LEVEL.INIT);
                    break;                 
            }
        }
        #endregion

        #region 更新主页显示状态
        private void SetMainText(object sender, STEP_LEVEL _level)
        {
            if (_level == STEP_LEVEL.FAIL || _level == STEP_LEVEL.PASS || _level == STEP_LEVEL.BIND_TIMEOUT)
                bTaskRunning = false;

            if(UpdateWorkStatusHandler != null)
            {
                StepArgs mArgs = new StepArgs();
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

        #region 写NV标志
        private void SetWriteNV(object sender, INFO_LEVEL _level)
        {
            if(WriteNVHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = _level;
                WriteNVHandler(sender, mArgs);
            }
        }
        #endregion

        #region 设置钥匙值的显示
        private void SetKeyValue(object sender, KeyType _type, int _value)
        {
            if(KeyValueHandler != null)
            {
                KeyValueArgs mArgs = new KeyValueArgs();
                mArgs.type = _type;
                mArgs.value = _value;
                KeyValueHandler(sender, mArgs);
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
            mBindKey1Task.ClearAllEvent();
            mBindKey2Task.ClearAllEvent();
            mBroadCastTask.ClearAllEvent();
            mStopSyncTask.ClearAllEvent();
            mMonitorTask.ClearAllEvent();
            mRecoverTask.ClearAllEvent();

            mGetDevInfoTask.EnableTimeOutHandler = false;
            mBindKey1Task.EnableTimeOutHandler = false;
            mBindKey2Task.EnableTimeOutHandler = false;
            mBroadCastTask.EnableTimeOutHandler = false;
            mStopSyncTask.EnableTimeOutHandler = false;
            mMonitorTask.EnableTimeOutHandler = false;
            mRecoverTask.EnableTimeOutHandler = false;
        }
        #endregion

        #region 初始化参数
        private void InitTask()
        {
            ht = new Hashtable();
            mKey1Value = 0;
            mKey2Value = 0;
            mTimeout = 100;
            mGetDevInfoTask.ClearAllEvent();
            mBindKey1Task.ClearAllEvent();
            mBroadCastTask.ClearAllEvent();
            mStopSyncTask.ClearAllEvent();
            mMonitorTask.ClearAllEvent();
            mRecoverTask.ClearAllEvent();
            mGetDevInfoTask.EnableTimeOutHandler = true;
            mBindKey1Task.EnableTimeOutHandler = true;
            mBroadCastTask.EnableTimeOutHandler = true;
            mStopSyncTask.EnableTimeOutHandler = true;
            mMonitorTask.EnableTimeOutHandler = true;
            mRecoverTask.EnableTimeOutHandler = true;
        }
        #endregion

        //执行任务
        public void ExcuteTask()
        { 
            InitTask();
            bTaskRunning = true;
            mBroadCastTask.Excute();
            SetMainText(this, STEP_LEVEL.WAIT_POWER);
        }
    }
}
