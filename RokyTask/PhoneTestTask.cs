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

    public enum KeyType
    {
        NONE_KEY = 0,
        BIND_KEY1 = 1,
        CHECK_KEY1 = 2,
        BIND_KEY2 = 3,
        CHECK_KEY2 = 4
    }

    public enum STEP_LEVEL
    {
        NONE = 0,
        WAIT_POWER = 1,
        CHECK_SN = 2,
        BIND_KEY = 3,
        CHECK_KEY = 4,
        PASS = 5,
        FAIL = 6,
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
        public string value { get; set; }
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
        public bool bTaskRunning { get; set; }
        public string mSN { get; set; }
        private BindSteps mBindSteps;
        private int nKey1Value { get; set; }
        private int nKey2Value { get; set; }
        KeyManager mKeyManager;
        #endregion

        #region 注册事件
        public event EventHandler UpdateWorkStatusHandler;
        public event EventHandler UpdateValidSNHandler;
        public event EventHandler BindKey1Handler;
        public event EventHandler BindKey2Handler;
        public event EventHandler CheckKey1Handler;
        public event EventHandler CheckKey2Handler;
        public event EventHandler ClearKeyHandler;
        public event EventHandler ListViewHandler;
        public event EventHandler KeyValueHandler;
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
            mKeyManager = new KeyManager();
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
                    SetMainText(sender, STEP_LEVEL.FAIL);
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
                    SetMainText(sender, STEP_LEVEL.CHECK_SN);
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
                        SetMainText(sender, STEP_LEVEL.FAIL);
                        SetValidSN(sender, INFO_LEVEL.FAIL);
                        StopTask();
                    }
                    else
                    {
                        SetValidSN(sender, INFO_LEVEL.PASS);                        
                        mBindKeyParam.ack_device = Const.CCU;
                        mBindKeyParam.server_mode = 0x20;//先清除钥匙
                        mBindSteps = BindSteps.KEYS_CLEAR;
                        mBindKeyTask.Excute();
                    }
                }
                else
                {
                    SetMainText(sender,  STEP_LEVEL.FAIL);
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
                                        SetClearKey(sender, INFO_LEVEL.PASS);
                                        mBindSteps = BindSteps.KEY_PCU_INIT;
                                        mBindKeyParam.ack_device = Const.PCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        bExcuted = true;
                                        SetMainText(sender,  STEP_LEVEL.FAIL);
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
                                        SetMainText(sender, STEP_LEVEL.BIND_KEY); //开始绑定钥匙                                                                           
                                        mBindSteps = BindSteps.KEY1_BIND;
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙1
                                    }
                                    else if(level == Task_Level.FALSE)
                                    {
                                        bExcuted = true;                                        
                                    }                                    
                                    break;
                                case BindSteps.KEY1_BIND:
                                    level = KEY1_BIND(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {                
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x10;//检查
                                        mBindSteps = BindSteps.KEY1_CHECK;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {

                                    }
                                    else if (level == Task_Level.REPEAT)
                                    {              
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
                                        bExcuted = true;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {                                     
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
                                        SetClearKey(sender, INFO_LEVEL.PASS);
                                        mBindSteps = BindSteps.KEY_PCU_INIT;
                                        mBindKeyParam.ack_device = Const.PCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        bExcuted = true;                                       
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
                                        SetMainText(sender, STEP_LEVEL.BIND_KEY); //开始绑定钥匙
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
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                        mBindSteps = BindSteps.KEY2_BIND;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {

                                    }
                                    else if (level == Task_Level.REPEAT)
                                    {                  
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x08;//绑定钥匙
                                        mBindSteps = BindSteps.KEY1_BIND;
                                    }
                                    break;
                                case BindSteps.KEY2_BIND:
                                    level = KEY2_BIND(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetMainText(sender, STEP_LEVEL.CHECK_KEY);    
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x10;//检查钥匙
                                        mBindSteps = BindSteps.KEY1_CHECK;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        
                                    }
                                    else if (level == Task_Level.REPEAT)
                                    {                
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
                                        mBindKeyParam.ack_device = Const.CCU;
                                        mBindKeyParam.server_mode = 0x10;//检查钥匙
                                        mBindSteps = BindSteps.KEY2_CHECK;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        SetBindKey1(sender, INFO_LEVEL.FAIL);
                                        bExcuted = true;
                                    }
                                    break;
                                case BindSteps.KEY2_CHECK:
                                    level = KEY2_CHECK(sender, mEventArgs.Data);
                                    if (level == Task_Level.TRUE)
                                    {
                                        SetBindKey2(sender, INFO_LEVEL.PASS);
                                        SetMainText(sender, STEP_LEVEL.PASS);
                                        bExcuted = true;
                                    }
                                    else if (level == Task_Level.FALSE)
                                    {
                                        SetBindKey2(sender, INFO_LEVEL.FAIL);
                                        SetMainText(sender, STEP_LEVEL.FAIL);
                                        bExcuted = true;
                                    }                                    
                                    break;
                            }
                            break;
                        default:
                            bExcuted = true;
                            SetMainText(sender, STEP_LEVEL.FAIL);
                            break;
                    }
                    //判断
                    if(!bExcuted)
                    {
                        Thread.Sleep(100);
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
                    SetMainText(sender, STEP_LEVEL.FAIL);
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
            byte mAddr_1 = (byte)mArgs.CutError_1;
            byte mAddr_2 = (byte)mArgs.CutError_2;
            byte mAddr_3 = (byte)mArgs.ShortError_1;
            byte mAddr_4 = (byte)mArgs.ShortError_2;

            mKeyManager.Key1Value = mAddr_1 << 16 | mAddr_2 << 8 | mAddr_3;

            if (mAskDevice == Const.CCU)
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
            byte mAddr_1 = (byte)mArgs.CutError_1;
            byte mAddr_2 = (byte)mArgs.CutError_2;
            byte mAddr_3 = (byte)mArgs.ShortError_1;
            byte mAddr_4 = (byte)mArgs.ShortError_2;

            mKeyManager.Key2Value = mAddr_1 << 16 | mAddr_2 << 8 | mAddr_3;

            if (mAskDevice == Const.CCU)
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

        private void SetKeyValue(object sender, KeyType type, string value)
        {
            if(KeyValueHandler != null)
            {
                KeyValueArgs mArgs = new KeyValueArgs();
                mArgs.type = type;
                mArgs.value = value;
                KeyValueHandler(sender, mArgs);
            }
        }

        #region Key1 check确认
        private Task_Level KEY1_CHECK(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            byte mAddr_1 = (byte)mArgs.CutError_1;
            byte mAddr_2 = (byte)mArgs.CutError_2;
            byte mAddr_3 = (byte)mArgs.ShortError_1;
            byte mAddr_4 = (byte)mArgs.ShortError_2;

            int keyValue = mAddr_1 << 16 | mAddr_2 << 8 | mAddr_3;

            if (mAskDevice == Const.CCU)
            {
                if (mKeyManager.Key1Value == keyValue)//如果确认的是第一把钥匙
                {
                    mKeyManager.Key1Flag = true;
                }


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
            byte mAddr_1 = (byte)mArgs.CutError_1;
            byte mAddr_2 = (byte)mArgs.CutError_2;
            byte mAddr_3 = (byte)mArgs.ShortError_1;
            byte mAddr_4 = (byte)mArgs.ShortError_2;

            int keyValue = mAddr_1 << 16 | mAddr_2 << 8 | mAddr_3;

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
        private void SetMainText(object sender, STEP_LEVEL _level)
        {
            if (_level == STEP_LEVEL.FAIL || _level == STEP_LEVEL.PASS)
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

        #region 检查钥匙1
        private void SetChkKey1(object sender, INFO_LEVEL level)
        {
            if(CheckKey1Handler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                CheckKey1Handler(sender, mArgs);
            }
        }
        #endregion

        #region 检查钥匙2
        private void SetChkKey2(object sender, INFO_LEVEL level)
        {
            if (CheckKey2Handler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                CheckKey2Handler(sender, mArgs);
            }
        }
        #endregion

        #region 清除钥匙
        private void SetClearKey(object sender, INFO_LEVEL level)
        {
            if (ClearKeyHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                ClearKeyHandler(sender, mArgs);
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
            mKeyManager = new KeyManager();
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
            InitTask();
            bTaskRunning = true;
            mBroadCastTask.Excute();
            SetMainText(this, STEP_LEVEL.WAIT_POWER);
        }
    }
}
