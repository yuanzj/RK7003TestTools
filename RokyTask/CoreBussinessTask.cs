using CommonUtils;
using RK7001Test;
using Roky;
using RokyTask.Entity.Protocols.request;
using RokyTask.Entity.Protocols.response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace RokyTask
{
    public enum INFO_LEVEL
    {
        NONE = 0,
        INIT = 1,
        PROCESS = 2,
        FAIL = 3,
        PASS = 4,
        ONLY_TIP = 5,
        GREY = 6,
    }

    public enum RK7001ITEM
    {
        NONE = 0,
        INIT = 1,
        VERSION = 2,
        AMPLIFY = 3,
        MOSFET = 4,
        BUZZER = 5
    }

    public enum RK4110ITEM
    {
        NONE = 0,
        INIT = 1,
        VERSION = 2,
        LIGHTSENSOR = 3,
        GSENSOR = 4,
        PWM = 5,
        BLE = 6,
    }

    public enum TASK_TYPE
    {
        NONE = 0,
        CHK_TEST_SERVER = 1,
        RK7001_CHKVERSION = 2,
        RK7001_SELFTEST = 3,
        RK4110_POWERON = 4,
        RK4110_SELFTEST = 5,
        RK4110_PARAMSETTING = 6,
        RK4110_WRITENV = 7,
        RK4110_BLETEST = 8,
    }

    public class DeviceInfo
    {
        public string sn { get; set; }
        public string sw { get; set; }
        public string hw { get; set; }
    }

    public class UIEventArgs : EventArgs
    {
        public int num { get; set; }
        public int key1 { get; set; }
        public int key2 { get; set; }
        public string msg { get; set; }
        public string submsg { get; set; }
        public INFO_LEVEL level { get; set; }
    }

    public class RK7001ItemsArgs : EventArgs
    {
        public DeviceInfo info { get; set; }
        public RK7001ITEM items { get; set; }
        public INFO_LEVEL level { get; set; }
    }

    public class RK4110ItemsArgs : EventArgs
    {
        public DeviceInfo info { get; set; }
        public RK4110ITEM items { get; set; }
        public INFO_LEVEL level { get; set; }
    }

    public class PinStatusArgs : EventArgs
    {
        public PIN_STATUS status { get; set; }
        public INFO_LEVEL level { get; set; }
    }

    public enum RK7001ResultType
    {
        NONE_CHECK = 0,
        NORMAL_CHECK = 1,
        SERVER_KSI_OFF_RSP = 2,
        SERVER_KSI_ON_RSP = 3,
        CLIENT_KSI_ON = 4,
        CLIENT_KSI_OFF = 5,
        SERVER_REMOTE_RSP = 6,
        CLIENT_REMOTE_RSP = 7,
        OPEN_BUZZER_RSP = 8,
    }

    public enum TaskSteps
    {
        Step0_Init = 0,
        Step1_SelfTest = 1,        
        Step2_CheckEcuOpen = 2,
        Step3_SoftCloseEcu = 3,
        Step4_CheckEcuClose = 4,
        Step5_OpenKSI = 5,
        Step6_CloseKSI = 6,
        Step7_RemoteInit = 7,
        Step8_RemoteTest = 8,
        Step11_RetryRemote = 11,
        Step12_CloseVddCCu = 12,
        Step13_CheckVddCCu = 13,
        Step14_IronHornShort = 14,
        Step15_LeftLightShort = 15,
        Step16_RightLightShort = 16,
        Step17_HeadLightShort = 17,
        Step18_FarLightShort = 18,
        Step19_TailLightShort = 19,
        Step20_BrakeLightShort = 20,
        Step21_PotSwitchShort = 21,
        Step22_LeftBackLightShort = 22,
        Step23_RightBackLightShort = 23,
        Step24_BackGroundLightShort = 24,
        Step25_SpareInit = 25,
    }

    public enum Task_Level
    {
        FALSE = 0,
        TRUE = 1,
        REPEAT = 2,
    }

    public enum FACTORY_MODE
    {
        TEST_MODE = 0,
        CHECK_MODE = 2,
    }

    #region PIN脚状态
    public class PIN_STATUS
    {
        public bool Pin1_Open { get; set; }
        public bool Pin1_Short { get; set; }
        public bool Pin2_Open { get; set; }
        public bool Pin2_Short { get; set; }
        public bool Pin3_Open { get; set; }
        public bool Pin3_Short { get; set; }
        public bool Pin4_Open { get; set; }
        public bool Pin4_Short { get; set; }
        public bool Pin5_Open { get; set; }
        public bool Pin5_Short { get; set; }
        public bool Pin6_Open { get; set; }
        public bool Pin6_Short { get; set; }
        public bool Pin7_Open { get; set; }
        public bool Pin7_Short { get; set; }
        public bool Pin8_Open { get; set; }
        public bool Pin8_Short { get; set; }
        public bool Pin9_Open { get; set; }
        public bool Pin9_Short { get; set; }
        public bool Pin10_Open { get; set; }
        public bool Pin10_Short { get; set; }
        public bool Pin11_Open { get; set; }
        public bool Pin11_Short { get; set; }
        public bool Pin12_Open { get; set; }
        public bool Pin12_Short { get; set; }
        public bool Pin13_Open { get; set; }
        public bool Pin13_Short { get; set; }
        public bool Pin14_Open { get; set; }
        public bool Pin14_Short { get; set; }
        public bool Pin15_Open { get; set; }
        public bool Pin15_Short { get; set; }
        public bool Pin16_Open { get; set; }
        public bool Pin16_Short { get; set; }
        public bool Pin17_Open { get; set; }
        public bool Pin17_Short { get; set; }
        public bool Pin18_Open { get; set; }
        public bool Pin18_Short { get; set; }
        public bool Pin19_Open { get; set; }
        public bool Pin19_Short { get; set; }
        public bool Pin20_Open { get; set; }
        public bool Pin20_Short { get; set; }
        public bool Pin21_Open { get; set; }
        public bool Pin21_Short { get; set; }
        public bool Pin22_Open { get; set; }
        public bool Pin22_Short { get; set; }
        public bool Pin23_Open { get; set; }
        public bool Pin23_Short { get; set; }
        public bool Pin24_Open { get; set; }
        public bool Pin24_Short { get; set; }
        public bool Pin25_Open { get; set; }
        public bool Pin25_Short { get; set; }
        public bool Pin26_Open { get; set; }
        public bool Pin26_Short { get; set; }
        public bool Pin27_Open { get; set; }
        public bool Pin27_Short { get; set; }
        public bool Pin28_Open { get; set; }
        public bool Pin28_Short { get; set; }
        public bool Pin29_Open { get; set; }
        public bool Pin29_Short { get; set; }
        public bool Pin30_Open { get; set; }
        public bool Pin30_Short { get; set; }
    }
    #endregion


    public class CoreBussinessTask : ITaskManager
    {
        #region 私有变量
        public bool bTaskRunning { get; set; }

        private TASK_TYPE mTaskType { get; set; }

        private RK7001ResultType mRK7001ResultType { get; set; }

        private TaskSteps mTaskSteps;

        private int ReTryCnts { get; set; }

        private PIN_STATUS mRK7001Pins;
        private PIN_STATUS mRK4110Pins;

        public FACTORY_MODE mode { get; set; }

        public bool bCruised { get; set; }
        public bool bRepaired { get; set; }
        public bool bPushcar { get; set; }
        public bool bBackcar { get; set; }
        public bool bTest4103 { get; set; }
        public bool bLcm_P { get; set; }
        public bool bLcm_F { get; set; }
        public bool bLcm_R { get; set; }
        public bool bLcm_L { get; set; }
        public bool bLcm_RD { get; set; }
        public bool bAnt { get; set; }
        public bool bLcm_CS { get; set; }
        public bool bLcm_CLK { get; set; }
        public bool bLcm_DO { get; set; }

        public bool bServerActivated { get; set; }
        public string DefaultSN { get; set; }
        public string DefaultBT { get; set; }
        public string DefaultBLE { get; set; }
        public string DefaultKeyt { get; set; }
        
        public string mSqlserverConnString { get; set; }
        public string mSN { get; set; }
        private string mBTaddr { get; set; }
        private string mBLEaddr { get; set; }
        private string mKeyt { get; set; }
        private string mUid { get; set; }
        private string mTestResult { get; set; }
        #endregion

        #region 注册事件
        public event EventHandler UpdateWorkStatusHandler;
        public event EventHandler UpdateValidSNHandler;
        public event EventHandler UpdateChkServerHandler;
        public event EventHandler Update4103ListViewHandler;
        public event EventHandler Update4103PinListHandler;
        public event EventHandler Update7001ListViewHandler;
        public event EventHandler Update7001PinListHandler;
        public event EventHandler UpdateRemoteHandler;
        public event EventHandler UpdateListViewHandler;
        public event EventHandler UpdatePotTickerHandler;
        public event EventHandler UpdateVddGpsHandler;
        #endregion
        
        #region 注册串口报文事件
        //获取RK7001的版本号
        SimpleSerialPortTask<get7001Version, get7001VersionRsp> mGet7001VersionTask;
        get7001Version mGet7001VersionParam;
        //获取RK7001的测试结果
        SimpleSerialPortTask<get7001Result, get7001ResultRsp> mGet7001ResultTask;
        get7001Result mGet700ResultParam;
        
        SimpleSerialPortTask<chk4103Server, chk4103ServerRsp> mChk4103ServerTask;
        chk4103Server mChk4103ServerParam;
        //获取中控信息
        SimpleSerialPortTask<getDevinfoReq, getDevinfoRsp> mGetDevinfoTask;
        getDevinfoReq mGetDevinfoParam;
        //接受0x06
        SimpleSerialPortTask<NullEntity, get4103BroadcastReq> mFirstRunningReqTask;
        //发送0x86，接受0x03
        SimpleSerialPortTask<get4103BroadcastRsp, ParameterSettingReq> mParamSettingReqTask;
        get4103BroadcastRsp mFirstRunRspParam;
        //发送0x83， 接受0x23
        SimpleSerialPortTask<ParameterSettingRsp, boardTestResultReq> mRecvTestResultTask;
        ParameterSettingRsp mParamSettingParam;
        //发送0xA3，接受0x24
        SimpleSerialPortTask<boardTestResultRsp, SaveNvReq> mSaveNvReqTask;
        boardTestResultRsp mTestResultRspParam;
        //发送0xA4,接受0x26
        SimpleSerialPortTask<SaveNvRsp, btTestReq> mBtTestReqTask;
        SaveNvRsp mSaveNvRspParam;
        //发送0xA6
        SimpleSerialPortTask<btTestRsp, NullEntity> mCompeteTestTask;
        btTestRsp mBtTestRspParam;
        
        //执行Timer
        System.Timers.Timer DynamicPotTicker;
        private int PotTickCnt { get; set; }
        #endregion

        #region MyRegion
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region 构造函数
        public CoreBussinessTask()
        {            
            TaskBuilder();
        }
        #endregion

        private void TaskBuilder()
        {
            #region 获取中控信息
            mGetDevinfoTask = new SimpleSerialPortTask<getDevinfoReq, getDevinfoRsp>();
            mGetDevinfoParam = mGetDevinfoTask.GetRequestEntity();
            mGetDevinfoParam.devType = 0X08;
            mGetDevinfoTask.Timerout = 1000;
            mGetDevinfoTask.RetryMaxCnts = 1;
            mGetDevinfoTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<getDevinfoRsp> mEventArgs = e as SerialPortEventArgs<getDevinfoRsp>;
                if (mEventArgs.Data != null)
                {
                    mParamSettingParam.sn = mEventArgs.Data.devSN;
                    mParamSettingParam.edrAddr = mEventArgs.Data.devBTaddr;
                    mParamSettingParam.bleAddr = mEventArgs.Data.devBLEaddr;
                    mParamSettingParam.key = mEventArgs.Data.devKeyt;
                    byte[] temp = new byte[4];
                    mParamSettingParam.adcParam = temp;
                    mParamSettingParam.v15Param = temp;
                    mParamSettingParam.sntemp = temp;
                    SetMainText(sender, "等待4103参数配置请求...", "", INFO_LEVEL.PROCESS);
                    mParamSettingReqTask.Excute();
                    string sn = ByteProcess.byteArrayToString(mEventArgs.Data.devSN);
                    byte[] edrByteArray = new byte[6];
                    edrByteArray = mEventArgs.Data.devBTaddr;
                    Array.Reverse(edrByteArray);
                    string edr = Util.ToHexString(edrByteArray);
                    string ble = Util.ToHexString(mEventArgs.Data.devBLEaddr);
                    string key = ByteProcess.byteArrayToString(mEventArgs.Data.devKeyt);
                    string devInfo = String.Format("SN:{0},BT2.1地址:{1},BLE4.0地址:{2},密钥:{3}",sn, edr, ble,key);
                    UpdateListView(sender, "当前设备信息", devInfo);
                }
                else
                {
                    SetMainText(sender, "未获取中控设备信息！", "SAVENONE", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "未获取中控设备信息", "485失去通讯，或者夹具中断异常！");
                    UpdateTestServer(sender, INFO_LEVEL.FAIL);
                    StopTask();
                }
            };
            #endregion

            #region 显示POS
            DynamicPotTicker = new System.Timers.Timer(1000);
            PotTickCnt = 0;
            DynamicPotTicker.Enabled = false;
            DynamicPotTicker.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs ElapsedEventArgs) =>
            {
                switch (PotTickCnt)
                {
                    case 0:
                        UpdatePotTicker(source, ".", INFO_LEVEL.PROCESS);
                        PotTickCnt++;
                        break;
                    case 1:
                        UpdatePotTicker(source, ". .", INFO_LEVEL.PROCESS);
                        PotTickCnt++;
                        break;
                    case 2:
                        UpdatePotTicker(source, ". . .", INFO_LEVEL.PROCESS);
                        PotTickCnt = 0;
                        break;
                    
                }
            });
            #endregion

            #region 自检报文
            mGet7001ResultTask = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mGet700ResultParam = mGet7001ResultTask.GetRequestEntity();
            mGet7001ResultTask.RetryMaxCnts = 10;
            mGet7001ResultTask.Timerout = 1000;//10s超时
            mGet700ResultParam.ack_device = Const.PCU;//发给PCU
            mGet700ResultParam.ecu_status = 0x34;//默认;//默认
            mGet700ResultParam.server_mode = 0x04;//开启软件上电
            mGet700ResultParam.backlight = 0;
            mGet700ResultParam.batt_soc = 0;
            mGet700ResultParam.level_ctrl = 0x0100;
            mGet700ResultParam.limit_per = 0;
            mGet700ResultParam.trigger_ctrl = 0;            
            mGet7001ResultTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {
                    bool bExcute = false;                    
                    Task_Level level = Task_Level.FALSE;
                    switch (mTaskSteps)
                    {
                        case TaskSteps.Step0_Init:
                            if (ReTryCnts++ >= 1)
                            {
                                mTaskSteps = TaskSteps.Step14_IronHornShort;                               
                                mGet700ResultParam.level_ctrl = 0x0100;//开启铁喇叭
                            }
                            break;                        
                        case TaskSteps.Step14_IronHornShort:
                            level = CheckSampleCurrent(sender, mEventArgs.Data);
                            string ironhorn = String.Format("铁喇叭 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                            Log.Info(ironhorn);
                            if (level == Task_Level.FALSE)
                            {
                                bExcute = true;
                                mRK7001Pins.Pin4_Open = true;
                                UpdateListView(sender, "7010 铁喇叭", "Mosfet可能短路");
                            }
                            else if(level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step15_LeftLightShort;
                                mGet700ResultParam.level_ctrl = 0x0200;//左转灯短路
                                ReTryCnts = 0;
                            }
                            break;                        
                        case TaskSteps.Step15_LeftLightShort:
                            level = getLeftRightLightCurrent(sender, mEventArgs.Data);                            
                            if (ReTryCnts++ >= 1)
                            {
                                string leftlight = String.Format("左转灯 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                                Log.Info(leftlight);
                                if (level == Task_Level.FALSE)
                                {
                                    bExcute = true;
                                    mRK7001Pins.Pin25_Open = true;
                                    mRK7001Pins.Pin28_Open = true;
                                    UpdateListView(sender, "7010 左转灯", "Mosfet可能短路");
                                    UpdateListView(sender, "7010 后左转灯", "Mosfet可能短路");
                                }
                                else if (level == Task_Level.TRUE)
                                {
                                    mTaskSteps = TaskSteps.Step16_RightLightShort;
                                    mGet700ResultParam.level_ctrl = 0x0400;//右转灯短路
                                    ReTryCnts = 0;
                                }
                            }                            
                            break;
                        case TaskSteps.Step16_RightLightShort:
                            level = getLeftRightLightCurrent(sender, mEventArgs.Data);                            
                            if (ReTryCnts++ >= 1)
                            {
                                string rightlight = String.Format("右转灯 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                                Log.Info(rightlight);
                                if (level == Task_Level.FALSE)
                                {
                                    bExcute = true;
                                    mRK7001Pins.Pin24_Open = true;
                                    mRK7001Pins.Pin27_Open = true;
                                    UpdateListView(sender, "7010 右转灯", "Mosfet可能短路");
                                    UpdateListView(sender, "7010 后右转灯", "Mosfet可能短路");
                                }
                                else if (level == Task_Level.TRUE)
                                {
                                    mTaskSteps = TaskSteps.Step17_HeadLightShort;
                                    mGet700ResultParam.level_ctrl = 0x0800;//近光灯短路
                                    ReTryCnts = 0;
                                }
                            }                            
                            break;
                        case TaskSteps.Step17_HeadLightShort:
                            level = CheckSampleCurrent(sender, mEventArgs.Data);
                            if (ReTryCnts++ >= 1)
                            {
                                string headlight = String.Format("前车灯 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                                Log.Info(headlight);
                                if (level == Task_Level.FALSE)
                                {
                                    bExcute = true;
                                    mRK7001Pins.Pin5_Open = true;
                                    UpdateListView(sender, "7010 近光灯", "Mosfet可能短路");
                                }
                                else if (level == Task_Level.TRUE)
                                {
                                    mTaskSteps = TaskSteps.Step18_FarLightShort;
                                    mGet700ResultParam.level_ctrl = 0x1000;//远光灯短路
                                    ReTryCnts = 0;
                                }
                            }                            
                            break;
                        case TaskSteps.Step18_FarLightShort:
                            level = CheckSampleCurrent(sender, mEventArgs.Data);
                            if (ReTryCnts++ >= 1)
                            {
                                string farlight = String.Format("远光灯 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                                Log.Info(farlight);
                                if (level == Task_Level.FALSE)
                                {
                                    bExcute = true;
                                    mRK7001Pins.Pin6_Open = true;
                                    UpdateListView(sender, "7010 远光灯", "Mosfet可能短路");
                                }
                                else if (level == Task_Level.TRUE)
                                {
                                    mTaskSteps = TaskSteps.Step19_TailLightShort;
                                    mGet700ResultParam.level_ctrl = 0x2000;//尾灯短路
                                    ReTryCnts = 0;
                                }
                            }                            
                            break;
                        case TaskSteps.Step19_TailLightShort:
                            level = CheckSampleCurrent(sender, mEventArgs.Data);
                            if (ReTryCnts++ >= 1)
                            {
                                string taillight = String.Format("尾灯 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                                Log.Info(taillight);
                                if (level == Task_Level.FALSE)
                                {
                                    bExcute = true;
                                    mRK7001Pins.Pin2_Open = true;
                                    UpdateListView(sender, "7010 尾灯", "Mosfet可能短路");
                                }
                                else if (level == Task_Level.TRUE)
                                {
                                    mTaskSteps = TaskSteps.Step20_BrakeLightShort;
                                    mGet700ResultParam.level_ctrl = 0x4000;//刹车短路
                                    ReTryCnts = 0;
                                }
                            }                           
                            break;
                        case TaskSteps.Step20_BrakeLightShort:
                            level = CheckSampleCurrent(sender, mEventArgs.Data);
                            if (ReTryCnts++ >= 1)
                            {
                                string brakelight = String.Format("刹车灯 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                                Log.Info(brakelight);
                                if (level == Task_Level.FALSE)
                                {
                                    bExcute = true;
                                    mRK7001Pins.Pin3_Open = true;
                                    UpdateListView(sender, "7010 刹车灯", "Mosfet可能短路");
                                }
                                else if (level == Task_Level.TRUE)
                                {
                                    mTaskSteps = TaskSteps.Step24_BackGroundLightShort;
                                    mGet700ResultParam.level_ctrl = 0x0000;//背景灯红短路
                                    mGet700ResultParam.backlight = 0xff0000;
                                    ReTryCnts = 0;
                                }
                            }                            
                            break;                                                   
                        case TaskSteps.Step24_BackGroundLightShort:
                            level = CheckSampleCurrent(sender, mEventArgs.Data);
                            if (ReTryCnts++ >= 1)
                            {
                                string red = String.Format("背景灯红 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                                Log.Info(red);
                                if (level == Task_Level.FALSE)
                                {
                                    bExcute = true;
                                    mRK7001Pins.Pin23_Open = true;
                                    UpdateListView(sender, "7010 背景灯红灯", "Mosfet可能短路");
                                }
                                else if (level == Task_Level.TRUE)
                                {
                                    mTaskSteps = TaskSteps.Step25_SpareInit;
                                    mGet700ResultParam.ack_device = Const.PCU;//发给PCU
                                    mGet700ResultParam.ecu_status = 0x34;//默认;//默认
                                    mGet700ResultParam.server_mode = 0x04;//开启软件上电
                                    mGet700ResultParam.backlight = 0;
                                    mGet700ResultParam.batt_soc = 0;
                                    mGet700ResultParam.level_ctrl = 0x0100;//开启铁喇叭
                                    mGet700ResultParam.limit_per = 0;
                                    mGet700ResultParam.trigger_ctrl = 0;
                                    ReTryCnts = 0;
                                }
                            }                            
                            break;
                        case TaskSteps.Step25_SpareInit:
                            if (ReTryCnts++ >= 1)
                            {
                                mTaskSteps = TaskSteps.Step1_SelfTest;
                                mGet700ResultParam.ack_device = Const.PCU;//发给PCU
                                mGet700ResultParam.ecu_status = 0x34;//默认;//默认
                                mGet700ResultParam.server_mode = 0x04;//开启软件上电
                                mGet700ResultParam.backlight = 0;
                                mGet700ResultParam.batt_soc = 0;
                                mGet700ResultParam.level_ctrl = 0x0100;//开启铁喇叭
                                mGet700ResultParam.limit_per = 0;
                                mGet700ResultParam.trigger_ctrl = 0;
                                ReTryCnts = 0;
                            }
                            break;
                        case TaskSteps.Step1_SelfTest:
                            level = Step1_SelfTest(sender, mEventArgs.Data);
                            string selfCheck = String.Format("自检 开启铁喇叭 采样电流:{0}", mEventArgs.Data.DcCurrent * 100);
                            Log.Info(selfCheck);
                            if (level == Task_Level.FALSE)
                                bExcute = true;
                            else if (level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step2_CheckEcuOpen;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;//发给SERVER
                                mGet700ResultParam.ecu_status = 0x34;
                                mGet700ResultParam.server_mode = 0x4;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if (level == Task_Level.REPEAT)
                            {

                            }                             
                            break;
                        case TaskSteps.Step2_CheckEcuOpen:
                            level = Step2_CheckEcuOpen(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                                bExcute = true;
                            else if(level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step3_SoftCloseEcu;
                                mGet700ResultParam.ack_device = Const.PCU;//发送PCU
                                mGet700ResultParam.ecu_status = 0x20;
                                mGet700ResultParam.server_mode = 0x0;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.REPEAT)
                            {

                            }
                            break;
                        case TaskSteps.Step3_SoftCloseEcu:
                            level = Step3_SoftCloseEcu(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                                bExcute = true;
                            else if(level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step4_CheckEcuClose;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;//发给SERVER
                                mGet700ResultParam.ecu_status = 0x20;
                                mGet700ResultParam.server_mode = 0x0;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.REPEAT)
                            {

                            }
                            break;
                        case TaskSteps.Step4_CheckEcuClose:
                            level = Step4_CheckEcuClose(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                                bExcute = true;
                            else if(level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step5_OpenKSI;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;//发给SERVER
                                mGet700ResultParam.ecu_status = 0x20;
                                mGet700ResultParam.server_mode = 0x2;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step4_CheckEcuClose;
                            }
                            break;
                        case TaskSteps.Step5_OpenKSI:
                            level = Step5_OpenKSI(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                                bExcute = true;
                            else if(level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step6_CloseKSI;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;//发给SERVER
                                mGet700ResultParam.ecu_status = 0x20;
                                mGet700ResultParam.server_mode = 0x0;
                                mGet700ResultParam.level_ctrl = 0x0100;
                                ReTryCnts = 0;
                            }
                            else if(level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step5_OpenKSI;
                            }
                            break;
                        case TaskSteps.Step6_CloseKSI:
                            level = Step6_CloseKSI(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                            {                                
                                bExcute = true;                                
                            }                                
                            else if(level == Task_Level.TRUE)
                            {
                                //设置pin脚
                                UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.PASS);
                                mTaskSteps = TaskSteps.Step7_RemoteInit;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;
                                mGet700ResultParam.ecu_status = 0x20;
                                mGet700ResultParam.server_mode = 0x1;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step6_CloseKSI;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;//发给SERVER
                                mGet700ResultParam.ecu_status = 0x20;
                                mGet700ResultParam.server_mode = 0x0;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            break;
                        case TaskSteps.Step7_RemoteInit:
                            level = Step7_RemoteInit(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                                bExcute = true;
                            else if(level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step8_RemoteTest;
                                mGet700ResultParam.ack_device = Const.PCU;
                                mGet700ResultParam.ecu_status = 0x20;
                                mGet700ResultParam.server_mode = 0x1;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step7_RemoteInit;
                            }
                            break;
                        case TaskSteps.Step8_RemoteTest:
                            level = Step8_RemoteTest(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                                mTaskSteps = TaskSteps.Step11_RetryRemote;
                            else if(level == Task_Level.TRUE)
                            {
                                UpdateRemoteStatus(sender, INFO_LEVEL.PASS);                                
                                //关闭VDD_CCU
                                mTaskSteps = TaskSteps.Step12_CloseVddCCu;
                                mGet700ResultParam.ack_device = Const.PCU;
                                mGet700ResultParam.ecu_status = 0x0;
                                mGet700ResultParam.server_mode = 0x0;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step8_RemoteTest;
                            }
                            break;
                        case TaskSteps.Step11_RetryRemote:
                            level = Step8_RemoteTest(sender, mEventArgs.Data);
                            if (level == Task_Level.FALSE)
                            {
                                UpdateListView(sender, "7010 遥控电路测试失败", "遥控电路有问题");
                                UpdateRemoteStatus(sender, INFO_LEVEL.FAIL);
                                bExcute = true;
                            }
                            else if (level == Task_Level.TRUE)
                            {
                                UpdateRemoteStatus(sender, INFO_LEVEL.PASS);                               
                                //关闭VDD_CCU
                                mTaskSteps = TaskSteps.Step12_CloseVddCCu;
                                mGet700ResultParam.ack_device = Const.PCU;
                                mGet700ResultParam.ecu_status = 0x0;
                                mGet700ResultParam.server_mode = 0x0;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if (level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step8_RemoteTest;
                            }
                            break;                        
                        case TaskSteps.Step12_CloseVddCCu:
                            level = Step12_CloseVddCCu(sender, mEventArgs.Data);
                            if(level == Task_Level.FALSE)
                            {
                                mTaskSteps = TaskSteps.Step12_CloseVddCCu;
                                mGet700ResultParam.ack_device = Const.PCU;
                                mGet700ResultParam.ecu_status = 0x00;
                                mGet700ResultParam.server_mode = 0x0;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.TRUE)
                            {
                                mTaskSteps = TaskSteps.Step13_CheckVddCCu;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;
                                mGet700ResultParam.ecu_status = 0x34;
                                mGet700ResultParam.server_mode = 0x8;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step12_CloseVddCCu;
                                mGet700ResultParam.ack_device = Const.PCU;
                                mGet700ResultParam.ecu_status = 0x0;
                                mGet700ResultParam.server_mode = 0x1;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            break;
                        case TaskSteps.Step13_CheckVddCCu:
                            level = Step13_CheckVddCCu(sender, mEventArgs.Data);
                            if(level == Task_Level.FALSE)
                            {
                                mTaskSteps = TaskSteps.Step13_CheckVddCCu;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;
                                mGet700ResultParam.ecu_status = 0x0;
                                mGet700ResultParam.server_mode = 0x8;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            else if(level == Task_Level.TRUE)
                            {                                
                                string mRest = String.Format("{0}\n测 试 成 功！", mSN);
                                //保存数据
                                SetMainText(sender, mRest, "PASS", INFO_LEVEL.PASS);
                                StopTask();
                                return;
                            }
                            else if(level == Task_Level.REPEAT)
                            {
                                mTaskSteps = TaskSteps.Step13_CheckVddCCu;
                                mGet700ResultParam.ack_device = Const.TESTSERVER;
                                mGet700ResultParam.ecu_status = 0x0;
                                mGet700ResultParam.server_mode = 0x8;
                                mGet700ResultParam.level_ctrl = 0x0100;
                            }
                            break;                            
                    }
                    if(!bExcute)
                    {                        
                        mGet7001ResultTask.Excute();
                        Thread.Sleep(500);
                    } 
                    else
                    {
                        SetMainText(sender, "测  试  失  败!", "FAIL", INFO_LEVEL.FAIL);
                        UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.FAIL);
                        StopTask();
                        return;
                    }                                                         
                }
                else
                {
                    SetMainText(sender, "自 检 超 时！", "FAIL", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试异常中断", "夹具被弹起，或485通讯失败");
                    StopTask();
                }                
            };
            #endregion

            #region 查找UID报文
            mGet7001VersionTask = new SimpleSerialPortTask<get7001Version, get7001VersionRsp>();
            mGet7001VersionParam = mGet7001VersionTask.GetRequestEntity();
            mGet7001VersionTask.RetryMaxCnts = 30;
            mGet7001VersionTask.Timerout = 1000; //重试30次
            mGet7001VersionParam.deviceType = 0x07;
            mGet7001VersionParam.battCurrent = 18;
            mGet7001VersionParam.battVolatage = 48;
            mGet7001VersionParam.underVoltage = 0;
            mGet7001VersionTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001VersionRsp> mEventArgs = e as SerialPortEventArgs<get7001VersionRsp>;
                if (mEventArgs.Data != null)
                {
                    string uid = "";
                    byte[] uidbyteArray = new byte[12];
                    uidbyteArray[0] = (byte)mEventArgs.Data.uid12;
                    uidbyteArray[1] = (byte)mEventArgs.Data.uid11;
                    uidbyteArray[2] = (byte)mEventArgs.Data.uid10;
                    uidbyteArray[3] = (byte)mEventArgs.Data.uid9;
                    uidbyteArray[4] = (byte)mEventArgs.Data.uid8;
                    uidbyteArray[5] = (byte)mEventArgs.Data.uid7;
                    uidbyteArray[6] = (byte)mEventArgs.Data.uid6;
                    uidbyteArray[7] = (byte)mEventArgs.Data.uid5;
                    uidbyteArray[8] = (byte)mEventArgs.Data.uid4;
                    uidbyteArray[9] = (byte)mEventArgs.Data.uid3;
                    uidbyteArray[10] = (byte)mEventArgs.Data.uid2;
                    uidbyteArray[11] = (byte)mEventArgs.Data.uid1;

                    int j = 0;
                    for (int i = 0; i < 12; i++)
                    {
                        if (uidbyteArray[i] == 0)
                            j++;
                    }
                    //uid
                    for (int i = 0; i < 12; i++)
                    {
                        uid += uidbyteArray[i].ToString("X02");
                    }

                    mUid = uid;
                    //hw
                    byte[] hwArray = new byte[4];
                    hwArray[0] = (byte)mEventArgs.Data.hw1;
                    hwArray[1] = (byte)mEventArgs.Data.hw2;
                    hwArray[2] = (byte)mEventArgs.Data.hw3;
                    hwArray[3] = (byte)mEventArgs.Data.hw4;

                    //sw      
                    byte[] swArray = new byte[4];
                    swArray[0] = (byte)mEventArgs.Data.sw1;
                    swArray[1] = (byte)mEventArgs.Data.sw2;
                    swArray[2] = (byte)mEventArgs.Data.sw3;
                    swArray[3] = (byte)mEventArgs.Data.sw4;

                    DeviceInfo info = new DeviceInfo();
                    info.sn = uid;
                    info.hw = String.Format("RK{0}", System.Text.Encoding.Default.GetString(hwArray));
                    info.sw = String.Format("W{0}{1:D2}.{2:D2}", swArray[1], swArray[2], swArray[3]);
                    UpdateRK7001Items(sender, RK7001ITEM.VERSION, info, INFO_LEVEL.NONE);
                    if (j >= 10)
                    {
                        SetMainText(sender, "RK7010未上电", "FAIL", INFO_LEVEL.FAIL);
                        StopTask();
                        return;
                    }
                    //开始执行查询
                    mTaskSteps = TaskSteps.Step0_Init;
                    mGet7001ResultTask.Excute();
                    Thread.Sleep(500);
                    SetMainText(sender, "RK7010自测中...", "", INFO_LEVEL.PROCESS);
                }
                else
                {
                    SetMainText(sender, "RK7010未上电", "FAIL", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试异常中断", "夹具被弹起，或485通讯失败");
                    StopTask();
                }
            };
#endregion

            #region 检查测试Server
            mChk4103ServerTask = new SimpleSerialPortTask<chk4103Server, chk4103ServerRsp>();
            mChk4103ServerParam = mChk4103ServerTask.GetRequestEntity();
            mChk4103ServerTask.RetryMaxCnts = 10;
            mChk4103ServerTask.Timerout = 1000;
            mChk4103ServerParam.deviceType = 0x02;
            mChk4103ServerTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<chk4103ServerRsp> mEventArgs = e as SerialPortEventArgs<chk4103ServerRsp>;
                if (mEventArgs.Data != null)
                {
                    UpdateTestServer(sender, INFO_LEVEL.PASS);
                    //先来获取版本号和uid
                    SetMainText(sender, "等待设备上电中...", "", INFO_LEVEL.PROCESS);
                    mFirstRunningReqTask.Excute();                                        
                }
                else
                {
                    SetMainText(sender, "测试Server未启动!", "SAVENONE", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试Server未启动", "485失去通讯，或者Server启动慢，或者Server程序不同");
                    UpdateTestServer(sender, INFO_LEVEL.FAIL);
                    StopTask();
                }
            };
            #endregion

            #region RK4110上电 发送广播报文
            mFirstRunningReqTask = new SimpleSerialPortTask<NullEntity, get4103BroadcastReq>();
            mFirstRunningReqTask.RetryMaxCnts = 1;
            mFirstRunningReqTask.Timerout = 20*1000;
            bTest4103 = false;
            mFirstRunningReqTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get4103BroadcastReq> mEventArgs = e as SerialPortEventArgs<get4103BroadcastReq>;
                if (mEventArgs.Data != null)
                {
                    byte hw1 = (byte)(mEventArgs.Data.hardwareID >> 24);
                    byte hw2 = (byte)(mEventArgs.Data.hardwareID >> 16);
                    byte hw3 = (byte)(mEventArgs.Data.hardwareID >> 8);

                    string softVersion = String.Format("W{0}{1:D2}.{2:D2}", (byte)(mEventArgs.Data.hardwareID >> 24), 
                                                                       (byte)(mEventArgs.Data.hardwareID >> 16), 
                                                                       (byte)(mEventArgs.Data.hardwareID >> 8));
                    DeviceInfo info = new DeviceInfo();
                    info.sw = softVersion;
                    UpdateRK4110Items(sender, RK4110ITEM.VERSION, info, INFO_LEVEL.NONE);

                    mFirstRunRspParam.deviceType = 0x01;
                    mFirstRunRspParam.firmwareYears = (byte)(mEventArgs.Data.hardwareID >> 24);
                    mFirstRunRspParam.firmwareWeeks = (byte)(mEventArgs.Data.hardwareID >> 16);
                    mFirstRunRspParam.firmwareVersion = (byte)(mEventArgs.Data.hardwareID >> 8);
                    mFirstRunRspParam.hardwareID = 0xFB;
                    if (mode == FACTORY_MODE.TEST_MODE)
                    {                        
                        SetMainText(sender, "等待4103参数配置请求...", "", INFO_LEVEL.PROCESS);
                        mParamSettingReqTask.Excute();
                    }
                    else if(mode == FACTORY_MODE.CHECK_MODE)
                    {                        
                        SetMainText(sender, "获得中控设备信息...", "", INFO_LEVEL.PROCESS);
                        mGetDevinfoTask.Excute();
                    }
                }
                else
                {
                    SetMainText(sender, "待测设备未上电!", "FAIL", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试异常中断", "夹具被弹起，或485通讯失败");
                    StopTask();
                }
            };
            #endregion

            #region 发送0x86, 接受0x03
            mParamSettingReqTask = new SimpleSerialPortTask<get4103BroadcastRsp, ParameterSettingReq>();
            mFirstRunRspParam = mParamSettingReqTask.GetRequestEntity();
            mParamSettingReqTask.RetryMaxCnts = 10;
            mParamSettingReqTask.Timerout = 1000;            
            mParamSettingReqTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<ParameterSettingReq> mEventArgs = e as SerialPortEventArgs<ParameterSettingReq>;
                if (mEventArgs.Data != null)
                {
                    mParamSettingParam.deviceType = 0x03;
                    if(mode == FACTORY_MODE.TEST_MODE)
                    {
                        mParamSettingParam.testItem = 0xFF;//写号
                        mParamSettingParam.sn = ByteProcess.stringToByteArray(mSN);
                        byte[] aArray = new byte[6];
                        aArray = ByteProcess.stringToByteArrayNoColon(mBTaddr);
                        Array.Reverse(aArray);
                        mParamSettingParam.edrAddr = aArray;
                        mParamSettingParam.bleAddr = ByteProcess.stringToByteArrayNoColon(mBLEaddr);
                        mParamSettingParam.key = ByteProcess.stringToByteArray(mKeyt);
                        byte[] temp = new byte[4];
                        mParamSettingParam.adcParam = temp;
                        mParamSettingParam.v15Param = temp;
                        mParamSettingParam.sntemp = temp;
                    }
                    else if(mode == FACTORY_MODE.CHECK_MODE)
                    {
                        mParamSettingParam.testItem = 0xBF;//不写号
                    }                                        
                    //等待接受结果
                    SetMainText(sender, "RK4110 测试中...", "", INFO_LEVEL.PROCESS);
                    Thread.Sleep(5);
                    mRecvTestResultTask.Excute();
                }
                else
                {
                    SetMainText(sender, "未收到参数配置请求!", "FAIL", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试异常中断", "夹具被弹起，或485通讯失败");
                    StopTask();
                }
            };
#endregion

            #region 发送0x83， 接受0x23
            mRecvTestResultTask = new SimpleSerialPortTask<ParameterSettingRsp, boardTestResultReq>();
            mParamSettingParam = mRecvTestResultTask.GetRequestEntity();
            mRecvTestResultTask.RetryMaxCnts = 5;
            mRecvTestResultTask.Timerout = 10 * 1000;
            mRecvTestResultTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<boardTestResultReq> mEventArgs = e as SerialPortEventArgs<boardTestResultReq>;
                if (mEventArgs.Data != null)
                {
                    ushort m_result = (ushort)mEventArgs.Data.result;
                    int inputPin = (int)mEventArgs.Data.inputPin;
                    byte outpin1 = (byte)mEventArgs.Data.outpin1;
                    byte outpin2 = (byte)mEventArgs.Data.outpin2;
                    byte extensionOut = (byte)mEventArgs.Data.extensionOut;
                    bool InputPinError = false;
                    if(m_result == 0)
                    {
                        UpdateRK4110Items(sender, RK4110ITEM.INIT, null, INFO_LEVEL.PASS);
                        UpdateRK4110Pins(sender, mRK4110Pins, INFO_LEVEL.PASS);
                        Thread.Sleep(5);
                        mSaveNvReqTask.Excute();
                    }
                    else
                    {
                        if ((m_result >> 0 & 0x1) == 1)
                        {
                            UpdateRK4110Items(sender, RK4110ITEM.GSENSOR, null, INFO_LEVEL.FAIL);
                            InputPinError = true;
                            UpdateListView(sender, "Gsensor故障", "Gsensor芯片或者外围元件有问题");
                        }
                        else
                            UpdateRK4110Items(sender, RK4110ITEM.GSENSOR, null, INFO_LEVEL.PASS);
                        
                        if ((m_result >> 1 & 0x1) == 1)
                        {
                            UpdateRK4110Items(sender, RK4110ITEM.LIGHTSENSOR, null, INFO_LEVEL.FAIL);
                            InputPinError = true;
                            mRK4110Pins.Pin11_Open = true;
                            mRK4110Pins.Pin22_Open = true;
                            UpdateListView(sender, "光感异常", "VLCM或者ADC_IN的pin脚有问题或者电压不在范围内");
                        }
                        else
                            UpdateRK4110Items(sender, RK4110ITEM.LIGHTSENSOR, null, INFO_LEVEL.PASS);                        
                        /*
                        if ((m_result >> 2 & 0x1) == 1)
                        {
                            UpdateRK4110Items(sender, RK4110ITEM.VDD33, null, INFO_LEVEL.FAIL);
                            InputPinError = true;
                        }
                        else
                            UpdateRK4110Items(sender, RK4110ITEM.VDD33, null, INFO_LEVEL.PASS);                        
                        if((m_result >> 3 & 0x1) == 1)
                        {
                            mRK4110Pins.Pin30_Open = true;
                            InputPinError = true;
                            UpdateListView(sender, "VDD_GPS异常", "VDD_GPS脚异常或者测的电压不在范围内");
                        }
                        */
                        if ((m_result >> 4 & 0x1) == 1) 
                        {
                            if ((inputPin >> 0 & 0x1) == 1) //右转灯 => 档位(RK7010)
                            {
                                mRK4110Pins.Pin27_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103档位", "档位脚短路或者断路");
                            }
                            if ((inputPin >> 1 & 0x1) == 1)//左转灯 => P档（RK7010）
                            {
                                mRK4110Pins.Pin28_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103 P档", "P档脚短路或者断路");
                            }
                            if ((inputPin >> 2 & 0x1) == 1)//远近光 => 大灯（RK7010）
                            {
                                mRK4110Pins.Pin3_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103 大灯", "大灯灯脚短路或者断路");
                            }
                            if ((inputPin >> 3 & 0x1) == 1)//喇叭 => 双闪（RK7010）
                            {
                                mRK4110Pins.Pin4_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103双闪", "双闪脚短路或者断路");
                            }
                            if ((inputPin >> 4 & 0x1) == 1)//P档切换 => 巡航（RK7010）
                            {
                                mRK4110Pins.Pin12_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103 巡航", "巡航脚短路或者断路");
                            }
                            if ((inputPin >> 5 & 0x1) == 1)//Power档切换  => 左转灯
                            {
                                mRK4110Pins.Pin18_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103 左转灯", "左转灯脚短路或者断路");
                            }
                            if ((inputPin >> 6 & 0x1) == 1)//开关大灯 => PASS(RK7010)
                            {
                                mRK4110Pins.Pin14_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103 PASS", "PASS灯脚短路或者断路");
                            }
                            if ((inputPin >> 7 & 0x1) == 1)//自动大灯 => 远近光（RK7010）
                            {
                                mRK4110Pins.Pin13_Open = true;
                                InputPinError = true;
                                UpdateListView(sender, "4103 远近光", "远近光灯脚短路或者断路");
                            }
                            /*
                            if ((inputPin >> 8 & 0x1) == 1)
                            {
                                UpdateRK4110Items(sender, RK4110ITEM.GPIO26, null, INFO_LEVEL.FAIL);
                                InputPinError = true;
                            }
                            else
                                UpdateRK4110Items(sender, RK4110ITEM.GPIO26, null, INFO_LEVEL.PASS);
                            */
                            if(bCruised)
                            {
                                if ((inputPin >> 9 & 0x1) == 1)  //巡航开关 =》 空
                                {
                                    mRK4110Pins.Pin26_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 巡航开关", "4103巡航开关脚短路或者断路");
                                }                                    
                            }
                            if(bPushcar)
                            {
                                if ((inputPin >> 10 & 0x1) == 1) //推车开关  => 铁喇叭（RK7010）
                                {
                                    mRK4110Pins.Pin17_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 铁喇叭", "4103 铁喇叭脚短路或者断路");
                                }                                    
                            }                   
                            if(bBackcar)
                            {
                                if ((inputPin >> 11 & 0x1) == 1) //倒车开关 => 右转
                                {
                                    mRK4110Pins.Pin15_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 右转灯", "4103 右转脚短路或者断路");
                                }                                 
                            }
                            if(bRepaired)
                            {
                                if ((inputPin >> 12 & 0x1) == 1)//一键修复  =》 空
                                {
                                    mRK4110Pins.Pin19_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 一键修复", "4103 一键修复脚短路或者断路");
                                }
                                    
                            }                 
                        }
                        //output pin
                        if ((m_result >> 5 & 0x1) == 1)
                        {
                            /*
                            if ((outpin1 >> 2 & 0x1) == 1)
                            {
                                UpdateRK4110Items(sender, RK4110ITEM.GPIO27, null, INFO_LEVEL.FAIL);
                                InputPinError = true;
                            }
                            else
                                UpdateRK4110Items(sender, RK4110ITEM.GPIO27, null, INFO_LEVEL.PASS);
                            */
                            if ((outpin1 >> 3 & 0x1) == 1)
                            {
                                UpdateRK4110Items(sender, RK4110ITEM.PWM, null, INFO_LEVEL.FAIL);
                                InputPinError = true;
                                UpdateListView(sender, "4103 -PWM", "4103 PWM 短路或者断路");
                            }
                            else
                                UpdateRK4110Items(sender, RK4110ITEM.PWM, null, INFO_LEVEL.PASS);
                        }
                        //扩展pin脚
                        if ((m_result >> 8 & 0x1) == 1)
                        {
                            if(bLcm_L)
                            {
                                if ((extensionOut >> 0 & 0x1) == 1)//LCM_左转指示
                                {
                                    mRK4110Pins.Pin8_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM左转指示", "4103 LCM左转指示短路或者断路");
                                }
                            }
                            if(bLcm_F)
                            {
                                if ((extensionOut >> 1 & 0x1) == 1)//LCM_远光指示
                                {
                                    mRK4110Pins.Pin24_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM远光指示", "4103 LCM远光指示短路或者断路");
                                }
                            }
                            if(bLcm_R)
                            {
                                if ((extensionOut >> 2 & 0x1) == 1)//LCM_READY指示
                                {
                                    mRK4110Pins.Pin7_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM_READY指示", "4103 LCM_READY指示短路或者断路");
                                }
                            }
                            if(bLcm_P)
                            {
                                if ((extensionOut >> 3 & 0x1) == 1) //LCM_PARK指示
                                {
                                    mRK4110Pins.Pin23_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM_P档指示", "4103 LCM_P档指示短路或者断路");
                                }
                            }
                            if(bLcm_R)
                            {
                                if ((extensionOut >> 4 & 0x1) == 1)//LCM_右转指示
                                {
                                    mRK4110Pins.Pin25_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM_右转指示", "4103 LCM_右转指示短路或者断路");
                                }
                            }     
                            if(bLcm_CS)
                            {
                                if ((extensionOut >> 5 & 0x1) == 1)//LCM_CS
                                {
                                    mRK4110Pins.Pin21_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM_CS脚", "4103 LCM_CS短路或者断路");
                                }
                            }                       
                            if(bLcm_CLK)
                            {
                                if ((extensionOut >> 6 & 0x1) == 1)//LCM_CLK
                                {
                                    mRK4110Pins.Pin9_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM_CLK脚", "4103 LCM_CLK短路或者断路");
                                }
                            }
                            if(bLcm_DO)
                            {
                                if ((extensionOut >> 7 & 0x1) == 1)//LCM_DO
                                {
                                    mRK4110Pins.Pin10_Open = true;
                                    InputPinError = true;
                                    UpdateListView(sender, "4103 LCM_DO脚", "4103 LCM_DO短路或者断路");
                                }
                            }                            
                        }
                        //最后判断
                        if(!InputPinError)
                        {
                            UpdateRK4110Items(sender, RK4110ITEM.INIT, null, INFO_LEVEL.PASS);
                            UpdateRK4110Pins(sender, mRK4110Pins, INFO_LEVEL.PASS);
                            Thread.Sleep(5);
                            mSaveNvReqTask.Excute();
                        }
                        else
                        {
                            //更新RK4110的Pin脚图
                            UpdateRK4110Pins(sender, mRK4110Pins, INFO_LEVEL.FAIL);
                            SetMainText(sender, "测试失败!", "FAIL", INFO_LEVEL.FAIL);
                            StopTask();
                            return;
                        }                           
                    }                    
                }
                else
                {
                    SetMainText(sender, "未收到测试结果!", "FAIL", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试异常中断", "夹具被弹起，或485通讯失败");
                    StopTask();
                }
            };
#endregion

            #region 发送0xA3,接受0x24
            mSaveNvReqTask = new SimpleSerialPortTask<boardTestResultRsp, SaveNvReq>();
            mTestResultRspParam = mSaveNvReqTask.GetRequestEntity();
            mSaveNvReqTask.RetryMaxCnts = 0;
            mSaveNvReqTask.Timerout = 10 * 1000;
            mSaveNvReqTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<SaveNvReq> mEventArgs = e as SerialPortEventArgs<SaveNvReq>;
                if (mEventArgs.Data != null)
                {
                    mSaveNvRspParam.status = 0x00;                  
                    //启动蓝牙接受测试结果
                    SetMainText(sender, "蓝牙测试中...", "", INFO_LEVEL.PROCESS);
                    Thread.Sleep(5);
                    mBtTestReqTask.Excute();
                }
                else
                {
                    SetMainText(sender, "未收到写号请求!", "FAIL", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试异常中断", "夹具被弹起，或485通讯失败");
                    StopTask();
                }
            };
#endregion

            #region 发送0XA4，接受0x26
            mBtTestReqTask = new SimpleSerialPortTask<SaveNvRsp, btTestReq>();
            mSaveNvRspParam = mBtTestReqTask.GetRequestEntity();
            mBtTestReqTask.RetryMaxCnts = 0;
            mBtTestReqTask.Timerout = 30*1000;
            mBtTestReqTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<btTestReq> mEventArgs = e as SerialPortEventArgs<btTestReq>;
                if (mEventArgs.Data != null)
                {
                    bool bBleFlag = false;
                    /*
                    if (mEventArgs.Data.edrResult == 1)
                    {
                        bEdrFlag = true;
                        UpdateRK4110Items(sender, RK4110ITEM.EDR, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK4110Items(sender, RK4110ITEM.EDR, null, INFO_LEVEL.PASS);
                    */
                    if (mEventArgs.Data.bleResult == 1)
                    {
                        bBleFlag = true;
                        UpdateRK4110Items(sender, RK4110ITEM.BLE, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK4110Items(sender, RK4110ITEM.BLE, null, INFO_LEVEL.PASS);

                    //mCompeteTestTask.Excute();

                    if (bBleFlag)
                    {
                        SetMainText(sender, "蓝牙测试失败!", "FAIL", INFO_LEVEL.FAIL);
                        UpdateListView(sender, "蓝牙测试失败", "蓝牙信号被遮挡，或蓝牙硬件电路有问题");
                        StopTask();
                        return;
                    }                        
                    else
                    {                        
                        SetMainText(sender, "获取RK7010版本号...", "", INFO_LEVEL.PROCESS);
                        Thread.Sleep(5);
                        mGet7001VersionTask.Excute();
                    }          
                }
                else
                {
                    SetMainText(sender, "未收到蓝牙测试结果！", "FAIL", INFO_LEVEL.FAIL);
                    UpdateRK4110Items(sender, RK4110ITEM.BLE, null, INFO_LEVEL.FAIL);
                    UpdateListView(sender, "测试异常中断", "夹具被弹起，或485通讯失败");
                    StopTask();
                }
};
#endregion

            #region 测试蓝牙 回应
            mCompeteTestTask = new SimpleSerialPortTask<btTestRsp, NullEntity>();
            mBtTestRspParam = mCompeteTestTask.GetRequestEntity();
            mBtTestRspParam.status = 0;
            mCompeteTestTask.RetryMaxCnts = 0;            
#endregion
        }

        #region Step1：Client 自检检测  
        private Task_Level Step1_SelfTest(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mAskResult = (byte)mArgs.ack_value;//响应
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDeviceFault = (byte)mArgs.DeviceFault;
            byte mDcCurrent = (byte)mArgs.DcCurrent;
            byte mDcVoltage = (byte)mArgs.DcVoltage;
            byte mCutError1 = (byte)mArgs.CutError_1;
            byte mCutError2 = (byte)mArgs.CutError_2;
            byte mShortError1 = (byte)mArgs.ShortError_1;
            byte mShortError2 = (byte)mArgs.ShortError_2;
            byte mRemoteCmd = (byte)mArgs.RemoteCmd;
            bool testResult = false;

            if (mAskDevice == Const.PCU)
            {                
                if (mDeviceFault == 0 && mCutError1 == 0 && mCutError2 == 0 && mShortError1 == 0 && mShortError2 == 0)
                {
                    testResult = false;
                }
                else
                {
                    if ((mDeviceFault >> 1 & 0x1) == 1)//U200 运放故障
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.AMPLIFY, null, INFO_LEVEL.FAIL);
                        UpdateListView(sender, "7010运放故障", "U200或者运放有问题");
                    }
                    else
                    {
                        UpdateRK7001Items(sender, RK7001ITEM.AMPLIFY, null, INFO_LEVEL.PASS);
                    }
                    if ((mDeviceFault >> 2 & 0x1) == 1)//dc使能控制
                    {
                        testResult = true;
                        mRK7001Pins.Pin11_Open = true;
                        mRK7001Pins.Pin19_Open = true;
                        UpdateListView(sender, "7010 DC故障", "ACC_DC或者12V有问题");
                    }
                    
                    if ((mDeviceFault >> 5 & 0x1) == 1) //mosfet故障
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.MOSFET, null, INFO_LEVEL.FAIL);
                        UpdateListView(sender, "7010 Mosfet故障", "Mosfet可能短路");
                    }
                    else
                    {
                        UpdateRK7001Items(sender, RK7001ITEM.MOSFET, null, INFO_LEVEL.PASS);
                    }
                    if ((mCutError1 >> 0 & 0x1) == 1)//前左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin25_Open = true;
                        UpdateListView(sender, "7010前左转灯断路", "该pin脚可能断路或其他原因");
                    }
                    if ((mCutError1 >> 1 & 0x1) == 1)//后左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin28_Open = true;
                        UpdateListView(sender, "7010后左转灯断路", "该pin脚可能断路或其他原因");
                    }
                    if ((mCutError1 >> 2 & 0x1) == 1)//前右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin24_Open = true;
                        UpdateListView(sender, "7010前右转灯断路", "该pin脚可能断路或其他原因");
                    }
                    if ((mCutError1 >> 3 & 0x1) == 1)//后右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin27_Open = true;
                        UpdateListView(sender, "7010后右转灯断路", "该pin脚可能断路或其他原因");
                    }
                    if ((mCutError1 >> 4 & 0x1) == 1)//近光
                    {
                        testResult = true;
                        mRK7001Pins.Pin5_Open = true;
                        UpdateListView(sender, "7010近光灯断路", "该pin脚可能断路或其他原因");
                    }
                    if ((mCutError1 >> 5 & 0x1) == 1)//远光
                    {
                        testResult = true;
                        mRK7001Pins.Pin6_Open = true;
                        UpdateListView(sender, "7010远光灯断路", "该pin脚可能断路或其他原因");
                    }
                    if ((mCutError1 >> 6 & 0x1) == 1)//尾灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin2_Open = true;
                        UpdateListView(sender, "7010尾灯断路", "该pin脚可能断路或其他原因");
                    }

                    if ((mCutError1 >> 7 & 0x1) == 1)//刹车灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin3_Open = true;
                        UpdateListView(sender, "7010刹车灯断路", "该pin脚可能断路或其他原因");
                    }

                    if ((mCutError2 >> 0 & 0x1) == 1)//背景灯R
                    {
                        testResult = true;
                        mRK7001Pins.Pin23_Open = true;
                        UpdateListView(sender, "7010背景灯红断路", "该pin脚可能断路或其他原因");
                    }
                    /*
                    if ((mCutError2 >> 1 & 0x1) == 1)//背景灯G
                        mRK7001Pins.Pin7_Open = true;
                    if ((mCutError2 >> 2 & 0x1) == 1)//背景灯B
                        mRK7001Pins.Pin22_Open = true;
                    */
                    if ((mCutError2 >> 3 & 0x1) == 1)//喇叭
                    {
                        testResult = true;
                        mRK7001Pins.Pin4_Open = true;
                        UpdateListView(sender, "7010铁喇叭故障", "该pin脚短路或者断路");
                    }
                    /*
                    if ((mCutError2 >> 4 & 0x1) == 1)//BUZZER
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.PASS);
                    */
                    if ((mShortError1 >> 0 & 0x1) == 1)//前左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin25_Short = true;
                        UpdateListView(sender, "7010前左转灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError1 >> 1 & 0x1) == 1)//后左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin28_Short = true;
                        UpdateListView(sender, "7010后左转灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError1 >> 2 & 0x1) == 1)//前右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin24_Short = true;
                        UpdateListView(sender, "7010前右转灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError1 >> 3 & 0x1) == 1)//后右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin27_Short = true;
                        UpdateListView(sender, "7010后右转灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError1 >> 4 & 0x1) == 1)//近光
                    {
                        testResult = true;
                        mRK7001Pins.Pin5_Short = true;
                        UpdateListView(sender, "7010近光灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError1 >> 5 & 0x1) == 1)//远光
                    {
                        testResult = true;
                        mRK7001Pins.Pin6_Short = true;
                        UpdateListView(sender, "7010远光灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError1 >> 6 & 0x1) == 1)//尾灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin2_Short = true;
                        UpdateListView(sender, "7010尾灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError1 >> 7 & 0x1) == 1)//刹车灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin3_Short = true;
                        UpdateListView(sender, "7010刹车灯短路", "该pin脚短路或者其他原因");
                    }
                    if ((mShortError2 >> 0 & 0x1) == 1)//背景灯R
                    {
                        testResult = true;
                        mRK7001Pins.Pin23_Short = true;
                        UpdateListView(sender, "7010背景灯红短路", "该pin脚短路或者其他原因");
                    }
                    /*
                    if ((mShortError2 >> 1 & 0x1) == 1)//背景灯G
                        mRK7001Pins.Pin7_Short = true;
                    if ((mShortError2 >> 2 & 0x1) == 1)//背景灯B
                        mRK7001Pins.Pin22_Short = true;
                    */
                    if ((mShortError2 >> 3 & 0x1) == 1)//喇叭
                    {
                        testResult = true;
                        mRK7001Pins.Pin4_Open = true;
                        UpdateListView(sender, "7010铁喇叭短路", "该pin脚短路或者其他原因");
                    }
                    /*
                    if ((mShortError2 >> 4 & 0x1) == 1)//BUZZER
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.PASS);
                    */                    
                }

                if ((mDcCurrent * 100 < 50) || (mDcCurrent * 100 > 150))//判断DC输出电流
                {
                    testResult = true;
                    mRK7001Pins.Pin4_Open = true;
                    UpdateListView(sender, "7010铁喇叭故障", "采样电流异常");
                    Console.WriteLine("mDcCurrent={0}", mDcCurrent * 100);
                }
                else
                {
                    Console.WriteLine("mDcCurrent={0}", mDcCurrent * 100);
                }

                if (!testResult)
                {                      
                    //UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.PASS);
                    return Task_Level.TRUE;
                }
                else
                {
                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.FAIL);
                    SetMainText(sender, "测试失败!", "FAIL", INFO_LEVEL.FAIL);
                    mGet7001ResultTask.ClearAllEvent();
                    return Task_Level.FALSE;
                }
            }
            
            return Task_Level.REPEAT;
        }
        #endregion

        #region Step2: 检查ACC_ECU
        private Task_Level Step2_CheckEcuOpen(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            byte mDeviceFault = (byte)mArgs.DeviceFault;
            if (mAckDevice == Const.TESTSERVER)
            {
                bool bFlag = false;                                         
                if (mAccStatus == 0)
                {
                    bFlag = true;
                    mRK7001Pins.Pin16_Open = true;
                    UpdateListView(sender, "ACC_ECU错误", "ACC_ECU的pin脚短路或者断路或者其他原因");
                }
                if (mDcOUT == 0)
                {
                    bFlag = true;
                    mRK7001Pins.Pin16_Open = true;
                    UpdateListView(sender, "ACC_ECU错误", "ACC_ECU的pin脚短路或者断路或者其他原因");
                }
                if((mDeviceFault >> 6 & 0x1) == 1)//ACC_CCU已经开启
                {
                    bFlag = true;
                    UpdateVddGpsPin(sender, INFO_LEVEL.FAIL);
                    UpdateListView(sender, "ACC_CCU错误", "ACC_CCU的pin脚短路或者断路或者其他原因");
                }
                /*
                else
                {
                    UpdateVddGpsPin(sender, INFO_LEVEL.PASS);
                }
                */
                if (!bFlag)
                {
                    return Task_Level.TRUE;
                }
                else
                {
                    return Task_Level.FALSE;
                }
            }

            return Task_Level.REPEAT;
        }
        #endregion

        #region Step3:软关DC
        private Task_Level Step3_SoftCloseEcu(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            if (mAckDevice == Const.PCU)
            {
                mTaskSteps = TaskSteps.Step4_CheckEcuClose;
                return Task_Level.TRUE;
            }

            return Task_Level.FALSE;
        }
        #endregion

        #region Step4:检查ACC_ECU是否关闭
        private Task_Level Step4_CheckEcuClose(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            if (mAckDevice == Const.TESTSERVER)
            {
                bool bFlag = false;
                
                if (mAccStatus != 0)
                {
                    bFlag = true;
                    mRK7001Pins.Pin16_Open = true;
                    UpdateListView(sender, "ACC_ECU错误", "ACC_ECU的pin脚短路或者断路或者其他原因");
                }
                if (mDcOUT != 0)
                {
                    bFlag = true;
                    mRK7001Pins.Pin16_Open = true;
                    UpdateListView(sender, "ACC_ECU错误", "ACC_ECU的pin脚短路或者断路或者其他原因");
                }
                
                if (!bFlag)
                {
                    return Task_Level.TRUE;
                }
                else
                {
                    return Task_Level.FALSE;
                }
            }

            return Task_Level.REPEAT;
        }
        #endregion

        #region Step5:硬开KSI
        private Task_Level Step5_OpenKSI(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            if (mAckDevice == Const.TESTSERVER)
            {                                         
                if (mAccStatus == 0)//V2.0.0 拆掉D416,所以DCout不测
                {
                    mRK7001Pins.Pin18_Open = true;
                    UpdateListView(sender, "KSI错误", "KSI的pin脚短路或者断路或者其他原因");
                    return Task_Level.FALSE;
                }             
                return Task_Level.TRUE;
            }

            return Task_Level.REPEAT;
        }
        #endregion

        #region Step6:关闭KSI
        private Task_Level Step6_CloseKSI(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            if (mAckDevice == Const.TESTSERVER)
            {                            
                if (mAccStatus != 0)//V2.0.0 拆掉D416,所以DCout不测
                {
                    if (ReTryCnts++ >= 2)
                    {
                        mRK7001Pins.Pin18_Open = true;
                        UpdateListView(sender, "KSI错误", "KSI的pin脚短路或者断路或者其他原因");
                        return Task_Level.FALSE;
                    }
                    else
                    {
                        mTaskSteps = TaskSteps.Step6_CloseKSI;
                        mGet700ResultParam.ack_device = Const.TESTSERVER;//发给SERVER
                        mGet700ResultParam.ecu_status = 0x20;
                        mGet700ResultParam.server_mode = 0x0;
                        mGet700ResultParam.level_ctrl = 0x0100;
                        return Task_Level.REPEAT;
                    }
                }
                
                return Task_Level.TRUE;
            }
            return Task_Level.REPEAT;
        }
        #endregion

        #region Step9: Server 通知server，按下遥控器按键
        private Task_Level Step7_RemoteInit(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            if(mAckDevice == Const.TESTSERVER)
            {
                if(mAckValue == Const.TESTSERVER_KEY_PRESS)
                {                   
                    return Task_Level.TRUE;
                }                        
            }
            else if(mAckDevice == Const.PCU)
            {
                return Task_Level.REPEAT;
            }

            return Task_Level.FALSE;
        }
        #endregion

        #region Step10: Client 通知client，进行配对
        private Task_Level Step8_RemoteTest(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            if (mAckDevice == Const.PCU)
            {
                if (mAckValue == Const.REMOTE_CONNECTED)
                {
                    return Task_Level.TRUE;
                }
                else if (mAckValue == Const.REMOTE_CHECK_FAIL)
                {
                    UpdateListView(sender, "遥控电路测试失败", "遥控电路有问题");
                    UpdateRemoteStatus(sender, INFO_LEVEL.FAIL);
                    return Task_Level.FALSE;
                }
                else if (mAckValue == Const.REMOTE_NO_RECV)
                {
                    UpdateListView(sender, "遥控电路测试失败", "遥控电路有问题");
                    UpdateRemoteStatus(sender, INFO_LEVEL.FAIL);
                    return Task_Level.FALSE;
                }
                else
                    return Task_Level.REPEAT;
            }
            else if (mAckDevice == Const.TESTSERVER)
                return Task_Level.REPEAT;

            return Task_Level.FALSE;
        }
        #endregion

        #region Step12: 关闭VDD_CCU
        private Task_Level Step12_CloseVddCCu(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            if (mAckDevice == Const.PCU)
            {
                if((mAccStatus >> 1 & 0x1) == 0x1)
                {
                    return Task_Level.TRUE;
                }
                else if((mAccStatus >> 1 & 0x1) == 0x0)
                {
                    return Task_Level.FALSE;
                }
            }
            
            return Task_Level.REPEAT;
        }
        #endregion

        #region Step13: 查询VDD_CCU状态
        private Task_Level Step13_CheckVddCCu(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDeviceFault = (byte)mArgs.DeviceFault;
            if (mAckDevice == Const.TESTSERVER)
            {                
                if ((mDeviceFault >> 6 & 0x1) == 0)
                {
                    UpdateVddGpsPin(sender, INFO_LEVEL.FAIL);
                    UpdateListView(sender, "ACC_CCU错误", "ACC_CCU的pin脚短路或者断路或者其他原因");
                    return Task_Level.FALSE;
                }                
                else if((mDeviceFault >> 6 & 0x1) == 1)
                {
                    UpdateVddGpsPin(sender, INFO_LEVEL.PASS);
                    return Task_Level.TRUE;
                }
            }
            return Task_Level.REPEAT;
        }
        #endregion

        #region RK7001目检失败
        public void RK7001KSICheckFail()
        {
            mGet7001ResultTask.ClearAllEvent();
            SetMainText(this, "RK7010目检失败!", "FAIL", INFO_LEVEL.FAIL);
        }
        #endregion

        #region 遥控器
        private void UpdateRemoteStatus(object sender, INFO_LEVEL level)
        {
            if(UpdateRemoteHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                UpdateRemoteHandler(sender, mArgs);
            }
        }
        #endregion

        #region 检测每个灯的采样电流
        private Task_Level CheckSampleCurrent(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mDcCurrent = (byte)mArgs.DcCurrent;            
            if (mAskDevice == Const.PCU)
            {
                if (mDcCurrent * 100 > 100)//判断DC输出电流 d
                {                    
                    Console.WriteLine("mDcCurrent={0}", mDcCurrent * 100);
                    return Task_Level.FALSE;
                }
                else
                {
                    Console.WriteLine("mDcCurrent={0}", mDcCurrent * 100);
                    return Task_Level.TRUE;
                }
            }

            return Task_Level.FALSE;
        }
        #endregion

        #region 检测左转、右转，前左转，前右转
        private Task_Level getLeftRightLightCurrent(object sender, get7001ResultRsp mArgs)
        {
            byte mAskDevice = (byte)mArgs.ack_device;//应答设备
            byte mDcCurrent = (byte)mArgs.DcCurrent;
            if(mAskDevice == Const.PCU)
            {
                if (mDcCurrent * 100 > 200)//判断DC输出电流
                {
                    Console.WriteLine("mDcCurrent={0}", mDcCurrent * 100);
                    return Task_Level.FALSE;
                }
                else
                {
                    Console.WriteLine("mDcCurrent={0}", mDcCurrent * 100);
                    return Task_Level.TRUE;
                }
            }

            return Task_Level.FALSE;
        }
        #endregion

        #region 通过SN，获取RK4110的数据
        private bool GetValueFrmServer(object sender)
            {
                DbHelperSQL.connectionString = mSqlserverConnString;
                string selectString = String.Format("select * from [PreDistribtNum] where SN='{0}'", mSN);
                string selectCount = String.Format("select count(*) from [PreDistribtNum] where SN='{0}'", mSN);            
                int cnt = (int)DbHelperSQL.GetSingle(selectCount);
                if (cnt > 0)
                {
                    DataSet ds = DbHelperSQL.Query(selectString);
                    mBTaddr = ds.Tables[0].Rows[0][2].ToString();
                    mBLEaddr = ds.Tables[0].Rows[0][3].ToString();
                    mKeyt = ds.Tables[0].Rows[0][4].ToString();
                    string mUsdFlag = ds.Tables[0].Rows[0][5].ToString();
                    if(mUsdFlag == "TRUE")
                    {
                        UpdateValidSN(sender, INFO_LEVEL.FAIL);
                        SetMainText(sender, "该SN号已经被使用过！", "SAVENONE", INFO_LEVEL.FAIL);
                        UpdateListView(sender, "该SN号已经被使用过", "该SN号以前被成功测试过！");
                        return false;
                    }
                    else
                    {
                        UpdateValidSN(sender, INFO_LEVEL.PASS);
                        SetMainText(sender, "测试Server是否在线？", "", INFO_LEVEL.PROCESS);
                        mChk4103ServerTask.Excute();
                    }                
                }
                else
                {
                    UpdateValidSN(sender, INFO_LEVEL.FAIL);
                    SetMainText(sender, "SN号在服务器不存在！", "SAVENONE", INFO_LEVEL.FAIL);
                    UpdateListView(sender, "SN号在服务器不存在", "服务器未开启，或SN号在服务器不存在！");
                    return false;
                }

                return true;
            }
            #endregion

        #region 保存数据
        private void SaveData2Server(INFO_LEVEL level)
        {
            DbHelperSQL.connectionString = mSqlserverConnString;
            string mGenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string insertString = String.Format("insert into [RK7001_Complex] (SN,BTaddr,BLEaddr,Keyt,UID,TestResult,GenTime) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                        mSN, mBTaddr, mBLEaddr, mKeyt, mUid, mTestResult, mGenTime);
            string queryString = String.Format("select count(*) from [RK7001_Complex] where SN='{0}'", mSN);
            string updateString = String.Format("update [RK7001_Complex] set BTaddr='{0}',BLEaddr='{1}',Keyt='{2}',UID='{3}',TestResult='{4}',GenTime='{5}' where SN='{6}'",
                        mBTaddr, mBLEaddr, mKeyt, mUid, mTestResult, mGenTime, mSN);
            string updateUsdFlag = "";
            switch(level)
            {
                case INFO_LEVEL.FAIL:
                    updateUsdFlag = String.Format("update [PredistribtNum] set UsdFlag='FALSE' where SN='{0}'", mSN);
                    break;
                case INFO_LEVEL.PASS:
                    updateUsdFlag = String.Format("update [PredistribtNum] set UsdFlag='TRUE' where SN='{0}'", mSN);
                    break;
            }
            
            if ((int)DbHelperSQL.GetSingle(queryString) > 0)
            {
                DbHelperSQL.ExecuteSql(updateString);
            }
            else
            {
                DbHelperSQL.ExecuteSql(insertString);
            }
            //更新
            DbHelperSQL.ExecuteSql(updateUsdFlag);
        }       
        #endregion

        #region 设置界面
        private void SetMainText(object sender, string msg, string submsg, INFO_LEVEL level)
        {
            if(level == INFO_LEVEL.FAIL || level == INFO_LEVEL.PASS)
            {
                bTaskRunning = false;
                if (mode == FACTORY_MODE.TEST_MODE)
                {
                    if (bServerActivated)
                    {
                        if (submsg != "SAVENONE")
                        {
                            mTestResult = submsg;
                            SaveData2Server(level);
                        }
                    }
                }                                       
            }
            if(UpdateWorkStatusHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.msg = msg;
                mArgs.submsg = submsg;
                mArgs.level = level;
                UpdateWorkStatusHandler(sender, mArgs);
            }
        }
        #endregion

        #region 刷新RK7001列表
        private void UpdateRK7001Items(object sender, RK7001ITEM items, DeviceInfo info, INFO_LEVEL level)
        {
            if(Update7001ListViewHandler != null)
            {
                RK7001ItemsArgs mArgs = new RK7001ItemsArgs();
                mArgs.items = items;
                mArgs.info = info;
                mArgs.level = level;
                Update7001ListViewHandler(sender, mArgs);
            }
        }
        #endregion

        #region 刷新RK4110列表
        private void UpdateRK4110Items(object sender, RK4110ITEM items, DeviceInfo info, INFO_LEVEL level)
        {
            if(Update4103ListViewHandler != null)
            {
                RK4110ItemsArgs mArgs = new RK4110ItemsArgs();
                mArgs.items = items;
                mArgs.info = info;
                mArgs.level = level;
                Update4103ListViewHandler(sender, mArgs);
            }
        }
        #endregion

        #region 刷新7001的PIN脚
            private void UpdateRK7001Pins(object sender, PIN_STATUS pins, INFO_LEVEL level)
            {
                if(Update7001PinListHandler != null)
                {
                    PinStatusArgs mArgs = new PinStatusArgs();
                    mArgs.status = pins;
                    mArgs.level = level;
                    Update7001PinListHandler(sender, mArgs);
                }
            }
        #endregion

        #region 刷新RK4110的PIN脚
            private void UpdateRK4110Pins(object sender, PIN_STATUS pins, INFO_LEVEL level)
            {
                if(Update4103PinListHandler != null)
                {
                    PinStatusArgs mArgs = new PinStatusArgs();
                    mArgs.status = pins;
                    mArgs.level = level;
                    Update4103PinListHandler(sender, mArgs);
                }
            }
            #endregion

        #region 刷新VDD_GPS
        private void UpdateVddGpsPin(object sender, INFO_LEVEL level)
        {
            if(UpdateVddGpsHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                UpdateVddGpsHandler(sender, mArgs);
            }
        }
        #endregion

        #region 刷新SN号的合法性
        private void UpdateValidSN(object sender, INFO_LEVEL level)
        {
            if(UpdateValidSNHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                UpdateValidSNHandler(sender, mArgs);
            }
        }
        #endregion

        #region 动态显示
        private void UpdatePotTicker(object sender, string msg, INFO_LEVEL level)
        {
            if(UpdatePotTickerHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                mArgs.msg = msg;
                UpdatePotTickerHandler(sender, mArgs);
            }
        }
        #endregion

        #region 刷新TestServer状态
        private void UpdateTestServer(object sender, INFO_LEVEL level)
        {
            if(UpdateChkServerHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = level;
                UpdateChkServerHandler(sender, mArgs);
            }
        }
        #endregion

        #region 刷新提示列表
        private void UpdateListView(object sender, string msg, string submsg)
        {
            if(UpdateListViewHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.msg = msg;
                mArgs.submsg = submsg;
                UpdateListViewHandler(sender, mArgs);
            }
        }
        #endregion

        #region 初始化配置参数
        private void InitParameters()
        {
            if(mGet700ResultParam != null)
            {
                mGet700ResultParam.ack_device = Const.PCU;//发给PCU
                mGet700ResultParam.ecu_status = 0x34;//默认;//默认
                mGet700ResultParam.server_mode = 0x4;//开启软件上电
                mGet700ResultParam.backlight = 0;
                mGet700ResultParam.batt_soc = 0;
                mGet700ResultParam.level_ctrl = 0x0100;
                mGet700ResultParam.limit_per = 0;
                mGet700ResultParam.trigger_ctrl = 0;
            }
            bTaskRunning = true;
            mBTaddr = "";
            mBLEaddr = "";
            mKeyt = "";
            mUid = "";
            mTestResult = "";
            ReTryCnts = 0;
            PotTickCnt = 0;
            //获取RK7001的版本号
            mGet7001VersionTask.ClearAllEvent();
            mGet7001VersionTask.EnableTimeOutHandler = true;
            //获取RK7001的测试结果
            mGet7001ResultTask.ClearAllEvent();
            mGet7001ResultTask.EnableTimeOutHandler = true;

            mChk4103ServerTask.ClearAllEvent();
            mChk4103ServerTask.EnableTimeOutHandler = true;
            //接受0x06
            mFirstRunningReqTask.ClearAllEvent(); 
            mFirstRunningReqTask.EnableTimeOutHandler = true;
            //发送0x86，接受0x03
            mParamSettingReqTask.ClearAllEvent();
            mParamSettingReqTask.EnableTimeOutHandler = true;

            //发送0x83， 接受0x23
            mRecvTestResultTask.ClearAllEvent();
            mRecvTestResultTask.EnableTimeOutHandler = true;
            //发送0xA3，接受0x24
            mSaveNvReqTask.ClearAllEvent();
            mSaveNvReqTask.EnableTimeOutHandler = true;
            //发送0xA4,接受0x26
            mBtTestReqTask.ClearAllEvent();
            mBtTestReqTask.EnableTimeOutHandler = true;
            //发送0xA6
            mCompeteTestTask.ClearAllEvent();
            mCompeteTestTask.EnableTimeOutHandler = true;

            mGetDevinfoTask.ClearAllEvent();
            mGetDevinfoTask.EnableTimeOutHandler = true;


        }
        #endregion

        #region 停止Task
        private void StopTask()
        {
            this.DynamicPotTicker.Enabled = false;
            //获取RK7001的版本号
            mGet7001VersionTask.ClearAllEvent();
            mGet7001VersionTask.EnableTimeOutHandler = false;
            //获取RK7001的测试结果
            mGet7001ResultTask.ClearAllEvent();
            mGet7001ResultTask.EnableTimeOutHandler = false;

            mChk4103ServerTask.ClearAllEvent();
            mChk4103ServerTask.EnableTimeOutHandler = false;
            //接受0x06
            mFirstRunningReqTask.ClearAllEvent();
            mFirstRunningReqTask.EnableTimeOutHandler = false;
            //发送0x86，接受0x03
            mParamSettingReqTask.ClearAllEvent();
            mParamSettingReqTask.EnableTimeOutHandler = false;

            //发送0x83， 接受0x23
            mRecvTestResultTask.ClearAllEvent();
            mRecvTestResultTask.EnableTimeOutHandler = false;
            //发送0xA3，接受0x24
            mSaveNvReqTask.ClearAllEvent();
            mSaveNvReqTask.EnableTimeOutHandler = false;
            //发送0xA4,接受0x26
            mBtTestReqTask.ClearAllEvent();
            mBtTestReqTask.EnableTimeOutHandler = false;
            //发送0xA6
            mCompeteTestTask.ClearAllEvent();
            mCompeteTestTask.EnableTimeOutHandler = false;

            mGetDevinfoTask.ClearAllEvent();
            mGetDevinfoTask.EnableTimeOutHandler = false;

            UpdatePotTicker(this, "", INFO_LEVEL.INIT);
        }
        #endregion


        #region 执行任务
        public void ExcuteTask()
        {
            Log.Info("开始测试......");
            InitParameters();
            mRK7001Pins = new PIN_STATUS();
            mRK4110Pins = new PIN_STATUS();
            UpdateRK7001Pins(this, mRK7001Pins, INFO_LEVEL.PROCESS);
            UpdateRK4110Pins(this, mRK4110Pins, INFO_LEVEL.PROCESS);
            UpdateRemoteStatus(this, INFO_LEVEL.PROCESS);
            UpdateVddGpsPin(this, INFO_LEVEL.PROCESS);
            this.DynamicPotTicker.Enabled = true;

            //mGet7001ResultTask.Excute();
            if(mode == FACTORY_MODE.TEST_MODE)
            {
                if (bServerActivated)
                {
                    if (!GetValueFrmServer(this))
                    {
                        StopTask();
                        return;
                    }
                }
                else
                {
                    mSN = DefaultSN;
                    mBTaddr = DefaultBT;
                    mBLEaddr = DefaultBLE;
                    mKeyt = DefaultKeyt;
                    UpdateValidSN(this, INFO_LEVEL.PASS);
                    SetMainText(this, "测试Server是否在线？...", "", INFO_LEVEL.PROCESS);
                    mChk4103ServerTask.Excute();
                }
            }
            else if(mode == FACTORY_MODE.CHECK_MODE)//如果是复检模式,SN号仍使
            {                
                SetMainText(this, "测试Server是否在线？...", "", INFO_LEVEL.PROCESS);
                mChk4103ServerTask.Excute();
            }                  
        }
        #endregion
    }
}
