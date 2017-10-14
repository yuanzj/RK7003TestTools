using RokyTask;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RK7001Test
{
    public partial class PhoneTest : Form
    {
        private PhoneTestTask mPhoneTask;

        public PhoneTest(string _comPort)
        {
            InitializeComponent();
            Const.COM_PORT = _comPort;
        }

        #region 加载界面
        private void PhoneTest_Load(object sender, EventArgs e)
        {
            //版本号
            this.Text = String.Format("RK4300配对工具 V{0}", AssemblyFileVersion());

            mPhoneTask = new PhoneTestTask();
            mPhoneTask.KeyNumber = int.Parse(ConfigurationManager.AppSettings["KeysNumber"].ToString());

            mPhoneTask.UpdateWorkStatusHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetMainText(mArgs.msg, mArgs.submsg, mArgs.level);
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

            mPhoneTask.ListViewHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetListView(mArgs.msg, mArgs.submsg);
                }
            };

        }
        #endregion

        #region 设置主界面
        delegate void SetMainTextCallback(string msg, string submsg, INFO_LEVEL level);
        private void SetMainText(string msg, string submsg, INFO_LEVEL level)
        {
            if (this.labelWorkStatus.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.labelWorkStatus.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.labelWorkStatus.Disposing || this.labelWorkStatus.IsDisposed)
                        return;
                }
                SetMainTextCallback d = new SetMainTextCallback(SetMainText);
                this.labelWorkStatus.Invoke(d, new object[] { msg, level });
            }
            else
            {
                this.labelWorkStatus.Text = msg;
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.panelWorkStatus.BackColor = Color.LightBlue;
                        this.label_Tip.Text = "";
                        this.textBox_SN.Enabled = true;
                        this.textBox_SN.Text = "";
                        this.textBox_SN.Focus();
                        break;
                    case INFO_LEVEL.PASS:
                        this.panelWorkStatus.BackColor = Color.Green;
                        this.label_Tip.Text = "再次扫描,进行下次测试!";
                        this.textBox_SN.Enabled = true;
                        this.textBox_SN.Text = "";
                        this.textBox_SN.Focus();
                        break;
                    case INFO_LEVEL.FAIL:
                        this.panelWorkStatus.BackColor = Color.Red;
                        this.label_Tip.Text = "再次扫描,进行下次测试!";
                        this.textBox_SN.Enabled = true;
                        this.textBox_SN.Text = "";
                        this.textBox_SN.Focus();
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.panelWorkStatus.BackColor = Color.Yellow;
                        this.label_Tip.Text = submsg;
                        break;
                    case INFO_LEVEL.ONLY_TIP:
                        this.label_Tip.Text = submsg;
                        break;
                }
            }
        }
        #endregion

        #region SN号合法性
        delegate void SetValidSNCallback(INFO_LEVEL level);
        private void SetValidSN(INFO_LEVEL level)
        {
            if (this.labelItem_SN.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.labelItem_SN.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.labelItem_SN.Disposing || this.labelItem_SN.IsDisposed)
                        return;
                }
                SetValidSNCallback d = new SetValidSNCallback(SetValidSN);
                this.labelItem_SN.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.labelItem_SN.BackColor = Color.White;
                        break;
                    case INFO_LEVEL.PASS:
                        this.labelItem_SN.BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.labelItem_SN.BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.labelItem_SN.BackColor = Color.Yellow;
                        break;
                }
            }
        }
        #endregion

        #region 绑定钥匙1
        delegate void SetBindKey1Callback(INFO_LEVEL level);
        private void SetBindKey1(INFO_LEVEL level)
        {
            if (this.labelItemKey1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.labelItemKey1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.labelItemKey1.Disposing || this.labelItemKey1.IsDisposed)
                        return;
                }
                SetBindKey1Callback d = new SetBindKey1Callback(SetBindKey1);
                this.labelItemKey1.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.labelItemKey1.BackColor = Color.White;
                        break;
                    case INFO_LEVEL.PASS:
                        this.labelItemKey1.BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.labelItemKey1.BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.labelItemKey1.BackColor = Color.Yellow;
                        break;
                }
            }
        }
        #endregion

        #region 绑定钥匙2
        delegate void SetBindKey2Callback(INFO_LEVEL level);
        private void SetBindKey2(INFO_LEVEL level)
        {
            if (this.labelItemKey2.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.labelItemKey2.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.labelItemKey2.Disposing || this.labelItemKey2.IsDisposed)
                        return;
                }
                SetBindKey2Callback d = new SetBindKey2Callback(SetBindKey2);
                this.labelItemKey2.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.labelItemKey2.BackColor = Color.White;
                        break;
                    case INFO_LEVEL.PASS:
                        this.labelItemKey2.BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.labelItemKey2.BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.labelItemKey2.BackColor = Color.Yellow;
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

        #region 开始任务
        private void StartTask()
        {
            this.listView_Data.Items.Clear();
            SetValidSN(INFO_LEVEL.PROCESS);
            if(mPhoneTask.KeyNumber == 1)
            {
                SetBindKey1(INFO_LEVEL.PROCESS);
                SetBindKey2(INFO_LEVEL.GREY);
            }
            else if(mPhoneTask.KeyNumber == 2)
            {
                SetBindKey1(INFO_LEVEL.PROCESS);
                SetBindKey2(INFO_LEVEL.PROCESS);
            }
            mPhoneTask.mSN = this.textBox_SN.Text;
            this.textBox_SN.Enabled = false;
            mPhoneTask.ExcuteTask();
        }
        #endregion

        #region
        private void PhoneTest_Closed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region
        private void KeyDown_Start(object sender, KeyEventArgs e)
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

        #region 获得版本号
        private string AssemblyFileVersion()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            else
            {
                return ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }
        }
        #endregion
    }
}
