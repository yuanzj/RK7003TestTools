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
    public partial class Login : Form
    {
        #region 私有变量
        private int com_index { get; set; }
        private int mode_index { get; set; }
        #endregion

        public Login()
        {
            InitializeComponent();
        }

        #region 加载登录界面
        private void LoginForm_Load(object sender, EventArgs e)
        {
            //版本号
            this.Text = String.Format("RK7010板测工具 V{0}", AssemblyFileVersion());
            //串口
            this.ccb_Port.DataSource = System.IO.Ports.SerialPort.GetPortNames();
            //生产模式
            this.ccbMode.Items.Insert(0, "板级测试");
            this.ccbMode.Items.Insert(1, "整机测试");
            this.ccbMode.Items.Insert(2, "复检抽测");

            LoadSetting();

            this.ccbMode.SelectedIndex = this.mode_index;
        }
        #endregion

        #region 加载参数
        private void LoadSetting()
        {
            this.mode_index = int.Parse(ConfigurationManager.AppSettings["Mode_Index"].ToString());
        }
        #endregion

        #region 保存参数
        private void SaveSetting()
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // 修改
            cfa.AppSettings.Settings["mode_index"].Value = this.ccbMode.SelectedIndex.ToString();
            this.mode_index = this.ccbMode.SelectedIndex;           
            // 最后调用当前的配置文件更新成功。
            cfa.Save();
            // 刷新命名节，在下次检索它时将从磁盘重新读取它。记住应用程序要刷新节点
            ConfigurationManager.RefreshSection("appSettings");
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

        #region 关闭登录界面
        private void LoginForm_Closed(object sender, FormClosedEventArgs e)
        {
            SaveSetting();
        }
        #endregion

        #region 点击确认按钮
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            switch (this.ccbMode.SelectedIndex)
            {
                case 0:
                    BoardTest();
                    break;
                case 1:
                    PhoneTest();
                    break;
                case 2:
                    BoardTest();
                    break;
            }
        }
        #endregion

        #region 点击取消按钮
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 板测
        private void BoardTest()
        {
            MainForm mForm = new MainForm((String)this.ccb_Port.SelectedItem, this.ccbMode.SelectedIndex);
            this.Hide();
            if (mForm.ShowDialog() == DialogResult.OK)
            {
                Application.Restart();
            }
            else
            {
                this.Close();
            }
        }
        #endregion

        #region 整机测试
        private void PhoneTest()
        {
            KeysBinding mForm = new KeysBinding((String)this.ccb_Port.SelectedItem);
            this.Hide();
            if (mForm.ShowDialog() == DialogResult.OK)
            {
                Application.Restart();
            }
            else
            {
                this.Close();
            }
        }
        #endregion
    }
}
