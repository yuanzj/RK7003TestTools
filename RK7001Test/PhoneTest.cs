﻿using RokyTask;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public PhoneTest(string _comPort)
        {
            InitializeComponent();
            Const.COM_PORT = _comPort;
        }

        #region 加载界面
        private void PhoneTest_Load(object sender, EventArgs e)
        {

        }
        #endregion


    }
}