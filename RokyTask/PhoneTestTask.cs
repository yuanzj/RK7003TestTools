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
using System.Windows.Forms;

namespace RokyTask
{
    public enum BindSteps
    {
        WAIT_POWER = 0,
        SN_VALID = 1,
        KEY1_BIND = 2,
        KEY2_BIND = 3,
        WRITE_NV = 4,
        READ_KEYS = 5
    }

    public enum KeyType
    {
        NONE_KEY = 0,
        BIND_KEY1 = 1,
        BIND_KEY2 = 2,
        WRITE_NV = 3,
        READ_KEYS = 4
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
        READ_KEYS = 9,
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
        public string mSN { get; set; }
        private int mKey1Value { get; set; }
        private int mKey2Value { get; set; }
        public int TryCnts { get; set; }
        public bool bTaskRunning { get; set; }



        #endregion

        #region 注册事件
        public event EventHandler UpdateWorkStatusHandler;
        public event EventHandler UpdateValidSNHandler;
        public event EventHandler BindKey1Handler;
        public event EventHandler BindKey2Handler;
        public event EventHandler WriteNVHandler;
        public event EventHandler ListViewHandler;
        public event EventHandler KeyValueHandler;
        public event EventHandler ReadKeysHandler;
        #endregion

        #region 注册串口事件
        //5.1.1	PC探测TS请求(0X32)
        SimpleSerialPortTask<CheckTSReq, TSCheckRsp> checkTSTask;
        CheckTSReq mCheckTSReqParam;

        //5.1.7	PC查询TS状态请求（0X35）
        SimpleSerialPortTask<CheckTSStatusReq, TSWriteSnStatusRsp> checkTSStatusTask;
        CheckTSStatusReq mCheckTSStatusReqParam;
        //清除
        SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp> mClearKeyTask;
        writeKeyAddrReq mClearKeyParam;
        //检查
        SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp> mWriteKeyTask;
        writeKeyAddrReq mWriteKeyParam;
        //查询
        SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp> mReadKeyAddrTask;
        writeKeyAddrReq mReadKeyAddrParam;

        int retryCount = 0;
        int writeCount = 0;
        int readCount = 0;

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
            #region PC探测TS请求(0X32)
            checkTSTask = new SimpleSerialPortTask<CheckTSReq, TSCheckRsp>();
            mCheckTSReqParam = checkTSTask.GetRequestEntity();
            mCheckTSReqParam.deviceType = 0;
            mCheckTSReqParam.reserveValue = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            checkTSTask.RetryMaxCnts = 6;
            checkTSTask.Timerout = 500;
            checkTSTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<TSCheckRsp> mEventArgs = e as SerialPortEventArgs<TSCheckRsp>;
                if (mEventArgs.Data != null)
                {
                    SetMainText(sender, STEP_LEVEL.CHECK_SN);
                    checkTSStatusTask.Excute();
                }
                else
                {
                    showErrorMsg(sender, "上电超时！", "设备未上电或通讯有异常", BindSteps.WAIT_POWER, STEP_LEVEL.FAIL);
                }
            };
            #endregion

            #region 5.1.7	PC查询TS状态请求（0X35）
            checkTSStatusTask = new SimpleSerialPortTask<CheckTSStatusReq, TSWriteSnStatusRsp>();
            mCheckTSStatusReqParam = checkTSStatusTask.GetRequestEntity();
            mCheckTSStatusReqParam.checkType = 0;
            mCheckTSStatusReqParam.reserveValue = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            checkTSStatusTask.RetryMaxCnts = 6;
            checkTSStatusTask.Timerout = 500;
            checkTSStatusTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<TSWriteSnStatusRsp> mEventArgs = e as SerialPortEventArgs<TSWriteSnStatusRsp>;
                if (mEventArgs.Data != null)
                {
                    //发现测试server存在则发送
                    TSWriteSnStatusRsp mTSWriteSnStatusRsp = mEventArgs.Data;

                    if (mTSWriteSnStatusRsp.checkType == 0)
                    {
                        retryCount++;

                        if (mTSWriteSnStatusRsp.subStatus == 0)
                        {
                            string sn = ByteProcess.byteArrayToString(mTSWriteSnStatusRsp.sn);
                            string ble = Util.ToHexString(mTSWriteSnStatusRsp.bleAddr);
                            string key = ByteProcess.byteArrayToString(mTSWriteSnStatusRsp.key);
                            string devInfo = String.Format("SN:{0},BLE4.0地址:{1},密钥:{2}", sn, ble, key);

                            if (mSN == sn)
                            {
                                SetValidSN(sender, INFO_LEVEL.PASS);
                                mClearKeyTask.Excute();
                            }
                            else
                            {
                                //1：CCU 报文未停止
                                //2：CCU未响应
                                if (retryCount >= 5)
                                {
                                    showErrorMsg(sender, "SN号获取失败", "", BindSteps.SN_VALID, STEP_LEVEL.FAIL);
                                }
                                else {
                                    checkTSStatusTask.Excute();
                                }
                            }

                        }
                        else
                        {

                            //1：CCU 报文未停止
                            //2：CCU未响应
                            if (retryCount >= 5)
                            {
                                showErrorMsg(sender, "SN号获取失败", "", BindSteps.SN_VALID, STEP_LEVEL.FAIL);
                            }
                            else
                            {
                                checkTSStatusTask.Excute();
                            }
                        }


                    }

                }
                else
                {
                    showErrorMsg(sender, "SN号获取失败", "", BindSteps.SN_VALID, STEP_LEVEL.FAIL);
                }
            };
            #endregion

            #region 清除NV存储的钥匙
            mClearKeyTask = new SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp>();
            mClearKeyParam = mClearKeyTask.GetRequestEntity();
            mClearKeyTask.RetryMaxCnts = 0;
            mClearKeyTask.Timerout = 1 * 1000;
            mClearKeyTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<writeKeyAddrRsp> mEventArgs = e as SerialPortEventArgs<writeKeyAddrRsp>;
                if (mEventArgs.Data != null)
                {
                    byte mResult = (byte)mEventArgs.Data.Result;
                    if (mResult == 0)
                    {
                        Thread.Sleep(500);

                        SetMainText(sender, STEP_LEVEL.BIND_KEY1);
                        mWriteKeyTask.Excute();
                    }
                    else
                    {
                        showErrorMsg(sender, "清除钥匙配置失败", "清除NV未成功，或通讯有异常", BindSteps.WRITE_NV, STEP_LEVEL.FAIL);
                    }
                }
                else
                {
                    showErrorMsg(sender, "清除钥匙配置失败", "清除NV未成功，或通讯有异常", BindSteps.WRITE_NV, STEP_LEVEL.FAIL);
                }
            };
            #endregion

            mWriteKeyTask = new SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp>();
            mWriteKeyParam = mWriteKeyTask.GetRequestEntity();
            mWriteKeyTask.RetryMaxCnts = 6;
            mWriteKeyTask.Timerout = 500;
            mWriteKeyTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<writeKeyAddrRsp> mEventArgs = e as SerialPortEventArgs<writeKeyAddrRsp>;
                if (mEventArgs.Data != null)
                {
                    byte mResult = (byte)mEventArgs.Data.Result;
                    if (mResult == 0)
                    {
                        writeCount++;

                        byte[] Key1Value = mEventArgs.Data.Key1Value;
                        byte[] Key2Value = mEventArgs.Data.Key2Value;

                        mKey1Value = ByteProcess.byteArrayToInt(new byte[] { Key1Value[0], Key1Value[1], Key1Value[2], 0 }, 0);
                        mKey2Value = ByteProcess.byteArrayToInt(new byte[] { Key2Value[0], Key2Value[1], Key2Value[2], 0 }, 0);

                        mWriteKeyParam.Key1Value = Key1Value;
                        mWriteKeyParam.Key2Value = Key2Value;

                        mReadKeyAddrParam.Key1Value = Key1Value;
                        mReadKeyAddrParam.Key2Value = Key2Value;

                        if (mKey1Value != 0 && mKey2Value != 0)
                        {
                            if (KeyNumber == 2)
                            {

                                SetBindKey1(sender, INFO_LEVEL.PASS);
                                SetKeyValue(sender, KeyType.BIND_KEY1, mKey1Value);

                                SetBindKey2(sender, INFO_LEVEL.PASS);
                                SetKeyValue(sender, KeyType.BIND_KEY2, mKey2Value);

                                mReadKeyAddrTask.Excute();
                                return;
                            }
                            else if (KeyNumber == 1)
                            {
                                SetBindKey1(sender, INFO_LEVEL.PASS);
                                SetKeyValue(sender, KeyType.BIND_KEY1, mKey1Value);

                                mReadKeyAddrTask.Excute();
                                return;
                            }
                        }
                        else if (mKey1Value != 0)
                        {
                            SetBindKey1(sender, INFO_LEVEL.PASS);
                            SetKeyValue(sender, KeyType.BIND_KEY1, mKey1Value);

                            if (KeyNumber == 1)
                            {
                                mReadKeyAddrTask.Excute();
                                return;
                            }

                            if (writeCount >= 10 * 5 * 2)
                            {
                                SetBindKey1(sender, INFO_LEVEL.PASS);
                                SetKeyValue(sender, KeyType.BIND_KEY1, mKey1Value);
                                showErrorMsg(sender, "监听钥匙配置失败", "未能获取到蓝牙钥匙2key", BindSteps.KEY2_BIND, STEP_LEVEL.FAIL);
                            }
                            else {
                                Thread.Sleep(100);
                                mWriteKeyTask.Excute();
                            }
                            

                        }
                        else
                        {
                            if (writeCount >= 10 * 5 * 2)
                            {
                                showErrorMsg(sender, "监听钥匙配置失败", "未能获取到蓝牙钥匙1key", BindSteps.KEY1_BIND, STEP_LEVEL.FAIL);
                            }
                            else
                            {
                                Thread.Sleep(100);
                                mWriteKeyTask.Excute();
                            }
                        }
                    }
                    else
                    {
                        showErrorMsg(sender, "监听钥匙配置失败", "结果为非完成状态", BindSteps.KEY1_BIND, STEP_LEVEL.FAIL);
                    }
                }
                else
                {
                    showErrorMsg(sender, "监听钥匙配置失败", "设备异常或通讯有异常", BindSteps.KEY1_BIND, STEP_LEVEL.FAIL);
                }
            };


            mReadKeyAddrTask = new SimpleSerialPortTask<writeKeyAddrReq, writeKeyAddrRsp>();
            mReadKeyAddrParam = mReadKeyAddrTask.GetRequestEntity();
            mReadKeyAddrTask.RetryMaxCnts = 0;
            mReadKeyAddrTask.Timerout = 5 * 1000;
            mReadKeyAddrTask.SimpleSerialPortTaskOnPostExecute += (object sender, EventArgs e) =>
            {
                SerialPortEventArgs<writeKeyAddrRsp> mEventArgs = e as SerialPortEventArgs<writeKeyAddrRsp>;
                if (mEventArgs.Data != null)
                {
                   

                    byte[] Key1Value = mEventArgs.Data.Key1Value;
                    byte[] Key2Value = mEventArgs.Data.Key2Value;

                    int readKey1Value = ByteProcess.byteArrayToInt(new byte[] { Key1Value[0], Key1Value[1], Key1Value[2], 0 }, 0);
                    int readKey2Value = ByteProcess.byteArrayToInt(new byte[] { Key2Value[0], Key2Value[1], Key2Value[2], 0 }, 0);

                    if (readCount == 0 )
                    {
                        readCount++;
                        Thread.Sleep(2000);
                        SetWriteNV(sender, INFO_LEVEL.PASS);
                        mReadKeyAddrParam.KeyNumber = 0XFF;
                        mReadKeyAddrTask.Excute();
                    }
                    else {

                        if (mKey1Value == readKey1Value && mKey2Value == readKey2Value)
                        {
                            string msg = String.Format("绑定后，当前已绑定{0}把，钥匙1:{1:X}, 钥匙2:{2:X}", KeyNumber, mKey1Value, mKey2Value);
                            SetListView(sender, msg, "");
                            SetReadKeys(sender, INFO_LEVEL.PASS, KeyNumber, mKey1Value, mKey2Value);
                            SetMainText(sender, STEP_LEVEL.PASS);
                            StopTask();
                        }
                        else {
                            showErrorMsg(sender, "读钥匙地址失败", "读取和写入的值不一致", BindSteps.READ_KEYS, STEP_LEVEL.FAIL);
                        }
                    }
                }
                else
                {
                    showErrorMsg(sender, "读钥匙地址失败", "设备异常或通讯有异常", BindSteps.READ_KEYS, STEP_LEVEL.FAIL);
                }
            };
        }
        #endregion

        #region 展示错误并停止任务
        private void showErrorMsg(object sender, string msg, string submsg, BindSteps step, STEP_LEVEL mainLevel) {
            SetListView(sender, msg, submsg);
            SetItemFail(sender, step);
            SetMainText(sender, mainLevel);
            StopTask();
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
                    SetReadKeys(sender, INFO_LEVEL.INIT, 0, 0, 0);
                    break;
                case BindSteps.SN_VALID:
                    SetValidSN(sender, INFO_LEVEL.FAIL);
                    SetBindKey1(sender, INFO_LEVEL.INIT);
                    SetBindKey2(sender, INFO_LEVEL.INIT);
                    SetWriteNV(sender, INFO_LEVEL.INIT);
                    SetReadKeys(sender, INFO_LEVEL.INIT, 0, 0, 0);
                    break;
                case BindSteps.KEY1_BIND:
                    SetBindKey1(sender, INFO_LEVEL.FAIL);
                    SetBindKey2(sender, INFO_LEVEL.INIT);
                    SetWriteNV(sender, INFO_LEVEL.INIT);
                    SetReadKeys(sender, INFO_LEVEL.INIT, 0, 0, 0);
                    break;
                case BindSteps.KEY2_BIND:
                    SetBindKey2(sender, INFO_LEVEL.FAIL);
                    SetWriteNV(sender, INFO_LEVEL.INIT);
                    SetReadKeys(sender, INFO_LEVEL.INIT, 0, 0, 0);
                    break;
                case BindSteps.WRITE_NV:
                    SetWriteNV(sender, INFO_LEVEL.FAIL);
                    SetReadKeys(sender, INFO_LEVEL.INIT, 0, 0, 0);
                    break;
                case BindSteps.READ_KEYS:
                    SetReadKeys(sender, INFO_LEVEL.FAIL, 0, 0, 0);
                    break;
                                     
            }
        }
        #endregion

        #region 更新主页显示状态
        private void SetMainText(object sender, STEP_LEVEL _level)
        {
            if (_level == STEP_LEVEL.FAIL || _level == STEP_LEVEL.PASS || _level == STEP_LEVEL.BIND_TIMEOUT) {

            }

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

        #region 写ReadKeys
        private void SetReadKeys(object sender, INFO_LEVEL _level, int _num, int key1, int key2)
        {
            if(ReadKeysHandler != null)
            {
                UIEventArgs mArgs = new UIEventArgs();
                mArgs.level = _level;
                mArgs.num = _num;
                mArgs.key1 = key1;
                mArgs.key2 = key2;
                ReadKeysHandler(sender, mArgs);
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
            bTaskRunning = false;
            checkTSTask.ClearAllEvent();
            checkTSStatusTask.ClearAllEvent();
            mClearKeyTask.ClearAllEvent();
            mWriteKeyTask.ClearAllEvent();
            mReadKeyAddrTask.ClearAllEvent();

            checkTSTask.EnableTimeOutHandler = false;
            checkTSStatusTask.EnableTimeOutHandler = false;
            mClearKeyTask.EnableTimeOutHandler = false;
            mWriteKeyTask.EnableTimeOutHandler = false;
            mReadKeyAddrTask.EnableTimeOutHandler = false;
        }
        #endregion

        #region 初始化参数
        private void InitTask()
        {
            mKey1Value = 0;
            mKey2Value = 0;


            retryCount = 0;
            writeCount = 0;
            readCount = 0;

            mClearKeyParam.DeviceType = Const.CCU;
            mClearKeyParam.KeyNumber = 0;
            mClearKeyParam.Key1Index = 0;
            mClearKeyParam.Key1Value = new byte[3];
            mClearKeyParam.Key2Index = 1;
            mClearKeyParam.Key2Value = new byte[3];

            mWriteKeyParam.DeviceType = Const.CCU;
            mWriteKeyParam.KeyNumber = KeyNumber;
            mWriteKeyParam.Key1Index = 0;
            mWriteKeyParam.Key1Value = new byte[3];
            mWriteKeyParam.Key2Index = 1;
            mWriteKeyParam.Key2Value = new byte[3];

            mReadKeyAddrParam.DeviceType = Const.CCU;
            mReadKeyAddrParam.KeyNumber = 0XF1;
            mReadKeyAddrParam.Key1Index = 0;
            mReadKeyAddrParam.Key1Value = new byte[3];
            mReadKeyAddrParam.Key2Index = 1;
            mReadKeyAddrParam.Key2Value = new byte[3];

            checkTSTask.ClearAllEvent();
            checkTSStatusTask.ClearAllEvent();
            mClearKeyTask.ClearAllEvent();
            mWriteKeyTask.ClearAllEvent();
            mReadKeyAddrTask.ClearAllEvent();

            checkTSTask.EnableTimeOutHandler = true;
            checkTSStatusTask.EnableTimeOutHandler = true;
            mClearKeyTask.EnableTimeOutHandler = true;
            mWriteKeyTask.EnableTimeOutHandler = true;
            mReadKeyAddrTask.EnableTimeOutHandler = true;
        }
        #endregion

        //执行任务
        public void ExcuteTask()
        {
            
            InitTask();

            SetMainText(this, STEP_LEVEL.WAIT_POWER);
            bTaskRunning = true;
            checkTSTask.Excute();
        }
    }
}
