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
                this.Close();
                DynamicPotTicker.Enabled = false;
                this.DialogResult = DialogResult.Cancel;
            });
        }        
    }
}
