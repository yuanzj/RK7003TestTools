using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        #endregion

        public Login()
        {
            InitializeComponent();
        }

        #region 加载登录界面
        private void LoginForm_Load(object sender, EventArgs e)
        {
            //版本号
            this.Text = String.Format("新日RK7003工具 V{0}", AssemblyFileVersion());
            //串口
            this.ccb_Port.DataSource = System.IO.Ports.SerialPort.GetPortNames();

            LoadSetting();
        }
        #endregion

        #region 加载参数
        private void LoadSetting()
        {
            
        }
        #endregion

        #region 保存参数
        private void SaveSetting()
        {

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
            MainForm mForm = new MainForm((String)this.ccb_Port.SelectedItem);
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

        #region 点击取消按钮
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 点击设置按钮
        private void btn_Settins_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
