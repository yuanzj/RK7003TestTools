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
            mPhoneTask = new PhoneTestTask();
            mPhoneTask.KeyNumber = int.Parse(ConfigurationManager.AppSettings["KeysNumber"].ToString());
        }
        #endregion

        #region 开始任务
        private void StartTask()
        {
            this.listView_Data.Items.Clear();
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
    }
}
