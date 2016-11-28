namespace RK7001Test
{
    partial class PhoneTest
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
            this.panelWorkStatus = new System.Windows.Forms.Panel();
            this.labelWorkStatus = new System.Windows.Forms.Label();
            this.textBox_SN = new System.Windows.Forms.TextBox();
            this.labelSN = new System.Windows.Forms.Label();
            this.listView_Data = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.labelItem_SN = new System.Windows.Forms.Label();
            this.labelItemKey1 = new System.Windows.Forms.Label();
            this.labelItemKey2 = new System.Windows.Forms.Label();
            this.label_Tip = new System.Windows.Forms.Label();
            this.panelWorkStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWorkStatus
            // 
            this.panelWorkStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWorkStatus.BackColor = System.Drawing.Color.SkyBlue;
            this.panelWorkStatus.Controls.Add(this.label_Tip);
            this.panelWorkStatus.Controls.Add(this.labelWorkStatus);
            this.panelWorkStatus.Location = new System.Drawing.Point(1, 2);
            this.panelWorkStatus.Name = "panelWorkStatus";
            this.panelWorkStatus.Size = new System.Drawing.Size(739, 418);
            this.panelWorkStatus.TabIndex = 0;
            // 
            // labelWorkStatus
            // 
            this.labelWorkStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWorkStatus.BackColor = System.Drawing.Color.Transparent;
            this.labelWorkStatus.Font = new System.Drawing.Font("新宋体", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelWorkStatus.Location = new System.Drawing.Point(3, 97);
            this.labelWorkStatus.Name = "labelWorkStatus";
            this.labelWorkStatus.Size = new System.Drawing.Size(733, 144);
            this.labelWorkStatus.TabIndex = 0;
            this.labelWorkStatus.Text = "请扫描SN号，进行钥匙绑定！";
            this.labelWorkStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_SN
            // 
            this.textBox_SN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_SN.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_SN.Font = new System.Drawing.Font("黑体", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_SN.Location = new System.Drawing.Point(291, 630);
            this.textBox_SN.MaxLength = 10;
            this.textBox_SN.Name = "textBox_SN";
            this.textBox_SN.Size = new System.Drawing.Size(449, 71);
            this.textBox_SN.TabIndex = 1;
            // 
            // labelSN
            // 
            this.labelSN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSN.Font = new System.Drawing.Font("宋体", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelSN.Location = new System.Drawing.Point(2, 637);
            this.labelSN.Name = "labelSN";
            this.labelSN.Size = new System.Drawing.Size(283, 56);
            this.labelSN.TabIndex = 2;
            this.labelSN.Text = "设备SN号:";
            this.labelSN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listView_Data
            // 
            this.listView_Data.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_Data.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_Data.GridLines = true;
            this.listView_Data.Location = new System.Drawing.Point(1, 426);
            this.listView_Data.Name = "listView_Data";
            this.listView_Data.Size = new System.Drawing.Size(739, 197);
            this.listView_Data.TabIndex = 3;
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
            this.columnHeader2.Text = "错误信息";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "解决方案";
            this.columnHeader3.Width = 350;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.Info;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(746, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(299, 46);
            this.label2.TabIndex = 4;
            this.label2.Text = "测  试  项  目";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelItem_SN
            // 
            this.labelItem_SN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelItem_SN.BackColor = System.Drawing.Color.White;
            this.labelItem_SN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelItem_SN.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelItem_SN.Location = new System.Drawing.Point(746, 68);
            this.labelItem_SN.Name = "labelItem_SN";
            this.labelItem_SN.Size = new System.Drawing.Size(299, 46);
            this.labelItem_SN.TabIndex = 5;
            this.labelItem_SN.Text = "SN号 匹配";
            this.labelItem_SN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelItemKey1
            // 
            this.labelItemKey1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelItemKey1.BackColor = System.Drawing.Color.White;
            this.labelItemKey1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelItemKey1.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelItemKey1.Location = new System.Drawing.Point(746, 130);
            this.labelItemKey1.Name = "labelItemKey1";
            this.labelItemKey1.Size = new System.Drawing.Size(300, 46);
            this.labelItemKey1.TabIndex = 6;
            this.labelItemKey1.Text = "绑定钥匙1";
            this.labelItemKey1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelItemKey2
            // 
            this.labelItemKey2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelItemKey2.BackColor = System.Drawing.Color.White;
            this.labelItemKey2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelItemKey2.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelItemKey2.Location = new System.Drawing.Point(746, 197);
            this.labelItemKey2.Name = "labelItemKey2";
            this.labelItemKey2.Size = new System.Drawing.Size(300, 46);
            this.labelItemKey2.TabIndex = 7;
            this.labelItemKey2.Text = "绑定钥匙2";
            this.labelItemKey2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Tip
            // 
            this.label_Tip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Tip.Font = new System.Drawing.Font("新宋体", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Tip.Location = new System.Drawing.Point(3, 241);
            this.label_Tip.Name = "label_Tip";
            this.label_Tip.Size = new System.Drawing.Size(733, 70);
            this.label_Tip.TabIndex = 1;
            this.label_Tip.Text = "请扫描SN号，进行下一次测试！";
            this.label_Tip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PhoneTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 706);
            this.Controls.Add(this.labelItemKey2);
            this.Controls.Add(this.labelItemKey1);
            this.Controls.Add(this.labelItem_SN);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView_Data);
            this.Controls.Add(this.labelSN);
            this.Controls.Add(this.textBox_SN);
            this.Controls.Add(this.panelWorkStatus);
            this.KeyPreview = true;
            this.Name = "PhoneTest";
            this.Text = "RK7003整测工具V2.0.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PhoneTest_Closed);
            this.Load += new System.EventHandler(this.PhoneTest_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_Start);
            this.panelWorkStatus.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelWorkStatus;
        private System.Windows.Forms.TextBox textBox_SN;
        private System.Windows.Forms.Label labelSN;
        private System.Windows.Forms.ListView listView_Data;
        private System.Windows.Forms.Label labelWorkStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelItem_SN;
        private System.Windows.Forms.Label labelItemKey1;
        private System.Windows.Forms.Label labelItemKey2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label_Tip;
    }
}