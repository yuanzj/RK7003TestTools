namespace RK7001Test
{
    partial class KeysBinding
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_MainResult = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_SN = new System.Windows.Forms.TextBox();
            this.listView_Data = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel_SN = new System.Windows.Forms.Panel();
            this.pictureBox_SN = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel_BindKey1 = new System.Windows.Forms.Panel();
            this.label_Key1Value = new System.Windows.Forms.Label();
            this.pictureBox_BindKey1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel_BindKey2 = new System.Windows.Forms.Panel();
            this.label_Key2Value = new System.Windows.Forms.Label();
            this.pictureBox_BindKey2 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel_WriteNV = new System.Windows.Forms.Panel();
            this.label_Key2Check = new System.Windows.Forms.Label();
            this.pictureBox_WriteNV = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panel_MainResult = new System.Windows.Forms.Panel();
            this.label_MainTip = new System.Windows.Forms.Label();
            this.label_TimeCount = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel_SN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SN)).BeginInit();
            this.panel_BindKey1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BindKey1)).BeginInit();
            this.panel_BindKey2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BindKey2)).BeginInit();
            this.panel_WriteNV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_WriteNV)).BeginInit();
            this.panel_MainResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_MainResult
            // 
            this.label_MainResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_MainResult.BackColor = System.Drawing.Color.Transparent;
            this.label_MainResult.Font = new System.Drawing.Font("宋体", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_MainResult.Location = new System.Drawing.Point(3, 0);
            this.label_MainResult.Name = "label_MainResult";
            this.label_MainResult.Size = new System.Drawing.Size(582, 384);
            this.label_MainResult.TabIndex = 4;
            this.label_MainResult.Text = "成功";
            this.label_MainResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(-1, 706);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 64);
            this.label6.TabIndex = 5;
            this.label6.Text = "SN号:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_SN
            // 
            this.textBox_SN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_SN.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_SN.Font = new System.Drawing.Font("宋体", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_SN.Location = new System.Drawing.Point(106, 704);
            this.textBox_SN.MaxLength = 10;
            this.textBox_SN.Name = "textBox_SN";
            this.textBox_SN.Size = new System.Drawing.Size(498, 62);
            this.textBox_SN.TabIndex = 7;
            // 
            // listView_Data
            // 
            this.listView_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_Data.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_Data.GridLines = true;
            this.listView_Data.Location = new System.Drawing.Point(6, 539);
            this.listView_Data.Name = "listView_Data";
            this.listView_Data.Size = new System.Drawing.Size(592, 159);
            this.listView_Data.TabIndex = 8;
            this.listView_Data.UseCompatibleStateImageBehavior = false;
            this.listView_Data.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "时间";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "结果";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "解决方案";
            this.columnHeader3.Width = 350;
            // 
            // panel_SN
            // 
            this.panel_SN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_SN.BackColor = System.Drawing.SystemColors.Window;
            this.panel_SN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_SN.Controls.Add(this.pictureBox_SN);
            this.panel_SN.Controls.Add(this.label2);
            this.panel_SN.Location = new System.Drawing.Point(605, 43);
            this.panel_SN.Name = "panel_SN";
            this.panel_SN.Size = new System.Drawing.Size(491, 70);
            this.panel_SN.TabIndex = 13;
            // 
            // pictureBox_SN
            // 
            this.pictureBox_SN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_SN.Image = global::RK7001Test.Properties.Resources.ic_loading;
            this.pictureBox_SN.Location = new System.Drawing.Point(413, 7);
            this.pictureBox_SN.Name = "pictureBox_SN";
            this.pictureBox_SN.Size = new System.Drawing.Size(65, 54);
            this.pictureBox_SN.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_SN.TabIndex = 1;
            this.pictureBox_SN.TabStop = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(6, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(328, 54);
            this.label2.TabIndex = 0;
            this.label2.Text = "设备SN号与标签SN号匹配";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_BindKey1
            // 
            this.panel_BindKey1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_BindKey1.BackColor = System.Drawing.SystemColors.Window;
            this.panel_BindKey1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_BindKey1.Controls.Add(this.label_Key1Value);
            this.panel_BindKey1.Controls.Add(this.pictureBox_BindKey1);
            this.panel_BindKey1.Controls.Add(this.label3);
            this.panel_BindKey1.Location = new System.Drawing.Point(605, 129);
            this.panel_BindKey1.Name = "panel_BindKey1";
            this.panel_BindKey1.Size = new System.Drawing.Size(492, 70);
            this.panel_BindKey1.TabIndex = 14;
            // 
            // label_Key1Value
            // 
            this.label_Key1Value.Font = new System.Drawing.Font("新宋体", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Key1Value.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label_Key1Value.Location = new System.Drawing.Point(298, 15);
            this.label_Key1Value.Name = "label_Key1Value";
            this.label_Key1Value.Size = new System.Drawing.Size(112, 39);
            this.label_Key1Value.TabIndex = 2;
            this.label_Key1Value.Text = "025686";
            this.label_Key1Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_BindKey1
            // 
            this.pictureBox_BindKey1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_BindKey1.Image = global::RK7001Test.Properties.Resources.OK;
            this.pictureBox_BindKey1.Location = new System.Drawing.Point(414, 7);
            this.pictureBox_BindKey1.Name = "pictureBox_BindKey1";
            this.pictureBox_BindKey1.Size = new System.Drawing.Size(65, 54);
            this.pictureBox_BindKey1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_BindKey1.TabIndex = 2;
            this.pictureBox_BindKey1.TabStop = false;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(306, 54);
            this.label3.TabIndex = 1;
            this.label3.Text = "绑定第一把钥匙";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_BindKey2
            // 
            this.panel_BindKey2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_BindKey2.BackColor = System.Drawing.SystemColors.Window;
            this.panel_BindKey2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_BindKey2.Controls.Add(this.label7);
            this.panel_BindKey2.Controls.Add(this.label_Key2Value);
            this.panel_BindKey2.Controls.Add(this.pictureBox_BindKey2);
            this.panel_BindKey2.Location = new System.Drawing.Point(605, 216);
            this.panel_BindKey2.Name = "panel_BindKey2";
            this.panel_BindKey2.Size = new System.Drawing.Size(491, 70);
            this.panel_BindKey2.TabIndex = 16;
            // 
            // label_Key2Value
            // 
            this.label_Key2Value.Font = new System.Drawing.Font("新宋体", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Key2Value.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label_Key2Value.Location = new System.Drawing.Point(298, 23);
            this.label_Key2Value.Name = "label_Key2Value";
            this.label_Key2Value.Size = new System.Drawing.Size(112, 23);
            this.label_Key2Value.TabIndex = 4;
            this.label_Key2Value.Text = "025686";
            this.label_Key2Value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_BindKey2
            // 
            this.pictureBox_BindKey2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_BindKey2.Image = global::RK7001Test.Properties.Resources.ic_loading;
            this.pictureBox_BindKey2.Location = new System.Drawing.Point(413, 4);
            this.pictureBox_BindKey2.Name = "pictureBox_BindKey2";
            this.pictureBox_BindKey2.Size = new System.Drawing.Size(65, 57);
            this.pictureBox_BindKey2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_BindKey2.TabIndex = 4;
            this.pictureBox_BindKey2.TabStop = false;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(8, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(261, 54);
            this.label7.TabIndex = 2;
            this.label7.Text = "绑定第二把钥匙";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_WriteNV
            // 
            this.panel_WriteNV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_WriteNV.BackColor = System.Drawing.SystemColors.Window;
            this.panel_WriteNV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_WriteNV.Controls.Add(this.label_Key2Check);
            this.panel_WriteNV.Controls.Add(this.pictureBox_WriteNV);
            this.panel_WriteNV.Controls.Add(this.label8);
            this.panel_WriteNV.Location = new System.Drawing.Point(605, 302);
            this.panel_WriteNV.Name = "panel_WriteNV";
            this.panel_WriteNV.Size = new System.Drawing.Size(492, 70);
            this.panel_WriteNV.TabIndex = 17;
            // 
            // label_Key2Check
            // 
            this.label_Key2Check.Font = new System.Drawing.Font("新宋体", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Key2Check.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label_Key2Check.Location = new System.Drawing.Point(297, 23);
            this.label_Key2Check.Name = "label_Key2Check";
            this.label_Key2Check.Size = new System.Drawing.Size(102, 23);
            this.label_Key2Check.TabIndex = 5;
            this.label_Key2Check.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_WriteNV
            // 
            this.pictureBox_WriteNV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_WriteNV.Image = global::RK7001Test.Properties.Resources.OK;
            this.pictureBox_WriteNV.Location = new System.Drawing.Point(414, 5);
            this.pictureBox_WriteNV.Name = "pictureBox_WriteNV";
            this.pictureBox_WriteNV.Size = new System.Drawing.Size(70, 56);
            this.pictureBox_WriteNV.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_WriteNV.TabIndex = 5;
            this.pictureBox_WriteNV.TabStop = false;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(9, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(248, 54);
            this.label8.TabIndex = 3;
            this.label8.Text = "写NV到设备";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(599, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(500, 24);
            this.label1.TabIndex = 18;
            this.label1.Text = "----------------执行步骤--------------";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(619, 717);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(142, 44);
            this.label12.TabIndex = 19;
            this.label12.Text = "本次耗时:";
            // 
            // panel_MainResult
            // 
            this.panel_MainResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_MainResult.BackColor = System.Drawing.Color.Green;
            this.panel_MainResult.Controls.Add(this.label_MainTip);
            this.panel_MainResult.Controls.Add(this.label_MainResult);
            this.panel_MainResult.Location = new System.Drawing.Point(10, 2);
            this.panel_MainResult.Name = "panel_MainResult";
            this.panel_MainResult.Size = new System.Drawing.Size(588, 531);
            this.panel_MainResult.TabIndex = 21;
            // 
            // label_MainTip
            // 
            this.label_MainTip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_MainTip.Font = new System.Drawing.Font("新宋体", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_MainTip.Location = new System.Drawing.Point(2, 369);
            this.label_MainTip.Name = "label_MainTip";
            this.label_MainTip.Size = new System.Drawing.Size(582, 89);
            this.label_MainTip.TabIndex = 5;
            this.label_MainTip.Text = "再次扫描,进行下次测试!";
            this.label_MainTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_TimeCount
            // 
            this.label_TimeCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label_TimeCount.Font = new System.Drawing.Font("宋体", 21.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_TimeCount.ForeColor = System.Drawing.Color.Maroon;
            this.label_TimeCount.Location = new System.Drawing.Point(779, 720);
            this.label_TimeCount.Name = "label_TimeCount";
            this.label_TimeCount.Size = new System.Drawing.Size(69, 35);
            this.label_TimeCount.TabIndex = 22;
            this.label_TimeCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("新宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(868, 725);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 29);
            this.label13.TabIndex = 23;
            this.label13.Text = "秒";
            // 
            // KeysBinding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 772);
            this.Controls.Add(this.panel_BindKey2);
            this.Controls.Add(this.panel_BindKey1);
            this.Controls.Add(this.panel_SN);
            this.Controls.Add(this.panel_WriteNV);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label_TimeCount);
            this.Controls.Add(this.panel_MainResult);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView_Data);
            this.Controls.Add(this.textBox_SN);
            this.Controls.Add(this.label6);
            this.KeyPreview = true;
            this.Name = "KeysBinding";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RK7003整测工具V2.0.6";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KeyBindForm_Closed);
            this.Load += new System.EventHandler(this.KeyBindForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyBindForm_Start);
            this.panel_SN.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SN)).EndInit();
            this.panel_BindKey1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BindKey1)).EndInit();
            this.panel_BindKey2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BindKey2)).EndInit();
            this.panel_WriteNV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_WriteNV)).EndInit();
            this.panel_MainResult.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_MainResult;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_SN;
        private System.Windows.Forms.ListView listView_Data;
        private System.Windows.Forms.Panel panel_SN;
        private System.Windows.Forms.Panel panel_BindKey1;
        private System.Windows.Forms.Panel panel_BindKey2;
        private System.Windows.Forms.Panel panel_WriteNV;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox pictureBox_SN;
        private System.Windows.Forms.PictureBox pictureBox_BindKey1;
        private System.Windows.Forms.PictureBox pictureBox_BindKey2;
        private System.Windows.Forms.PictureBox pictureBox_WriteNV;
        private System.Windows.Forms.Panel panel_MainResult;
        private System.Windows.Forms.Label label_MainTip;
        private System.Windows.Forms.Label label_TimeCount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label_Key1Value;
        private System.Windows.Forms.Label label_Key2Value;
        private System.Windows.Forms.Label label_Key2Check;
    }
}