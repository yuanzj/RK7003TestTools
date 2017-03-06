using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace RK7001Test
{
    public partial class ManualVisio : Form
    {
        private System.Timers.Timer DynamicPotTicker;
        public ManualVisio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void KeyDown_Checked(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
            }
            else if(e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        //界面的加载
        private void MainViso_Load(object sender, EventArgs e)
        {
            DynamicPotTicker = new System.Timers.Timer(1000*10);
            DynamicPotTicker.Enabled = true;
            DynamicPotTicker.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs ElapsedEventArgs) =>
            {

                MainVisoClose();
                DynamicPotTicker.Enabled = false;
                this.DialogResult = DialogResult.Cancel;
            });
        }

        #region 关闭界面
        delegate void MainVisoCloseCallback();
        private void MainVisoClose()
        {
            if (this.panel1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.panel1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.panel1.Disposing || this.panel1.IsDisposed)
                        return;
                }
                MainVisoCloseCallback d = new MainVisoCloseCallback(MainVisoClose);
                this.panel1.Invoke(d, new object[] {  });
            }
            else
            {
                this.Close();
            }
        }
        #endregion
    }
}
