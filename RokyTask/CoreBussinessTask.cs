﻿using CommonUtils;
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
        ONLY_TIP = 5
    }

    public enum RK7001ITEM
    {
        NONE = 0,
        INIT = 1,
        VERSION = 2,
        DC = 3,
        AMPLIFY = 4,
        MOSFET = 5,
        BUZZER = 6
    }

    public enum RK4103ITEM
    {
        NONE = 0,
        INIT = 1,
        VERSION = 2,
        LIGHTSENSOR = 3,
        GSENSOR = 4,
        GPIO26 = 5,
        GPIO27 = 6,
        PWM = 7,
        EDR = 8,
        BLE = 9,
        VDD33 = 10
    }

    public enum TASK_TYPE
    {
        NONE = 0,
        CHK_TEST_SERVER = 1,
        RK7001_CHKVERSION = 2,
        RK7001_SELFTEST = 3,
        RK4103_POWERON = 4,
        RK4103_SELFTEST = 5,
        RK4103_PARAMSETTING = 6,
        RK4103_WRITENV = 7,
        RK4103_BLETEST = 8,
    }

    public class DeviceInfo
    {
        public string sn { get; set; }
        public string sw { get; set; }
        public string hw { get; set; }
    }

    public class UIEventArgs : EventArgs
    {
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

    public class RK4103ItemsArgs : EventArgs
    {
        public DeviceInfo info { get; set; }
        public RK4103ITEM items { get; set; }
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


    public enum RK7001Steps
    {
        Step0_Init_1 = 0,
        Step1_SelfCheck = 1,
        Step2_Init_2 = 2,
        Step3_OpenKSI = 3,
        Step4_CloseKSI = 4,
        Step5_SoftOpenEcu = 5,
        Step6_ReadOpenEcu = 6,
        Step7_SoftCloseEcu = 7,
        Step8_ReadCloseEcu = 8,
        Step9_RemoteInit = 9,
        Step10_RemoteTest = 10,
    }

    public enum Level
    {
        FALSE = 0,
        TRUE = 1,
        REPEAT = 2,
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

        private int nPotTicker { get; set; }

        private TASK_TYPE mTaskType { get; set; }

        private RK7001ResultType mRK7001ResultType { get; set; }

        private RK7001Steps mStep;

        private PIN_STATUS mRK7001Pins;
        private PIN_STATUS mRK4103Pins;

        public bool bCruised { get; set; }
        public bool bRepaired { get; set; }
        public bool bPushcar { get; set; }
        public bool bBackcar { get; set; }
        public bool bTest4103 { get; set; }

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
        
        //接受0x06
        SimpleSerialPortTask<NullEntity, get4103BroadcastReq> mFirstRunningReqTask; //
        //发送0x86，接受0x03
        SimpleSerialPortTask<get4103BroadcastRsp, ParameterSettingReq> mParamSettingReqTask; //
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

        //发送0x06 0x86
        SimpleSerialPortTask<pcTakeOverRsp, NullEntity> mPCTaskOverRspTask;
        pcTakeOverRsp mTakeOverParam;

        //发送0x0A，0x8A
        SimpleSerialPortTask<ResetEcuReq, ResetEcuRsp> mResetEcuTask;
        ResetEcuReq mResetEcuReqParam;

        System.Timers.Timer DynamicPotTicker;
        #endregion

        #region 构造函数
        public CoreBussinessTask()
        {            
            TaskBuilder();
        }
        #endregion

        #region 构造任务
        private void TaskBuilder()
        {
            #region 重启ECU
            mResetEcuTask = new SimpleSerialPortTask<ResetEcuReq, ResetEcuRsp>();
            mResetEcuReqParam = mResetEcuTask.GetRequestEntity();
            mResetEcuTask.RetryMaxCnts = 2;
            mResetEcuTask.Timerout = 2000;
            mResetEcuReqParam.deviceType = 0xF1;
            mResetEcuReqParam.cmdCode = 0x02; //回复ECU同步命令
            mResetEcuTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<ResetEcuRsp> mEventArgs = e as SerialPortEventArgs<ResetEcuRsp>;
                if (mEventArgs.Data != null)
                {
                    if(mEventArgs.Data.RspType == 0)
                    {
                        bTest4103 = true;
                        StartDynamicPotTicker(30, TASK_TYPE.RK4103_PARAMSETTING);
                        mFirstRunRspParam.deviceType = 0x01;
                        mFirstRunRspParam.firmwareYears = 0;
                        mFirstRunRspParam.firmwareWeeks = 0;
                        mFirstRunRspParam.firmwareVersion = 0;
                        mFirstRunRspParam.hardwareID = 0xFB;
                        mParamSettingReqTask.Excute();
                    }                    
                }
            };            
            #endregion

            #region PC接管报文 回应
            mPCTaskOverRspTask = new SimpleSerialPortTask<pcTakeOverRsp, NullEntity>();
            mPCTaskOverRspTask.RetryMaxCnts = 0;
            mPCTaskOverRspTask.Timerout = 1000;
            mTakeOverParam = mPCTaskOverRspTask.GetRequestEntity();
            mTakeOverParam.deviceType = 0xF1;
            mTakeOverParam.hardVare = 0xF1;
            #endregion

            #region 自检报文
            mGet7001ResultTask = new SimpleSerialPortTask<get7001Result, get7001ResultRsp>();
            mGet700ResultParam = mGet7001ResultTask.GetRequestEntity();
            mGet7001ResultTask.RetryMaxCnts = 30;
            mGet7001ResultTask.Timerout = 1000;//10s超时
            mGet700ResultParam.ack_device = Const.PCU;//发给PCU
            mGet700ResultParam.ecu_status = 0x14;//默认;//默认
            mGet700ResultParam.server_mode = 0x04;//开启软件上电
            mGet700ResultParam.backlight = 0;
            mGet700ResultParam.batt_soc = 0;
            mGet700ResultParam.level_ctrl = 0 | 0x01 << 8;
            mGet700ResultParam.limit_per = 0;
            mGet700ResultParam.trigger_ctrl = 0;            
            mGet7001ResultTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get7001ResultRsp> mEventArgs = e as SerialPortEventArgs<get7001ResultRsp>;
                if (mEventArgs.Data != null)
                {
                    bool bExcute = false;
                    Level level = Level.FALSE;
                    switch (mStep)
                    {
                        case RK7001Steps.Step0_Init_1:
                            level = Step0_OpenSoftDC(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step1_SelfCheck;
                                    mGet700ResultParam.ack_device = Const.PCU;
                                    mGet700ResultParam.ecu_status = 0x14;//默认;//默认
                                    mGet700ResultParam.server_mode = 0x4;//开启软件上电
                                    mGet700ResultParam.level_ctrl = 0 | 0x01 << 8;
                                    bExcute = true;
                                    break;
                            }                                                         
                            break;
                        case RK7001Steps.Step1_SelfCheck:
                            level = Step1_SelfCheck(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    SetMainText(sender, "测试失败！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step2_Init_2;
                                    mGet700ResultParam.ack_device = Const.PCU;//自测结果请求
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x0;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                                                            
                            break;
                        case RK7001Steps.Step2_Init_2:
                            level = Step2_Init_2(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:                                    
                                    mStep = RK7001Steps.Step3_OpenKSI;
                                    mGet700ResultParam.ack_device = Const.TESTSERVER;//自测结果请求
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x2;
                                    mGet700ResultParam.level_ctrl = 0;                                    
                                    bExcute = true;
                                    break;
                            }                            
                            break;
                        case RK7001Steps.Step3_OpenKSI:
                            level = Step3_OpenKSI(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    mRK7001Pins.Pin14_Open = true;
                                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.FAIL);
                                    SetMainText(sender, "测试失败！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step4_CloseKSI;
                                    mGet700ResultParam.ack_device = Const.TESTSERVER;//自测结果请求
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x0;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                           
                            break;
                        case RK7001Steps.Step4_CloseKSI:
                            level = Step4_CloseKSI(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    mRK7001Pins.Pin14_Open = true;
                                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.FAIL);
                                    SetMainText(sender, "测试失败！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step5_SoftOpenEcu;
                                    mGet700ResultParam.ack_device = Const.PCU;
                                    mGet700ResultParam.ecu_status = 0x14;
                                    mGet700ResultParam.server_mode = 0x4;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                            
                            break;
                        case RK7001Steps.Step5_SoftOpenEcu:
                            level = Step5_SoftOpenEcu(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    SetMainText(sender, "软件打开ACC_ECU失败！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step6_ReadOpenEcu;
                                    mGet700ResultParam.ack_device = Const.TESTSERVER;
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x4;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                            
                            break;
                        case RK7001Steps.Step6_ReadOpenEcu:
                            level = Step6_ReadOpenEcu(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    mRK7001Pins.Pin16_Open = true;
                                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.FAIL);
                                    SetMainText(sender, "测试失败", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step7_SoftCloseEcu;
                                    mGet700ResultParam.ack_device = Const.PCU;
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x0;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                                                           
                            break;
                        case RK7001Steps.Step7_SoftCloseEcu:
                            level = Step7_SoftCloseEcu(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    SetMainText(sender, "软件关闭ACC_ECU失败！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step8_ReadCloseEcu;
                                    mGet700ResultParam.ack_device = Const.TESTSERVER;
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x0;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                                                            
                            break;
                        case RK7001Steps.Step8_ReadCloseEcu:
                            level = Step8_ReadCloseEcu(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    mRK7001Pins.Pin16_Open = true;
                                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.FAIL);
                                    SetMainText(sender, "测试失败！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step9_RemoteInit;
                                    mGet700ResultParam.ack_device = Const.TESTSERVER;
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x1;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                            
                            break;
                        case RK7001Steps.Step9_RemoteInit:
                            level = Step9_RemoteInit(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    SetMainText(sender, "测试Server遥控器未按下键！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    mStep = RK7001Steps.Step10_RemoteTest;
                                    mGet700ResultParam.ack_device = Const.PCU;
                                    mGet700ResultParam.ecu_status = 0x0;
                                    mGet700ResultParam.server_mode = 0x1;
                                    mGet700ResultParam.level_ctrl = 0;
                                    bExcute = true;
                                    break;
                            }                            
                            break;
                        case RK7001Steps.Step10_RemoteTest:
                            level = Step10_RemoteTest(sender, mEventArgs.Data);
                            switch(level)
                            {
                                case Level.FALSE:
                                    UpdateRemoteStatus(sender, INFO_LEVEL.FAIL);
                                    SetMainText(sender, "遥控器测试失败！", "", INFO_LEVEL.FAIL);
                                    mGet7001ResultTask.ClearAllEvent();
                                    break;
                                case Level.REPEAT:
                                    bExcute = true;
                                    break;
                                case Level.TRUE:
                                    UpdateRemoteStatus(sender, INFO_LEVEL.PASS);
                                    mResetEcuTask.Excute();                                    
                                    bExcute = false;
                                    break;
                            }                            
                            break;                                                                      
                    }

                    if(bExcute)
                    {
                        Thread.Sleep(500);
                        mGet7001ResultTask.Excute();
                    }                                                          
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "自检超时！", "",INFO_LEVEL.FAIL);
                    mGet7001ResultTask.ClearAllEvent();
                }                
            };
            #endregion

            #region 动态显示
            DynamicPotTicker = new System.Timers.Timer(1000);
            DynamicPotTicker.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs ElapsedEventArgs) =>
            {
                nPotTicker--;
                if (nPotTicker <= 0)
                    nPotTicker = 0;
                switch(mTaskType)
                {
                    case TASK_TYPE.CHK_TEST_SERVER:
                        SetMainText(sender, "测试Server是否在线?", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                    case TASK_TYPE.RK7001_CHKVERSION:
                        SetMainText(sender, "等待设备上电中...", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                    case TASK_TYPE.RK7001_SELFTEST:
                        SetMainText(sender, "RK7001自检中...", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                    case TASK_TYPE.RK4103_POWERON:
                        SetMainText(sender, "等待4103设备上电...", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                    case TASK_TYPE.RK4103_SELFTEST:
                        SetMainText(sender, "RK4103测试中...", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                    case TASK_TYPE.RK4103_PARAMSETTING:
                        SetMainText(sender, "等待4103参数配置请求...", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                    case TASK_TYPE.RK4103_WRITENV:
                        SetMainText(sender, "等待4103写NV请求...", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                    case TASK_TYPE.RK4103_BLETEST:
                        SetMainText(sender, "蓝牙测试中...", nPotTicker.ToString(), INFO_LEVEL.PROCESS);
                        break;
                }
            });
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
                    info.hw = String.Format("V{0}.{1}.{2}", hwArray[1], hwArray[2], hwArray[3]);
                    info.sw = String.Format("W{0}{1}.0{2}", swArray[1], swArray[2], swArray[3]);
                    UpdateRK7001Items(sender, RK7001ITEM.VERSION, info, INFO_LEVEL.NONE);
                    if (j >= 10)
                    {
                        StopDynamicPotTicker(sender);
                        SetMainText(sender, "RK7001未上电", "请下压夹具，再测一次", INFO_LEVEL.FAIL);
                        return;
                    }
                    //开始执行查询
                    mStep = RK7001Steps.Step0_Init_1;
                    mGet7001ResultTask.Excute();
                    StartDynamicPotTicker(30, TASK_TYPE.RK7001_SELFTEST);
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "RK7001未上电", "请下压夹具，再测一次", INFO_LEVEL.FAIL);
                    mGet7001VersionTask.ClearAllEvent();
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
                    bTest4103 = false;
                    StartDynamicPotTicker(30, TASK_TYPE.RK7001_CHKVERSION);
                    mFirstRunningReqTask.Excute();                
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "测试Server未启动!", "", INFO_LEVEL.FAIL);
                    UpdateTestServer(sender, INFO_LEVEL.FAIL);           
                    mChk4103ServerTask.ClearAllEvent();
                }
            };
#endregion

#region RK4103上电 发送广播报文
            mFirstRunningReqTask = new SimpleSerialPortTask<NullEntity, get4103BroadcastReq>();
            mFirstRunningReqTask.RetryMaxCnts = 1;
            mFirstRunningReqTask.Timerout = 20*1000;
            bTest4103 = false;
            mFirstRunningReqTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<get4103BroadcastReq> mEventArgs = e as SerialPortEventArgs<get4103BroadcastReq>;
                if (mEventArgs.Data != null)
                {
                    string softVersion = String.Format("W{0}{1}.0{2}", (byte)(mEventArgs.Data.hardwareID >> 24), 
                                                                       (byte)(mEventArgs.Data.hardwareID >> 16), 
                                                                       (byte)(mEventArgs.Data.hardwareID >> 8));
                    DeviceInfo info = new DeviceInfo();
                    info.sw = softVersion;
                    UpdateRK4103Items(sender, RK4103ITEM.VERSION, info, INFO_LEVEL.NONE);
                    if(!bTest4103)
                    {
                        mPCTaskOverRspTask.Excute();
                        Thread.Sleep(100);
                        mGet7001VersionTask.Excute();
                    }
                    else
                    {
                        mFirstRunRspParam.deviceType = 0x01;
                        mFirstRunRspParam.firmwareYears = (byte)(mEventArgs.Data.hardwareID >> 24);
                        mFirstRunRspParam.firmwareWeeks = (byte)(mEventArgs.Data.hardwareID >> 16);
                        mFirstRunRspParam.firmwareVersion = (byte)(mEventArgs.Data.hardwareID >> 8);
                        mFirstRunRspParam.hardwareID = 0xFB;
                        mParamSettingReqTask.Excute();
                    }                  
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "待测设备未上电!", "", INFO_LEVEL.FAIL);
                    mFirstRunningReqTask.ClearAllEvent();
                }
            };
#endregion

#region 发送0x86, 接受0x03
            mParamSettingReqTask = new SimpleSerialPortTask<get4103BroadcastRsp, ParameterSettingReq>();
            mFirstRunRspParam = mParamSettingReqTask.GetRequestEntity();
            mParamSettingReqTask.RetryMaxCnts = 1;
            mParamSettingReqTask.Timerout = 5 * 1000;
            mParamSettingReqTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<ParameterSettingReq> mEventArgs = e as SerialPortEventArgs<ParameterSettingReq>;
                if (mEventArgs.Data != null)
                {
                    mParamSettingParam.deviceType = 0x03;
                    mParamSettingParam.testItem = 0x7F;
                    mParamSettingParam.sn = ByteProcess.stringToByteArray(mSN);
                    mParamSettingParam.edrAddr = ByteProcess.stringToByteArrayNoColon(mBTaddr);
                    mParamSettingParam.bleAddr = ByteProcess.stringToByteArrayNoColon(mBLEaddr);
                    mParamSettingParam.key = ByteProcess.stringToByteArray(mKeyt);
                    byte[] temp = new byte[4];
                    mParamSettingParam.adcParam = temp;
                    mParamSettingParam.v15Param = temp;
                    mParamSettingParam.sntemp = temp;
                    //等待接受结果
                    StartDynamicPotTicker(50, TASK_TYPE.RK4103_SELFTEST);
                    Thread.Sleep(100);
                    mRecvTestResultTask.Excute();
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "未收到参数配置请求!", "", INFO_LEVEL.FAIL);
                    mParamSettingReqTask.ClearAllEvent();
                }
            };
#endregion

#region 发送0x83， 接受0x23
            mRecvTestResultTask = new SimpleSerialPortTask<ParameterSettingRsp, boardTestResultReq>();
            mParamSettingParam = mRecvTestResultTask.GetRequestEntity();
            mRecvTestResultTask.RetryMaxCnts = 10;
            mRecvTestResultTask.Timerout = 5 * 1000;
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
                        UpdateRK4103Items(sender, RK4103ITEM.INIT, null, INFO_LEVEL.PASS);
                        UpdateRK4103Pins(sender, mRK4103Pins, INFO_LEVEL.PASS);
                        mSaveNvReqTask.Excute();
                    }
                    else
                    {
                        if ((m_result >> 0 & 0x1) == 1)
                        {
                            UpdateRK4103Items(sender, RK4103ITEM.GSENSOR, null, INFO_LEVEL.FAIL);
                            InputPinError = true;
                        }
                        else
                            UpdateRK4103Items(sender, RK4103ITEM.GSENSOR, null, INFO_LEVEL.PASS);
                        /*
                        if ((m_result >> 1 & 0x1) == 1)
                        {
                            UpdateRK4103Items(sender, RK4103ITEM.LIGHTSENSOR, null, INFO_LEVEL.FAIL);
                            InputPinError = true;
                        }
                        else
                            UpdateRK4103Items(sender, RK4103ITEM.LIGHTSENSOR, null, INFO_LEVEL.PASS);
                        */
                        if ((m_result >> 2 & 0x1) == 1)
                        {
                            UpdateRK4103Items(sender, RK4103ITEM.VDD33, null, INFO_LEVEL.FAIL);
                            InputPinError = true;
                        }
                        else
                            UpdateRK4103Items(sender, RK4103ITEM.VDD33, null, INFO_LEVEL.PASS);

                        if ((m_result >> 4 & 0x1) == 1) 
                        {
                            if ((inputPin >> 0 & 0x1) == 1) //右转灯
                            {
                                mRK4103Pins.Pin27_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 1 & 0x1) == 1)//左转灯
                            {
                                mRK4103Pins.Pin28_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 2 & 0x1) == 1)//远近光
                            {
                                mRK4103Pins.Pin3_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 3 & 0x1) == 1)//喇叭
                            {
                                mRK4103Pins.Pin4_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 4 & 0x1) == 1)//P档切换
                            {
                                mRK4103Pins.Pin12_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 5 & 0x1) == 1)//Power档切换
                            {
                                mRK4103Pins.Pin18_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 6 & 0x1) == 1)//开关大灯
                            {
                                mRK4103Pins.Pin14_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 7 & 0x1) == 1)//自动大灯
                            {
                                mRK4103Pins.Pin13_Open = true;
                                InputPinError = true;
                            }
                            if ((inputPin >> 8 & 0x1) == 1)
                            {
                                UpdateRK4103Items(sender, RK4103ITEM.GPIO26, null, INFO_LEVEL.FAIL);
                                InputPinError = true;
                            }
                            else
                                UpdateRK4103Items(sender, RK4103ITEM.GPIO26, null, INFO_LEVEL.PASS);
                            
                            if(bCruised)
                            {
                                if ((inputPin >> 9 & 0x1) == 1)  //巡航开关
                                    mRK4103Pins.Pin26_Open = true;
                            }
                            if(bPushcar)
                            {
                                if ((inputPin >> 10 & 0x1) == 1) //推车开关
                                    mRK4103Pins.Pin17_Open = true;
                            }                   
                            if(bBackcar)
                            {
                                if ((inputPin >> 11 & 0x1) == 1) //倒车开关
                                    mRK4103Pins.Pin16_Open = true;
                            }
                            if(bRepaired)
                            {
                                if ((inputPin >> 12 & 0x1) == 1)//一键修复
                                    mRK4103Pins.Pin19_Open = true;
                            }                 
                        }
                        //output pin
                        if ((m_result >> 5 & 0x1) == 1)
                        {
                            if ((outpin1 >> 2 & 0x1) == 1)
                            {
                                UpdateRK4103Items(sender, RK4103ITEM.GPIO27, null, INFO_LEVEL.FAIL);
                                InputPinError = true;
                            }
                            else
                                UpdateRK4103Items(sender, RK4103ITEM.GPIO27, null, INFO_LEVEL.PASS);
                            if ((outpin1 >> 3 & 0x1) == 1)
                            {
                                UpdateRK4103Items(sender, RK4103ITEM.PWM, null, INFO_LEVEL.FAIL);
                                InputPinError = true;
                            }
                            else
                                UpdateRK4103Items(sender, RK4103ITEM.PWM, null, INFO_LEVEL.PASS);
                        }
                        //扩展pin脚
                        if ((m_result >> 8 & 0x1) == 1)
                        {
                            if ((extensionOut >> 0 & 0x1) == 1)//LCM_左转指示
                            {
                                mRK4103Pins.Pin8_Open = true;
                                InputPinError = true;
                            }
                            if ((extensionOut >> 1 & 0x1) == 1)//LCM_远光指示
                            {
                                mRK4103Pins.Pin24_Open = true;
                                InputPinError = true;
                            }
                            if ((extensionOut >> 2 & 0x1) == 1)//LCM_READY指示
                            {
                                mRK4103Pins.Pin7_Open = true;
                                InputPinError = true;
                            }
                            if ((extensionOut >> 3 & 0x1) == 1) //LCM_PARK指示
                            {
                                mRK4103Pins.Pin6_Open = true;
                                InputPinError = true;
                            }
                            if ((extensionOut >> 4 & 0x1) == 1)//LCM_右转指示
                            {
                                mRK4103Pins.Pin25_Open = true;
                                InputPinError = true;
                            }
                            if ((extensionOut >> 5 & 0x1) == 1)//LCM_CS
                            {
                                mRK4103Pins.Pin21_Open = true;
                                InputPinError = true;
                            }
                            if ((extensionOut >> 6 & 0x1) == 1)//LCM_CLK
                            {
                                mRK4103Pins.Pin23_Open = true;
                                InputPinError = true;
                            }
                            if ((extensionOut >> 7 & 0x1) == 1)//LCM_DO
                            {
                                mRK4103Pins.Pin10_Open = true;
                                InputPinError = true;
                            }
                        }
                        //最后判断
                        if(!InputPinError)
                        {
                            UpdateRK4103Items(sender, RK4103ITEM.INIT, null, INFO_LEVEL.PASS);
                            UpdateRK4103Pins(sender, mRK4103Pins, INFO_LEVEL.PASS);
                            mSaveNvReqTask.Excute();
                        }
                        else
                        {
                            //更新RK4103的Pin脚图
                            StopDynamicPotTicker(sender);
                            UpdateRK4103Pins(sender, mRK4103Pins, INFO_LEVEL.FAIL);
                            SetMainText(sender, "测试失败!", "", INFO_LEVEL.FAIL);
                            mRecvTestResultTask.ClearAllEvent();
                            return;
                        }                           
                    }                    
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "未收到测试结果!", "", INFO_LEVEL.FAIL);
                    mRecvTestResultTask.ClearAllEvent();
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
                    StartDynamicPotTicker(30, TASK_TYPE.RK4103_BLETEST);
                    Thread.Sleep(100);
                    mBtTestReqTask.Excute();
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "未收到写NV请求!", "", INFO_LEVEL.FAIL);
                    mSaveNvReqTask.ClearAllEvent();
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
                    bool bEdrFlag = false;
                    bool bBleFlag = false;
                    StopDynamicPotTicker(sender);
                    if (mEventArgs.Data.edrResult == 1)
                    {
                        bEdrFlag = true;
                        UpdateRK4103Items(sender, RK4103ITEM.EDR, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK4103Items(sender, RK4103ITEM.EDR, null, INFO_LEVEL.PASS);
                    if (mEventArgs.Data.bleResult == 1)
                    {
                        bBleFlag = true;
                        UpdateRK4103Items(sender, RK4103ITEM.BLE, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK4103Items(sender, RK4103ITEM.BLE, null, INFO_LEVEL.PASS);

                    if(bEdrFlag || bBleFlag)
                    {
                        StopDynamicPotTicker(sender);
                        SetMainText(sender, "蓝牙测试失败!", "", INFO_LEVEL.FAIL);
                        mBtTestReqTask.ClearAllEvent();
                        return;
                    }                        
                    else
                    {
                        mCompeteTestTask.EnableTimeOutHandler = false;
                        mCompeteTestTask.Excute();
                        StopDynamicPotTicker(sender);
                        SetMainText(sender, "蓝牙测试成功!开始目检...", "", INFO_LEVEL.PROCESS);
                        //人工目检
                        mRK7001ResultType = RK7001ResultType.OPEN_BUZZER_RSP;
                        mGet700ResultParam.ack_device = 0X07;//发给PCU
                        mGet700ResultParam.ecu_status = 0x14;//默认;//默认
                        mGet700ResultParam.server_mode = 0x04;//开启软件上电
                        mGet700ResultParam.trigger_ctrl = 0 | 1 << 3;//开启Buzzer
                        mGet7001ResultTask.Excute();
                        ManualVisio mMaualFrm = new ManualVisio();
                        DialogResult result = mMaualFrm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            //保存数据
                            SetMainText(sender, "测试成功！", "", INFO_LEVEL.PASS);
                            mMaualFrm.Close();
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            SetMainText(sender, "目检失败！", "", INFO_LEVEL.FAIL);
                            mMaualFrm.Close();
                        }
                        mGet700ResultParam.trigger_ctrl = 0;//关闭Buzzer                      
                    }          
                }
                else
                {
                    StopDynamicPotTicker(sender);
                    SetMainText(sender, "未收到蓝牙测试结果！", "", INFO_LEVEL.FAIL);
                    mBtTestReqTask.ClearAllEvent();
                }
            };
#endregion

#region 测试蓝牙 回应
            mCompeteTestTask = new SimpleSerialPortTask<btTestRsp, NullEntity>();
            mBtTestRspParam = mCompeteTestTask.GetRequestEntity();
            mBtTestRspParam.status = 0;
#endregion
        }
        #endregion

        #region Step0:软件上电，开始自测
        private Level Step0_OpenSoftDC(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            if (mAckDevice == Const.PCU)
            {
                return Level.TRUE;
            }
            else
                return Level.REPEAT;
            return Level.FALSE ;
        }
        #endregion

        #region Step1：Client 自检检测  
        private Level Step1_SelfCheck(object sender, get7001ResultRsp mArgs)
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
                if (mAccStatus != 1)
                    return Level.FALSE;
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
                    }
                    else
                    {
                        UpdateRK7001Items(sender, RK7001ITEM.AMPLIFY, null, INFO_LEVEL.PASS);
                    }
                    if ((mDeviceFault >> 2 & 0x1) == 1)//dc使能控制
                    {
                        testResult = true;
                        mRK7001Pins.Pin10_Open = true;
                        mRK7001Pins.Pin21_Open = true;
                    }
                    if ((mDeviceFault >> 3 & 0x1) == 1 || (mDeviceFault >> 4 & 0x1) == 1)//DC故障、输出过压
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.DC, null, INFO_LEVEL.FAIL);
                    }
                    else
                    {
                        UpdateRK7001Items(sender, RK7001ITEM.DC, null, INFO_LEVEL.PASS);
                    }
                    if ((mDeviceFault >> 5 & 0x1) == 1) //mosfet故障
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.MOSFET, null, INFO_LEVEL.FAIL);
                    }
                    else
                    {
                        UpdateRK7001Items(sender, RK7001ITEM.MOSFET, null, INFO_LEVEL.PASS);
                    }
                    if ((mCutError1 >> 0 & 0x1) == 1)//前左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin27_Open = true;
                    }
                    if ((mCutError1 >> 1 & 0x1) == 1)//后左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin30_Open = true;
                    }
                    if ((mCutError1 >> 2 & 0x1) == 1)//前右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin26_Open = true;
                    }
                    if ((mCutError1 >> 3 & 0x1) == 1)//后右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin29_Open = true;
                    }
                    if ((mCutError1 >> 4 & 0x1) == 1)//近光
                    {
                        testResult = true;
                        mRK7001Pins.Pin5_Open = true;
                    }
                    if ((mCutError1 >> 5 & 0x1) == 1)//远光
                    {
                        testResult = true;
                        mRK7001Pins.Pin6_Open = true;
                    }
                    if ((mCutError1 >> 6 & 0x1) == 1)//尾灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin2_Open = true;
                    }

                    if ((mCutError1 >> 7 & 0x1) == 1)//刹车灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin3_Open = true;
                    }

                    if ((mCutError2 >> 0 & 0x1) == 1)//背景灯R
                    {
                        testResult = true;
                        mRK7001Pins.Pin25_Open = true;
                    }
                    /*
                    if ((mCutError2 >> 1 & 0x1) == 1)//背景灯G
                        mRK7001Pins.Pin7_Open = true;
                    if ((mCutError2 >> 2 & 0x1) == 1)//背景灯B
                        mRK7001Pins.Pin24_Open = true;
                    */
                    if ((mCutError2 >> 3 & 0x1) == 1)//喇叭
                    {
                        testResult = true;
                        mRK7001Pins.Pin4_Open = true;
                    }
                    if ((mCutError2 >> 4 & 0x1) == 1)//BUZZER
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.PASS);
                    if ((mShortError1 >> 0 & 0x1) == 1)//前左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin27_Short = true;
                    }
                    if ((mShortError1 >> 1 & 0x1) == 1)//后左转
                    {
                        testResult = true;
                        mRK7001Pins.Pin30_Short = true;
                    }
                    if ((mShortError1 >> 2 & 0x1) == 1)//前右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin26_Short = true;
                    }
                    if ((mShortError1 >> 3 & 0x1) == 1)//后右转
                    {
                        testResult = true;
                        mRK7001Pins.Pin29_Short = true;
                    }
                    if ((mShortError1 >> 4 & 0x1) == 1)//近光
                    {
                        testResult = true;
                        mRK7001Pins.Pin5_Short = true;
                    }
                    if ((mShortError1 >> 5 & 0x1) == 1)//远光
                    {
                        testResult = true;
                        mRK7001Pins.Pin6_Short = true;
                    }
                    if ((mShortError1 >> 6 & 0x1) == 1)//尾灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin2_Short = true;
                    }
                    if ((mShortError1 >> 7 & 0x1) == 1)//刹车灯
                    {
                        testResult = true;
                        mRK7001Pins.Pin3_Short = true;
                    }
                    if ((mShortError2 >> 0 & 0x1) == 1)//背景灯R
                    {
                        testResult = true;
                        mRK7001Pins.Pin25_Short = true;
                    }
                    /*
                    if ((mShortError2 >> 1 & 0x1) == 1)//背景灯G
                        mRK7001Pins.Pin7_Short = true;
                    if ((mShortError2 >> 2 & 0x1) == 1)//背景灯B
                        mRK7001Pins.Pin24_Short = true;
                    */
                    if ((mShortError2 >> 3 & 0x1) == 1)//喇叭
                    {
                        testResult = true;
                        mRK7001Pins.Pin4_Open = true;
                    }
                    if ((mShortError2 >> 4 & 0x1) == 1)//BUZZER
                    {
                        testResult = true;
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.FAIL);
                    }
                    else
                        UpdateRK7001Items(sender, RK7001ITEM.BUZZER, null, INFO_LEVEL.PASS);

                    if ((mDcCurrent * 100 < 50) || (mDcCurrent * 100 > 150))//判断DC输出电流
                    {
                        testResult = true;
                        mRK7001Pins.Pin3_Open = true;
                    }

                    if (!testResult)
                    {
                        //测试成功                        
                        UpdateRK7001Items(sender, RK7001ITEM.INIT, null, INFO_LEVEL.PASS);
                        UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.PASS);
                    }
                    else
                    {
                        UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.FAIL);
                        StopDynamicPotTicker(sender);
                        SetMainText(sender, "测试失败!", "", INFO_LEVEL.FAIL);
                        mGet7001ResultTask.ClearAllEvent();
                        return Level.FALSE;
                    }

                    return Level.TRUE;
                }
            }
            else
                return Level.REPEAT;
            return Level.FALSE;
        }
        #endregion

        #region Step2: 开关全部关闭
        private Level Step2_Init_2(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAccStatus = (byte)mArgs.AccStatus;
            if (mAckDevice == Const.PCU)
            {
                //读取Server返回的状态数据  
                /*              
                if (mAccStatus == 0)
                {
                    return Level.TRUE;
                }
                */
                return Level.TRUE;
            }
            else if(mAckDevice == Const.TESTSERVER)
                    return Level.REPEAT;

            return Level.FALSE;
        }
        #endregion

        #region Step3: 判断KSI是否打开
        private Level Step3_OpenKSI(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            if(mAckDevice == Const.TESTSERVER)
            {
                if(mAccStatus != 0)
                {
                    return Level.TRUE;
                }              
            }
            else
            {
                return Level.REPEAT;
            }
            return Level.FALSE;
        }
        #endregion

        #region Step4:关闭soft
        private Level Step4_CloseKSI(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDCout = (byte)mArgs.DcVoltage;
            if(mAckDevice == Const.TESTSERVER)
            {
                if(mAccStatus == 0)
                {
                    return Level.TRUE;
                }                        
            }
            else
            {
                return Level.REPEAT;
            }

            return Level.FALSE; ;
        }
        #endregion

        #region Step5: 软件打开DC
        private Level Step5_SoftOpenEcu(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            if (mAckDevice == Const.PCU)
            {                
                return Level.TRUE;                                            
            }
            else if(mAckDevice == Const.TESTSERVER)
            {
                return Level.REPEAT;
            }

            return Level.FALSE;           
        }
        #endregion

        #region Step6:读取软件开电状态
        private Level Step6_ReadOpenEcu(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            if (mAckDevice == Const.TESTSERVER)
            {
                //判断是否KSI是否已经关闭
                if (mAccStatus != 0 && mDcOUT != 0)
                {
                    mRK7001Pins.Pin14_Open = false;
                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.PASS);
                    return Level.TRUE;
                }
                else
                {
                    mRK7001Pins.Pin14_Open = true;
                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.PASS);
                    return Level.FALSE;
                }
            }
            else if(mAckDevice == Const.PCU)
                       return Level.REPEAT;


            return Level.FALSE; ;
        }
        #endregion

        #region Step7:软件关电
        private Level Step7_SoftCloseEcu(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;

            if (mAckDevice == Const.PCU)
            {
                return Level.TRUE;
            }
            else if(mAckDevice == Const.TESTSERVER)
                    return Level.REPEAT;

            return Level.FALSE;
        }
        #endregion

        #region Step8: 读软件关电状态
        private Level Step8_ReadCloseEcu(Object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            byte mAccStatus = (byte)mArgs.AccStatus;
            byte mDcOUT = (byte)mArgs.DcVoltage;
            if (mAckDevice == Const.TESTSERVER)
            {
                //判断是否KSI是否已经关闭
                if (mAccStatus == 0 && mDcOUT == 0)
                {
                    mRK7001Pins.Pin14_Open = false;
                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.PASS);
                    return Level.TRUE;
                }
                else
                {
                    mRK7001Pins.Pin14_Open = true;
                    UpdateRK7001Pins(sender, mRK7001Pins, INFO_LEVEL.PASS);
                    return Level.FALSE;
                }
            }
            else if (mAckDevice == Const.PCU)
                return Level.REPEAT;

            return Level.FALSE;
        }
        #endregion


        #region Step9: Server 通知server，按下遥控器按键
        private Level Step9_RemoteInit(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            if(mAckDevice == Const.TESTSERVER)
            {
                if(mAckValue == Const.TESTSERVER_KEY_PRESS)
                {                   
                    return Level.TRUE;
                }                        
            }
            else if(mAckDevice == Const.PCU)
            {
                return Level.REPEAT;
            }

            return Level.FALSE;
        }
        #endregion

        #region Step10: Client 通知client，进行配对
        private Level Step10_RemoteTest(object sender, get7001ResultRsp mArgs)
        {
            byte mAckDevice = (byte)mArgs.ack_device;
            byte mAckValue = (byte)mArgs.ack_value;
            if (mAckDevice == Const.PCU)
            {
                if (mAckValue == Const.REMOTE_CONNECTED)
                {
                    return Level.TRUE;
                }
                else if (mAckValue == Const.REMOTE_CHECK_FAIL)
                {
                    return Level.FALSE;
                }
                else if (mAckValue == Const.REMOTE_NO_RECV)
                {
                    return Level.FALSE;
                }
                else
                    return Level.REPEAT;
            }
            else if (mAckDevice == Const.TESTSERVER)
                return Level.REPEAT;

            return Level.FALSE;
        }
        #endregion


        #region RK7001目检失败
        public void RK7001KSICheckFail()
        {
            mGet7001ResultTask.ClearAllEvent();
            SetMainText(this, "RK7001目检失败!", "", INFO_LEVEL.FAIL);
        }
        #endregion

        #region 启动动态显示
        private void StartDynamicPotTicker(int ticker, TASK_TYPE type)
        {
            nPotTicker = ticker;
            mTaskType = type;
            DynamicPotTicker.Enabled = true;
        }
        #endregion

        #region 停止动态显示
        private void StopDynamicPotTicker(object sender)
        {
            DynamicPotTicker.Enabled = false;
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

        #region 通过SN，获取RK4103的数据
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
                    StopDynamicPotTicker(sender);
                    UpdateValidSN(sender, INFO_LEVEL.FAIL);
                    SetMainText(sender, "该标签已经被使用过！", "SAVENONE", INFO_LEVEL.FAIL);
                    return false;
                }
                else
                {
                    UpdateValidSN(sender, INFO_LEVEL.PASS);
                    StartDynamicPotTicker(10, TASK_TYPE.CHK_TEST_SERVER);
                    mChk4103ServerTask.Excute();
                }                
            }
            else
            {
                StopDynamicPotTicker(sender);
                UpdateValidSN(sender, INFO_LEVEL.FAIL);
                SetMainText(sender, "获取不到服务器信息", "SAVENONE", INFO_LEVEL.FAIL);
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
                if(bServerActivated)
                {
                    if(submsg != "SAVENONE")
                    {
                        mTestResult = msg;
                        SaveData2Server(level);
                    }                   
                }         
                bTaskRunning = false;
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

        #region 刷新RK4103列表
        private void UpdateRK4103Items(object sender, RK4103ITEM items, DeviceInfo info, INFO_LEVEL level)
        {
            if(Update4103ListViewHandler != null)
            {
                RK4103ItemsArgs mArgs = new RK4103ItemsArgs();
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

        #region 刷新RK4103的PIN脚
        private void UpdateRK4103Pins(object sender, PIN_STATUS pins, INFO_LEVEL level)
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


        #region 初始化配置参数
        private void InitParameters()
        {
            if(mGet700ResultParam != null)
            {
                mGet700ResultParam.ack_device = Const.PCU;//发给PCU
                mGet700ResultParam.ecu_status = 0;//默认;//默认
                mGet700ResultParam.server_mode = 0x4;//开启软件上电
                mGet700ResultParam.backlight = 0;
                mGet700ResultParam.batt_soc = 0;
                mGet700ResultParam.level_ctrl = 0 | 0x01 << 8;
                mGet700ResultParam.limit_per = 0;
                mGet700ResultParam.trigger_ctrl = 0;
            }

            bTaskRunning = true;
            nPotTicker = 0;
            mBTaddr = "";
            mBLEaddr = "";
            mKeyt = "";
            mUid = "";
            mTestResult = "";
        }
        #endregion

        public void ExcuteTask()
        {
            InitParameters();
            StopDynamicPotTicker(this);
            mRK7001Pins = new PIN_STATUS();
            mRK4103Pins = new PIN_STATUS();
            UpdateRK7001Pins(this, mRK7001Pins, INFO_LEVEL.PROCESS);
            UpdateRK4103Pins(this, mRK4103Pins, INFO_LEVEL.PROCESS);
            UpdateRemoteStatus(this, INFO_LEVEL.PROCESS);
            if(bServerActivated)
            {
                if (!GetValueFrmServer(this))
                    return;
            }
            else
            {
                mSN = DefaultSN;
                mBTaddr = DefaultBT;
                mBLEaddr = DefaultBLE;
                mKeyt = DefaultKeyt;
                UpdateValidSN(this, INFO_LEVEL.PASS);
                StartDynamicPotTicker(10, TASK_TYPE.CHK_TEST_SERVER);
                mChk4103ServerTask.Excute();
            }      
        }
    }
}
