using RokyTask;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace RK7001Test
{
    public partial class KeysBinding : Form
    {
        private PhoneTestTask mPhoneTask;
        System.Timers.Timer TestTimeTicker;
        private int TimeCounts { get; set; }

        public KeysBinding(string _comPort)
        {
            InitializeComponent();
            Const.COM_PORT = _comPort;
        }


        #region 加载界面
        private void KeyBindForm_Load(object sender, EventArgs e)
        {
            mPhoneTask = new PhoneTestTask();
            mPhoneTask.KeyNumber = int.Parse(ConfigurationManager.AppSettings["KeysNumber"].ToString());
            

            mPhoneTask.UpdateWorkStatusHandler += (object _sender, EventArgs _e) =>
            {
                StepArgs mArgs = _e as StepArgs;
                if (mArgs != null)
                {
                    SetMainText(mArgs.level);
                }
            };

            mPhoneTask.UpdateValidSNHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetValidSN(mArgs.level);
                }
            };

            mPhoneTask.BindKey1Handler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetBindKey1(mArgs.level);
                }
            };

            mPhoneTask.BindKey2Handler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetBindKey2(mArgs.level);
                }
            };

            mPhoneTask.CheckKey1Handler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetCheckKey1(mArgs.level);
                }
            };

            mPhoneTask.CheckKey2Handler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetCheckKey2(mArgs.level);
                }
            };

            mPhoneTask.ClearKeyHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetClearKey(mArgs.level);
                }
            };

            mPhoneTask.ListViewHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetListView(mArgs.msg, mArgs.submsg);
                }
            };

            mPhoneTask.KeyValueHandler += (object _sender, EventArgs _e) =>
            {
                KeyValueArgs mArgs = _e as KeyValueArgs;
                if (mArgs != null)
                {
                    SetKeyValue(mArgs.type, mArgs.value);
                }
            };

            TestTimeTicker = new System.Timers.Timer(1000);
            TestTimeTicker.Enabled = false;
            TestTimeTicker.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs ElapsedEventArgs) =>
            {
                TimeCounts++;
                SetCountNum(TimeCounts);
            });
           
            //设置界面            
            if (mPhoneTask.KeyNumber < 1 || mPhoneTask.KeyNumber > 2)
            {
                MessageBox.Show("绑定钥匙至少1把，最多2把！");
            }
            else
            {
                if(mPhoneTask.KeyNumber == 1)
                {
                    this.panel_BindKey2.Visible = false;
                    this.panel_CheckKey2.Visible = false;
                }
                else if(mPhoneTask.KeyNumber == 2)
                {
                    this.panel_BindKey2.Visible = true;
                    this.panel_CheckKey2.Visible = true;
                }
            }

            SetMainText(STEP_LEVEL.NONE);
            SetValidSN(INFO_LEVEL.INIT);
            SetBindKey1(INFO_LEVEL.INIT);
            SetCheckKey1(INFO_LEVEL.INIT);
            SetBindKey2(INFO_LEVEL.INIT);
            SetCheckKey2(INFO_LEVEL.INIT);
            SetClearKey(INFO_LEVEL.INIT);
            SetKeyValue(KeyType.NONE_KEY, 0);
        }
        #endregion

        #region 设置主界面
        delegate void SetMainTextCallback(STEP_LEVEL level);
        private void SetMainText(STEP_LEVEL level)
        {
            if (this.label_MainResult.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.label_MainResult.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.label_MainResult.Disposing || this.label_MainResult.IsDisposed)
                        return;
                }
                SetMainTextCallback d = new SetMainTextCallback(SetMainText);
                this.label_MainResult.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case STEP_LEVEL.NONE:
                        this.panel_MainResult.BackColor = Color.LightBlue;
                        this.label_MainTip.Text = "扫描设备,进行测试";
                        this.label_MainResult.Text = "开  始  测  试";
                        break;
                    case STEP_LEVEL.WAIT_POWER:
                        this.panel_MainResult.BackColor = Color.Yellow;
                        this.label_MainResult.Text = "等待设备上电...";
                        this.label_MainTip.Text = "";
                        break;
                    case STEP_LEVEL.PASS:
                        this.TestTimeTicker.Enabled = false;
                        this.panel_MainResult.BackColor = Color.Green;
                        this.label_MainResult.Text = String.Format("{0}\n成  功", mPhoneTask.mSN);
                        this.label_MainTip.Text = "再次扫描,进行下一次测试!";
                        this.textBox_SN.Enabled = true;
                        this.textBox_SN.Text = "";
                        this.textBox_SN.Focus();
                        break;
                    case STEP_LEVEL.CHECK_SN:
                        this.panel_MainResult.BackColor = Color.Yellow;
                        this.label_MainResult.Text = "SN号检查...";
                        this.label_MainTip.Text = "";
                        break;
                    case STEP_LEVEL.BIND_KEY1:
                        this.panel_MainResult.BackColor = Color.Yellow;
                        this.label_MainResult.Text = "绑定第一把钥匙...";
                        this.label_MainTip.Text = "";
                        break;
                    case STEP_LEVEL.BIND_KEY2:
                        this.panel_MainResult.BackColor = Color.Yellow;
                        this.label_MainResult.Text = "绑定第二把钥匙...";
                        this.label_MainTip.Text = "";
                        break;
                    case STEP_LEVEL.FAIL:
                        this.TestTimeTicker.Enabled = false;
                        this.panel_MainResult.BackColor = Color.Red;
                        this.label_MainResult.Text = String.Format("{0}\n失  败", mPhoneTask.mSN);
                        this.label_MainTip.Text = "再次扫描,进行下一次测试!";
                        this.textBox_SN.Enabled = true;
                        this.textBox_SN.Text = "";
                        this.textBox_SN.Focus();
                        break;                                           
                                       
                }
            }
        }
        #endregion

        #region 计时
        delegate void SetCountNumCallback(int counts);
        private void SetCountNum(int counts)
        {
            if (this.label_TimeCount.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.label_TimeCount.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.label_TimeCount.Disposing || this.label_TimeCount.IsDisposed)
                        return;
                }
                SetCountNumCallback d = new SetCountNumCallback(SetCountNum);
                this.label_TimeCount.Invoke(d, new object[] { counts });
            }
            else
            {
                string value = String.Format("{0}", counts);
                this.label_TimeCount.Text = value;
            }
        }
        #endregion

        #region SN号合法性
        delegate void SetValidSNCallback(INFO_LEVEL level);
        private void SetValidSN(INFO_LEVEL level)
        {
            if (this.pictureBox_SN.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.pictureBox_SN.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.pictureBox_SN.Disposing || this.pictureBox_SN.IsDisposed)
                        return;
                }
                SetValidSNCallback d = new SetValidSNCallback(SetValidSN);
                this.pictureBox_SN.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.pictureBox_SN.Visible = false;
                        break;
                    case INFO_LEVEL.PASS:
                        this.pictureBox_SN.Visible = true;
                        this.pictureBox_SN.Image = global::RK7001Test.Properties.Resources.OK;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.pictureBox_SN.Visible = true;
                        this.pictureBox_SN.Image = global::RK7001Test.Properties.Resources.Shape;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.pictureBox_SN.Visible = true;
                        this.pictureBox_SN.Image = global::RK7001Test.Properties.Resources.ic_loading;
                        break;
                }
            }
        }
        #endregion

        #region 绑定钥匙1
        delegate void SetBindKey1Callback(INFO_LEVEL level);
        private void SetBindKey1(INFO_LEVEL level)
        {
            if (this.pictureBox_BindKey1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.pictureBox_BindKey1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.pictureBox_BindKey1.Disposing || this.pictureBox_BindKey1.IsDisposed)
                        return;
                }
                SetBindKey1Callback d = new SetBindKey1Callback(SetBindKey1);
                this.pictureBox_BindKey1.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.pictureBox_BindKey1.Visible = false;
                        break;
                    case INFO_LEVEL.PASS:
                        this.pictureBox_BindKey1.Visible = true;
                        this.pictureBox_BindKey1.Image = global::RK7001Test.Properties.Resources.OK;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.pictureBox_BindKey1.Visible = true;
                        this.pictureBox_BindKey1.Image = global::RK7001Test.Properties.Resources.Shape;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.pictureBox_BindKey1.Visible = true;
                        this.pictureBox_BindKey1.Image = global::RK7001Test.Properties.Resources.ic_loading;
                        break;
                }
            }
        }
        #endregion

        #region 检查钥匙1
        delegate void SetCheckKey1Callback(INFO_LEVEL level);
        private void SetCheckKey1(INFO_LEVEL level)
        {
            if (this.pictureBox_CheckKey1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.pictureBox_CheckKey1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.pictureBox_CheckKey1.Disposing || this.pictureBox_CheckKey1.IsDisposed)
                        return;
                }
                SetCheckKey1Callback d = new SetCheckKey1Callback(SetCheckKey1);
                this.pictureBox_CheckKey1.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.pictureBox_CheckKey1.Visible = false;
                        break;
                    case INFO_LEVEL.PASS:
                        this.pictureBox_CheckKey1.Visible = true;
                        this.pictureBox_CheckKey1.Image = global::RK7001Test.Properties.Resources.OK;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.pictureBox_CheckKey1.Visible = true;
                        this.pictureBox_CheckKey1.Image = global::RK7001Test.Properties.Resources.Shape;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.pictureBox_CheckKey1.Visible = true;
                        this.pictureBox_CheckKey1.Image = global::RK7001Test.Properties.Resources.ic_loading;
                        break;
                }
            }
        }
        #endregion

        #region 绑定钥匙2
        delegate void SetBindKey2Callback(INFO_LEVEL level);
        private void SetBindKey2(INFO_LEVEL level)
        {
            if (this.pictureBox_BindKey2.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.pictureBox_BindKey2.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.pictureBox_BindKey2.Disposing || this.pictureBox_BindKey2.IsDisposed)
                        return;
                }
                SetBindKey2Callback d = new SetBindKey2Callback(SetBindKey2);
                this.pictureBox_BindKey2.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.pictureBox_BindKey2.Visible = false;
                        break;
                    case INFO_LEVEL.PASS:
                        this.pictureBox_BindKey2.Visible = true;
                        this.pictureBox_BindKey2.Image = global::RK7001Test.Properties.Resources.OK;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.pictureBox_BindKey2.Visible = true;
                        this.pictureBox_BindKey2.Image = global::RK7001Test.Properties.Resources.Shape;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.pictureBox_BindKey2.Visible = true;
                        this.pictureBox_BindKey2.Image = global::RK7001Test.Properties.Resources.ic_loading;
                        break;

                }
            }
        }
        #endregion

        #region 检查钥匙2
        delegate void SetCheckKey2Callback(INFO_LEVEL level);
        private void SetCheckKey2(INFO_LEVEL level)
        {
            if (this.pictureBox_CheckKey2.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.pictureBox_CheckKey2.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.pictureBox_CheckKey2.Disposing || this.pictureBox_CheckKey2.IsDisposed)
                        return;
                }
                SetCheckKey2Callback d = new SetCheckKey2Callback(SetCheckKey2);
                this.pictureBox_CheckKey2.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.pictureBox_CheckKey2.Visible = false;
                        break;
                    case INFO_LEVEL.PASS:
                        this.pictureBox_CheckKey2.Visible = true;
                        this.pictureBox_CheckKey2.Image = global::RK7001Test.Properties.Resources.OK;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.pictureBox_CheckKey2.Visible = true;
                        this.pictureBox_CheckKey2.Image = global::RK7001Test.Properties.Resources.Shape;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.pictureBox_CheckKey2.Visible = true;
                        this.pictureBox_CheckKey2.Image = global::RK7001Test.Properties.Resources.ic_loading;
                        break;                   
                }
            }
        }
        #endregion

        #region 设置清除钥匙
        delegate void SetClearKeyCallback(INFO_LEVEL level);
        private void SetClearKey(INFO_LEVEL level)
        {
            if (this.pictureBox_ClearKey.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.pictureBox_ClearKey.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.pictureBox_ClearKey.Disposing || this.pictureBox_ClearKey.IsDisposed)
                        return;
                }
                SetClearKeyCallback d = new SetClearKeyCallback(SetClearKey);
                this.pictureBox_ClearKey.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.pictureBox_ClearKey.Visible = false;
                        break;
                    case INFO_LEVEL.PASS:
                        this.pictureBox_ClearKey.Visible = true;
                        this.pictureBox_ClearKey.Image = global::RK7001Test.Properties.Resources.OK;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.pictureBox_ClearKey.Visible = true;
                        this.pictureBox_ClearKey.Image = global::RK7001Test.Properties.Resources.Shape;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.pictureBox_ClearKey.Visible = true;
                        this.pictureBox_ClearKey.Image = global::RK7001Test.Properties.Resources.ic_loading;
                        break;
                }
            }
        }
        #endregion

        #region ListView
        delegate void SetListViewCallback(string msg, string submsg);
        private void SetListView(string msg, string submsg)
        {
            if (this.listView_Data.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.listView_Data.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.listView_Data.Disposing || this.listView_Data.IsDisposed)
                        return;
                }
                SetListViewCallback d = new SetListViewCallback(SetListView);
                this.listView_Data.Invoke(d, new object[] { msg, submsg });
            }
            else
            {
                this.listView_Data.BeginUpdate();
                ListViewItem lvi = new ListViewItem();
                lvi.Text = DateTime.Now.ToString("HH:mm:ss:fff");
                lvi.SubItems.Add(msg);
                lvi.SubItems.Add(submsg);
                this.listView_Data.Items.Add(lvi);
                //总是显示最后一行
                this.listView_Data.Items[this.listView_Data.Items.Count - 1].EnsureVisible();
                this.listView_Data.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            }
        }
        #endregion

        #region 钥匙地址码
        delegate void SetKeyValueCallback(KeyType type, int msg);
        private void SetKeyValue(KeyType type, int msg)
        {
            if (this.label_Key1Value.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.label_Key1Value.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.label_Key1Value.Disposing || this.label_Key1Value.IsDisposed)
                        return;
                }
                SetKeyValueCallback d = new SetKeyValueCallback(SetKeyValue);
                this.label_Key1Value.Invoke(d, new object[] { type, msg });
            }
            else
            {
                switch(type)
                {
                    case KeyType.NONE_KEY:
                        this.label_Key1Value.Text = "";
                        this.label_Key1Check.Text = "";
                        this.label_Key2Value.Text = "";
                        this.label_Key2Check.Text = "";
                        break;
                    case KeyType.BIND_KEY1:
                        this.label_Key1Value.Text = msg.ToString();
                        break;
                    case KeyType.BIND_KEY2:
                        this.label_Key2Value.Text = msg.ToString();
                        break;
                    case KeyType.CHECK_KEY1:
                        this.label_Key1Check.Text = msg.ToString();
                        break;
                    case KeyType.CHECK_KEY2:
                        this.label_Key2Check.Text = msg.ToString();
                        break;
                }
            }
        }
        #endregion



        #region 执行任务
        private void StartTask()
        {
            TimeCounts = 0;
            this.listView_Data.Items.Clear();
            SetValidSN(INFO_LEVEL.PROCESS);
            if (mPhoneTask.KeyNumber == 1)
            {
                SetValidSN(INFO_LEVEL.PROCESS);
                SetBindKey1(INFO_LEVEL.PROCESS);
                SetCheckKey1(INFO_LEVEL.PROCESS);
                SetClearKey(INFO_LEVEL.PROCESS);
                this.pictureBox_BindKey2.Visible = false;
                this.pictureBox_CheckKey2.Visible = false;             
            }
            else if (mPhoneTask.KeyNumber == 2)
            {
                SetValidSN(INFO_LEVEL.PROCESS);
                SetClearKey(INFO_LEVEL.PROCESS);
                SetBindKey1(INFO_LEVEL.PROCESS);
                SetCheckKey1(INFO_LEVEL.PROCESS);
                SetBindKey2(INFO_LEVEL.PROCESS);
                SetCheckKey2(INFO_LEVEL.PROCESS);
            }

            SetKeyValue(KeyType.NONE_KEY, 0);
            mPhoneTask.mSN = this.textBox_SN.Text;
            this.textBox_SN.Enabled = false;
            this.TestTimeTicker.Enabled = true;
            mPhoneTask.ExcuteTask();
        }
        #endregion

        #region 界面关闭
        private void KeyBindForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 开始执行任务
        private void KeyBindForm_Start(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (mPhoneTask.bTaskRunning)
                        return;
                    if (this.textBox_SN.TextLength == 10)
                        StartTask();
                }
            }
        }
        #endregion
    }
}
