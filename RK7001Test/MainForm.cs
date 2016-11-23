using CommonUtils;
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
    public partial class MainForm : Form
    {
        #region 私有变量
        private CoreBussinessTask mCoreTask;
        private Label[] mRK7001Pin;
        private Label[] mRK4103Pin;
        private ListViewItem lv7001uid;
        private ListViewItem lv7001sw;
        private ListViewItem lv7001hw;
        private ListViewItem lv7001ampf;
        private ListViewItem lv7001mosfet;
        private ListViewItem lv7001buzzer;
        private ListViewItem lv4103sw;
        private ListViewItem lv4103lgtsensor;
        private ListViewItem lv4103gsensor;
        //private ListViewItem lv4103pwm;
        private ListViewItem lv4103ble;
        #endregion

        public MainForm(string _comPort)
        {
            InitializeComponent();
            Const.COM_PORT = _comPort;
            
            mRK7001Pin = new Label[] { this.lbPin_1, this.lbPin_2, this.lbPin_3, this.lbPin_4, this.lbPin_5, this.lbPin_6, this.lbPin_7, this.lbPin_8, this.lbPin_9, this.lbPin_10,
                                        this.lbPin_11, this.lbPin_12, this.lbPin_13, this.lbPin_14, this.lbPin_15, this.lbPin_16, this.lbPin_17, this.lbPin_18, this.lbPin_19, this.lbPin_20,
                                       this.lbPin_21, this.lbPin_22, this.lbPin_23, this.lbPin_24, this.lbPin_25, this.lbPin_26, this.lbPin_27, this.lbPin_28};
            mRK4103Pin = new Label[] { this.rkPin_1, this.rkPin_2, this.rkPin_3, this.rkPin_4, this.rkPin_5, this.rkPin_6, this.rkPin_7, this.rkPin_8, this.rkPin_9, this.rkPin_10,
                                        this.rkPin_11, this.rkPin_12, this.rkPin_13, this.rkPin_14, this.rkPin_15, this.rkPin_16, this.rkPin_17, this.rkPin_18, this.rkPin_19, this.rkPin_20,
                                        this.rkPin_21, this.rkPin_22, this.rkPin_23, this.rkPin_24, this.rkPin_25, this.rkPin_26, this.rkPin_27, this.rkPin_28, this.rkPin_29, this.rkPin_30};        
        }

        #region 加载界面
        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Listview初始化
            lv7001uid = new ListViewItem("设备UID");
            lv7001sw = new ListViewItem("软件版本号");
            lv7001hw = new ListViewItem("硬件版本号");
            lv7001ampf = new ListViewItem("运放故障");
            lv7001mosfet = new ListViewItem("Mos短路");
            lv7001buzzer = new ListViewItem("Buzzer故障");

            lv7001uid.UseItemStyleForSubItems = false;
            lv7001sw.UseItemStyleForSubItems = false;
            lv7001hw.UseItemStyleForSubItems = false;
            lv7001ampf.UseItemStyleForSubItems = false;
            lv7001mosfet.UseItemStyleForSubItems = false;
            lv7001buzzer.UseItemStyleForSubItems = false;

            this.lvRK7001ErrItem.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            lv7001uid,
            lv7001sw,
            lv7001hw,
            lv7001ampf,
            lv7001mosfet,
            lv7001buzzer});

            lv4103sw = new ListViewItem("软件版本号");
            lv4103lgtsensor = new ListViewItem("光感异常测试");
            lv4103gsensor = new ListViewItem("Gsensor测试");
            lv4103ble = new ListViewItem("蓝牙4.0测试");

            lv4103sw.UseItemStyleForSubItems = false;
            lv4103lgtsensor.UseItemStyleForSubItems = false;
            lv4103gsensor.UseItemStyleForSubItems = false;
            lv4103ble.UseItemStyleForSubItems = false;

            this.lvRK4003ErrItem.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            lv4103sw,
            lv4103lgtsensor,
            lv4103gsensor,
            lv4103ble});
            #endregion

            UpdateVddGpsPin(INFO_LEVEL.INIT);

            mCoreTask = new CoreBussinessTask();

            mCoreTask.mSqlserverConnString = "Provider=SQLOLEDB.1; Persist Security Info=False; Data Source=" + ConfigurationManager.AppSettings["ServerName"].ToString()
                                            + "; Initial Catalog=" + ConfigurationManager.AppSettings["DataBase"].ToString()
                                            + "; User ID=" + ConfigurationManager.AppSettings["User"].ToString()
                                            + "; PassWord=" + ConfigurationManager.AppSettings["Password"].ToString();
            
            mCoreTask.bCruised = Convert.ToBoolean(ConfigurationManager.AppSettings["CruisedSwitch"].ToString());
            mCoreTask.bRepaired = Convert.ToBoolean(ConfigurationManager.AppSettings["RepairSwitch"].ToString());
            mCoreTask.bPushcar = Convert.ToBoolean(ConfigurationManager.AppSettings["PushSwitch"].ToString());
            mCoreTask.bBackcar = Convert.ToBoolean(ConfigurationManager.AppSettings["BackSwitch"].ToString());

            mCoreTask.bServerActivated = Convert.ToBoolean(ConfigurationManager.AppSettings["ServerFlag"].ToString());
            mCoreTask.DefaultSN = ConfigurationManager.AppSettings["DefaultSN"].ToString();
            mCoreTask.DefaultBT = ConfigurationManager.AppSettings["DefaultBT"].ToString();
            mCoreTask.DefaultBLE = ConfigurationManager.AppSettings["DefaultBLE"].ToString();
            mCoreTask.DefaultKeyt = ConfigurationManager.AppSettings["DefaultKeyt"].ToString();

            mCoreTask.Update4103ListViewHandler += (object _sender, EventArgs _e) =>
            {
                RK4103ItemsArgs mArgs = _e as RK4103ItemsArgs;
                if (mArgs != null)
                {
                    SetRK4103ItemList(mArgs.items, mArgs.info, mArgs.level);
                }
            };
            mCoreTask.Update4103PinListHandler += (object _sender, EventArgs _e) =>
            {
                PinStatusArgs mArgs = _e as PinStatusArgs;
                if (mArgs != null)
                {
                    SetRK4103PinList(mArgs.status, mArgs.level);
                }
            };
            mCoreTask.Update7001ListViewHandler += (object _sender, EventArgs _e) =>
            {
                RK7001ItemsArgs mArgs = _e as RK7001ItemsArgs;
                if (mArgs != null)
                {
                    SetRK7001ItemList(mArgs.items, mArgs.info, mArgs.level);
                }
            };
            mCoreTask.Update7001PinListHandler += (object _sender, EventArgs _e) =>
            {
                PinStatusArgs mArgs = _e as PinStatusArgs;
                if (mArgs != null)
                {
                    SetRK7001PinList(mArgs.status, mArgs.level);
                }
            };
            mCoreTask.UpdateChkServerHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetTestServer(mArgs.level);
                }
            };
            mCoreTask.UpdateValidSNHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetValidSN(mArgs.level);
                }
            };          
            mCoreTask.UpdateWorkStatusHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetMainText(mArgs.msg, mArgs.submsg, mArgs.level);
                }
            };
            mCoreTask.UpdateRemoteHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if(mArgs != null)
                {
                    SetRemoteStatus(mArgs.level);
                }
            };

            mCoreTask.UpdateListViewHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if(mArgs != null)
                {
                    SetListView(mArgs.msg, mArgs.submsg);
                }
            };

            mCoreTask.UpdatePotTickerHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if (mArgs != null)
                {
                    SetPotTicker(mArgs.msg, mArgs.level);
                }
            };

            mCoreTask.UpdateVddGpsHandler += (object _sender, EventArgs _e) =>
            {
                UIEventArgs mArgs = _e as UIEventArgs;
                if(mArgs != null)
                {
                    UpdateVddGpsPin(mArgs.level);
                }
            };
            //初始化界面
            SetValidSN(INFO_LEVEL.INIT);
            SetTestServer(INFO_LEVEL.INIT);
            SetRK7001PinList(null, INFO_LEVEL.INIT);
            SetRK4103PinList(null, INFO_LEVEL.INIT);

            this.labelPos.Visible = false;
            this.tbInputSN.Focus();
            this.tbInputSN.TabIndex = 0;
        }
        #endregion
     
        #region 启动任务
        private void StartTask()
        {
            SetValidSN(INFO_LEVEL.PROCESS);
            SetTestServer(INFO_LEVEL.PROCESS);
            SetRK7001ItemList(RK7001ITEM.INIT, null, INFO_LEVEL.INIT);
            SetRK4103ItemList(RK4103ITEM.INIT, null, INFO_LEVEL.INIT);
            this.lvSolutions.Items.Clear();
            mCoreTask.mSN = this.tbInputSN.Text;
            this.tbInputSN.Enabled = false;
            mCoreTask.ExcuteTask();
        }
        #endregion

        #region 设置动态
        delegate void SetPotTickerCallback(string msg, INFO_LEVEL level);
        private void SetPotTicker(string msg, INFO_LEVEL level)
        {
            if (this.labelPos.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.labelPos.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.labelPos.Disposing || this.labelPos.IsDisposed)
                        return;
                }
                SetPotTickerCallback d = new SetPotTickerCallback(SetPotTicker);
                this.labelPos.Invoke(d, new object[] { msg, level });
            }
            else
            {
                switch(level)
                {
                    case INFO_LEVEL.INIT:
                        this.labelPos.Visible = false;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.labelPos.Visible = true;
                        this.labelPos.Text = msg;
                        break;                    
                }
            }
        }
        #endregion

        #region 设置主界面
        delegate void setMainTextCallback(string msg, string submsg, INFO_LEVEL level);
        private void SetMainText(string msg, string submsg, INFO_LEVEL level)
        {
            if (this.LbWorkResult.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.LbWorkResult.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.LbWorkResult.Disposing || this.LbWorkResult.IsDisposed)
                        return;
                }
                setMainTextCallback d = new setMainTextCallback(SetMainText);
                this.LbWorkResult.Invoke(d, new object[] { msg, submsg, level });
            }
            else
            {
                this.LbWorkResult.Text = msg;        
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.PnWorkStatus.BackColor = Color.LightBlue;
                        this.LbWorkTip.Text = "";
                        break;
                    case INFO_LEVEL.PASS:
                        this.PnWorkStatus.BackColor = Color.Green;
                        this.LbWorkTip.Text = "再次扫描,进行下一次测试!";
                        this.tbInputSN.Enabled = true;
                        this.tbInputSN.Text = "";
                        this.tbInputSN.Focus();
                        break;
                    case INFO_LEVEL.FAIL:
                        this.PnWorkStatus.BackColor = Color.Red;
                        this.LbWorkTip.Text = "再次扫描,进行下一次测试!";
                        this.tbInputSN.Enabled = true;
                        this.tbInputSN.Text = "";
                        this.tbInputSN.Focus();
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.PnWorkStatus.BackColor = Color.Yellow;
                        this.LbWorkTip.Text = submsg;
                        break;
                    case INFO_LEVEL.ONLY_TIP:
                        this.LbWorkTip.Text = submsg;
                        break;
                }
            }


        }
        #endregion

        #region 更新VDD_GPS pin
        delegate void UpdateVddGpsPinCallback(INFO_LEVEL level);
        private void UpdateVddGpsPin(INFO_LEVEL level)
        {
            if (this.rkPin_1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.rkPin_1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.rkPin_1.Disposing || this.rkPin_1.IsDisposed)
                        return;
                }
                UpdateVddGpsPinCallback d = new UpdateVddGpsPinCallback(UpdateVddGpsPin);
                this.rkPin_1.Invoke(d, new object[] { level });
            }
            else
            {
                switch(level)
                {
                    case INFO_LEVEL.INIT:
                        this.rkPin_1.BackColor = Color.White;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.rkPin_1.BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PASS:
                        this.rkPin_1.BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.rkPin_1.BackColor = Color.Yellow;
                        break;
                    case INFO_LEVEL.GREY:
                        this.rkPin_1.BackColor = Color.Gray;
                        break;
                }
            }
        }
        #endregion

        #region 设置ListView
        delegate void SetListViewCallback(string msg, string submsg);
        private void SetListView(string msg, string submsg)
        {
            if (this.lvSolutions.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.lvSolutions.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.lvSolutions.Disposing || this.lvSolutions.IsDisposed)
                        return;
                }
                SetListViewCallback d = new SetListViewCallback(SetListView);
                this.lvSolutions.Invoke(d, new object[] { msg, submsg});
            }
            else
            {
                this.lvSolutions.BeginUpdate();
                ListViewItem lvi = new ListViewItem();
                lvi.Text = DateTime.Now.ToString("HH:mm:ss:fff");
                lvi.SubItems.Add(msg);                
                lvi.SubItems.Add(submsg);
                this.lvSolutions.Items.Add(lvi);
                //总是显示最后一行
                this.lvSolutions.Items[this.lvSolutions.Items.Count - 1].EnsureVisible();
                this.lvSolutions.EndUpdate();  //结束数据处理，UI界面一次性绘制。



            }
        }
        #endregion

        #region 设置SN号合法性界面
        delegate void SetValidSNCallback(INFO_LEVEL level);
        private void SetValidSN(INFO_LEVEL level)
        {
            if (this.lbItemValidSN.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.lbItemValidSN.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.lbItemValidSN.Disposing || this.lbItemValidSN.IsDisposed)
                        return;
                }
                SetValidSNCallback d = new SetValidSNCallback(SetValidSN);
                this.lbItemValidSN.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.lbItemValidSN.BackColor = Color.White;
                        break;
                    case INFO_LEVEL.PASS:
                        this.lbItemValidSN.BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.lbItemValidSN.BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.lbItemValidSN.BackColor = Color.Yellow;
                        break;
                }
            }
        }
        #endregion

        #region 设置测试SERVER
        delegate void SetTestServerCallback(INFO_LEVEL level);
        private void SetTestServer(INFO_LEVEL level)
        {
            if (this.lbItemChkServer.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.lbItemChkServer.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.lbItemChkServer.Disposing || this.lbItemChkServer.IsDisposed)
                        return;
                }
                SetTestServerCallback d = new SetTestServerCallback(SetTestServer);
                this.lbItemChkServer.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.lbItemChkServer.BackColor = Color.White;
                        break;
                    case INFO_LEVEL.PASS:
                        this.lbItemChkServer.BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.lbItemChkServer.BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.lbItemChkServer.BackColor = Color.Yellow;
                        break;
                }
            }
        }
        #endregion

        #region 设置遥控器测试界面
        delegate void SetRemoteStatusCallback(INFO_LEVEL level);
        private void SetRemoteStatus(INFO_LEVEL level)
        {
            if (this.panel_remoter.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.panel_remoter.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.panel_remoter.Disposing || this.panel_remoter.IsDisposed)
                        return;
                }
                SetRemoteStatusCallback d = new SetRemoteStatusCallback(SetRemoteStatus);
                this.panel_remoter.Invoke(d, new object[] { level });
            }
            else
            {
                switch (level)
                {
                    case INFO_LEVEL.INIT:
                        this.panel_remoter.BackColor = Color.White;
                        break;
                    case INFO_LEVEL.PASS:
                        this.panel_remoter.BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.FAIL:
                        this.panel_remoter.BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PROCESS:
                        this.panel_remoter.BackColor = Color.Yellow;
                        break;
                }
            }
        }
        #endregion

        #region 设置RK7001的列表项目
        delegate void SetRK7001ItemListCallback(RK7001ITEM item, DeviceInfo info, INFO_LEVEL level);
        private void SetRK7001ItemList(RK7001ITEM item, DeviceInfo info, INFO_LEVEL level)
        {
            if (this.lvRK7001ErrItem.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.lvRK7001ErrItem.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.lvRK7001ErrItem.Disposing || this.lvRK7001ErrItem.IsDisposed)
                        return;
                }
                SetRK7001ItemListCallback d = new SetRK7001ItemListCallback(SetRK7001ItemList);
                this.lvRK7001ErrItem.Invoke(d, new object[] { item, info, level });
            }
            else
            {
                this.lvRK7001ErrItem.BeginUpdate();
                switch (item)
                {
                    case RK7001ITEM.VERSION:
                        this.lv7001uid.SubItems.Add(info.sn);
                        this.lv7001sw.SubItems.Add(info.sw);
                        this.lv7001hw.SubItems.Add(info.hw);
                        break;
                    case RK7001ITEM.INIT:     
                        switch(level)
                        {
                            case INFO_LEVEL.INIT:
                                this.lv7001uid.SubItems.Clear();
                                this.lv7001sw.SubItems.Clear();
                                this.lv7001hw.SubItems.Clear();
                                this.lv7001ampf.SubItems.Clear();
                                this.lv7001mosfet.SubItems.Clear();
                                this.lv7001buzzer.SubItems.Clear();                                
                                this.lv7001uid.BackColor = Color.White;
                                this.lv7001sw.BackColor = Color.White;
                                this.lv7001hw.BackColor = Color.White;
                                this.lv7001ampf.BackColor = Color.White;
                                this.lv7001mosfet.BackColor = Color.White;
                                this.lv7001buzzer.BackColor = Color.White;
                                this.lv7001uid.Text = "设备UID";
                                this.lv7001sw.Text = "软件版本号";
                                this.lv7001hw.Text = "硬件版本号";
                                this.lv7001ampf.Text = "运放故障";
                                this.lv7001mosfet.Text = "Mos短路";
                                this.lv7001buzzer.Text = "Buzzer故障";
                                break;
                            case INFO_LEVEL.PASS:
                                this.lv7001ampf.SubItems.Add("通过");
                                this.lv7001mosfet.SubItems.Add("通过");
                                this.lv7001buzzer.SubItems.Add("通过");
                                this.lv7001ampf.SubItems[1].ForeColor = Color.Green;
                                this.lv7001mosfet.SubItems[1].ForeColor = Color.Green;
                                this.lv7001buzzer.SubItems[1].ForeColor = Color.Green;
                                break;
                        }                 
                        
                        break;                    
                    case RK7001ITEM.AMPLIFY:
                        switch(level)
                        {
                            case INFO_LEVEL.PASS:
                                this.lv7001ampf.SubItems.Add("通过");
                                this.lv7001ampf.SubItems[1].ForeColor = Color.Green;
                                break;
                            case INFO_LEVEL.FAIL:
                                this.lv7001ampf.SubItems.Add("失败");
                                this.lv7001ampf.SubItems[0].BackColor = Color.Red;
                                this.lv7001ampf.SubItems[1].BackColor = Color.Red;
                                break;
                        }
                        break;
                    case RK7001ITEM.MOSFET:
                        switch(level)
                        {
                            case INFO_LEVEL.PASS:
                                this.lv7001mosfet.SubItems.Add("通过");
                                this.lv7001mosfet.SubItems[1].ForeColor = Color.Green;
                                break;
                            case INFO_LEVEL.FAIL:
                                this.lv7001mosfet.SubItems.Add("失败");
                                this.lv7001mosfet.SubItems[0].BackColor = Color.Red;
                                this.lv7001mosfet.SubItems[1].BackColor = Color.Red;
                                break;
                        }
                        break;
                    case RK7001ITEM.BUZZER:
                        switch (level)
                        {
                            case INFO_LEVEL.PASS:
                                this.lv7001buzzer.SubItems.Add("通过");
                                this.lv7001buzzer.SubItems[1].ForeColor = Color.Green;
                                break;
                            case INFO_LEVEL.FAIL:
                                this.lv7001buzzer.SubItems.Add("失败");
                                this.lv7001buzzer.SubItems[0].BackColor = Color.Red;
                                this.lv7001buzzer.SubItems[1].BackColor = Color.Red;
                                break;
                        }
                        break;
                }
                this.lvRK7001ErrItem.EndUpdate();
            }
        }
        #endregion

        #region 设置RK4103的列表项目
        delegate void SetRK4103ItemListCallback(RK4103ITEM item, DeviceInfo info, INFO_LEVEL level);
        private void SetRK4103ItemList(RK4103ITEM item, DeviceInfo info, INFO_LEVEL level)
        {
            if (this.lvRK4003ErrItem.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.lvRK4003ErrItem.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.lvRK4003ErrItem.Disposing || this.lvRK4003ErrItem.IsDisposed)
                        return;
                }
                SetRK4103ItemListCallback d = new SetRK4103ItemListCallback(SetRK4103ItemList);
                this.lvRK4003ErrItem.Invoke(d, new object[] { item, info, level });
            }
            else
            {
                this.lvRK4003ErrItem.BeginUpdate();
                switch(item)
                {
                    case RK4103ITEM.INIT:
                        switch(level)
                        {
                            case INFO_LEVEL.INIT:
                                this.lv4103sw.SubItems.Clear();
                                this.lv4103lgtsensor.SubItems.Clear();
                                this.lv4103gsensor.SubItems.Clear();
                                this.lv4103ble.SubItems.Clear();
                                this.lv4103sw.BackColor = Color.White;
                                this.lv4103lgtsensor.BackColor = Color.White;
                                this.lv4103gsensor.BackColor = Color.White;
                                this.lv4103ble.BackColor = Color.White;
                                this.lv4103sw.Text = "软件版本号";
                                this.lv4103lgtsensor.Text = "光感异常测试";
                                this.lv4103gsensor.Text = "Gsensor测试";
                                this.lv4103ble.Text = "蓝牙4.0测试";
                                break;
                            case INFO_LEVEL.PASS:
                                this.lv4103lgtsensor.SubItems.Add("通过");
                                this.lv4103gsensor.SubItems.Add("通过");
                                this.lv4103lgtsensor.SubItems[1].ForeColor = Color.Green;
                                this.lv4103gsensor.SubItems[1].ForeColor = Color.Green;
                                break;
                        }
                                         
                        break;
                    case RK4103ITEM.VERSION:
                        this.lv4103sw.SubItems.Add(info.sw);
                        break;
                    case RK4103ITEM.LIGHTSENSOR:
                        switch(level)
                        {
                            case INFO_LEVEL.FAIL:
                                this.lv4103lgtsensor.SubItems.Add("失败");
                                this.lv4103lgtsensor.SubItems[0].BackColor = Color.Red;
                                this.lv4103lgtsensor.SubItems[1].BackColor = Color.Red;
                                break;
                            case INFO_LEVEL.PASS:
                                this.lv4103lgtsensor.SubItems.Add("通过");
                                this.lv4103lgtsensor.SubItems[1].ForeColor = Color.Green;
                                break;
                        }
                        break;
                    case RK4103ITEM.GSENSOR:
                        switch(level)
                        {
                            case INFO_LEVEL.FAIL:
                                this.lv4103gsensor.SubItems.Add("失败");
                                this.lv4103gsensor.SubItems[0].BackColor = Color.Red;
                                this.lv4103gsensor.SubItems[1].BackColor = Color.Red;
                                break;
                            case INFO_LEVEL.PASS:
                                this.lv4103gsensor.SubItems.Add("通过");
                                this.lv4103gsensor.SubItems[1].ForeColor = Color.Green;
                                break;
                        }
                        break;                                                            
                    case RK4103ITEM.BLE:
                        switch(level)
                        {
                            case INFO_LEVEL.FAIL:
                                this.lv4103ble.SubItems.Add("失败");
                                this.lv4103ble.SubItems[0].BackColor = Color.Red;
                                this.lv4103ble.SubItems[1].BackColor = Color.Red;
                                break;
                            case INFO_LEVEL.PASS:
                                this.lv4103ble.SubItems.Add("通过");
                                this.lv4103ble.SubItems[1].ForeColor = Color.Green;
                                break;
                        }
                        break;                    
                }
                this.lvRK4003ErrItem.EndUpdate();
            }
        }
        #endregion

        #region 设置RK7001 PIN脚图
        private void SetRK7001PinList(PIN_STATUS status, INFO_LEVEL level)
        {
            if(level == INFO_LEVEL.INIT)
            {
                for(int i = 1; i <=28; i++)
                {
                    if(i == 1 || i == 7 || i == 22 || i == 10 || i == 14)
                        Set7001Pin(i, INFO_LEVEL.GREY);
                    else
                        Set7001Pin(i, INFO_LEVEL.INIT);
                }
                return;
            }
            else if (level == INFO_LEVEL.PROCESS)
            {
                for (int i = 1; i <= 28; i++)
                {
                    if (i == 1 || i == 7 || i == 22 || i == 10 || i == 14)
                        Set7001Pin(i, INFO_LEVEL.GREY);
                    else
                        Set7001Pin(i, INFO_LEVEL.PROCESS);
                }
                return;         
            }
            else if(level == INFO_LEVEL.PASS)
            {
                for (int i = 1; i <= 28; i++)
                {
                    if (i == 1 || i == 7 || i == 22 || i == 10 || i == 14)
                        Set7001Pin(i, INFO_LEVEL.GREY);
                    else
                        Set7001Pin(i, INFO_LEVEL.PASS);
                }
                return;
            }

            //判断错误
            /*
            if(status.Pin1_Open || status.Pin1_Short)
                Set7001Pin(1, INFO_LEVEL.FAIL);
            else
                Set7001Pin(1, INFO_LEVEL.PASS);
            */
            if (status.Pin2_Open || status.Pin2_Short)
                Set7001Pin(2, INFO_LEVEL.FAIL);
            else
                Set7001Pin(2, INFO_LEVEL.PASS);
            if (status.Pin3_Open || status.Pin3_Short)
                Set7001Pin(3, INFO_LEVEL.FAIL);
            else
                Set7001Pin(3, INFO_LEVEL.PASS);
            if (status.Pin4_Open || status.Pin4_Short)
                Set7001Pin(4, INFO_LEVEL.FAIL);
            else
                Set7001Pin(4, INFO_LEVEL.PASS);
            if (status.Pin5_Open || status.Pin5_Short)
                Set7001Pin(5, INFO_LEVEL.FAIL);
            else
                Set7001Pin(5, INFO_LEVEL.PASS);
            if (status.Pin6_Open || status.Pin6_Short)
                Set7001Pin(6, INFO_LEVEL.FAIL);
            else
                Set7001Pin(6, INFO_LEVEL.PASS);
            /*
            if (status.Pin7_Open || status.Pin7_Short)
                Set7001Pin(7, INFO_LEVEL.FAIL);
            else
                Set7001Pin(7, INFO_LEVEL.PASS);
            */
            if (status.Pin8_Open || status.Pin8_Short)
                Set7001Pin(8, INFO_LEVEL.FAIL);
            else
                Set7001Pin(8, INFO_LEVEL.PASS);
            if (status.Pin9_Open || status.Pin9_Short)
                Set7001Pin(9, INFO_LEVEL.FAIL);
            else
                Set7001Pin(9, INFO_LEVEL.PASS);
            /*
            if (status.Pin10_Open || status.Pin10_Short)
                Set7001Pin(10, INFO_LEVEL.FAIL);
            else
                Set7001Pin(10, INFO_LEVEL.PASS);
            */
            if (status.Pin11_Open || status.Pin11_Short)
                Set7001Pin(11, INFO_LEVEL.FAIL);
            else
                Set7001Pin(11, INFO_LEVEL.PASS);
            if (status.Pin12_Open || status.Pin12_Short)
                Set7001Pin(12, INFO_LEVEL.FAIL);
            else
                Set7001Pin(12, INFO_LEVEL.PASS);
            
            if (status.Pin13_Open || status.Pin13_Short)
                Set7001Pin(13, INFO_LEVEL.FAIL);
            else
                Set7001Pin(13, INFO_LEVEL.PASS);
            /*
            if (status.Pin14_Open || status.Pin14_Short)
                Set7001Pin(14, INFO_LEVEL.FAIL);
            else
                Set7001Pin(14, INFO_LEVEL.PASS);
            */
            if (status.Pin15_Open || status.Pin15_Short)
                Set7001Pin(15, INFO_LEVEL.FAIL);
            else
                Set7001Pin(15, INFO_LEVEL.PASS);
            
            if (status.Pin16_Open || status.Pin16_Short)
                Set7001Pin(16, INFO_LEVEL.FAIL);
            else
                Set7001Pin(16, INFO_LEVEL.PASS);
            if (status.Pin17_Open || status.Pin17_Short)
                Set7001Pin(17, INFO_LEVEL.FAIL);
            else
                Set7001Pin(17, INFO_LEVEL.PASS);
            if (status.Pin18_Open || status.Pin18_Short)
                Set7001Pin(18, INFO_LEVEL.FAIL);
            else
                Set7001Pin(18, INFO_LEVEL.PASS);
            if (status.Pin19_Open || status.Pin19_Short)
                Set7001Pin(19, INFO_LEVEL.FAIL);
            else
                Set7001Pin(19, INFO_LEVEL.PASS);

            if (status.Pin20_Open || status.Pin20_Short)
                Set7001Pin(20, INFO_LEVEL.FAIL);
            else
                Set7001Pin(20, INFO_LEVEL.PASS);
            if (status.Pin21_Open || status.Pin21_Short)
                Set7001Pin(21, INFO_LEVEL.FAIL);
            else
                Set7001Pin(21, INFO_LEVEL.PASS);
            /*
            if (status.Pin22_Open || status.Pin22_Short)
                Set7001Pin(22, INFO_LEVEL.FAIL);
            else
                Set7001Pin(22, INFO_LEVEL.PASS);
            */
            if (status.Pin23_Open || status.Pin23_Short)
                Set7001Pin(23, INFO_LEVEL.FAIL);
            else
                Set7001Pin(23, INFO_LEVEL.PASS);
            
            if (status.Pin24_Open || status.Pin24_Short)
                Set7001Pin(24, INFO_LEVEL.FAIL);
            else
                Set7001Pin(24, INFO_LEVEL.PASS);
            
            if (status.Pin25_Open || status.Pin25_Short)
                Set7001Pin(25, INFO_LEVEL.FAIL);
            else
                Set7001Pin(25, INFO_LEVEL.PASS);
            if (status.Pin26_Open || status.Pin26_Short)
                Set7001Pin(26, INFO_LEVEL.FAIL);
            else
                Set7001Pin(26, INFO_LEVEL.PASS);
            if (status.Pin27_Open || status.Pin27_Short)
                Set7001Pin(27, INFO_LEVEL.FAIL);
            else
                Set7001Pin(27, INFO_LEVEL.PASS);
            if (status.Pin28_Open || status.Pin28_Short)
                Set7001Pin(28, INFO_LEVEL.FAIL);
            else
                Set7001Pin(28, INFO_LEVEL.PASS);
            /*
            if (status.Pin29_Open || status.Pin29_Short)
                Set7001Pin(29, INFO_LEVEL.FAIL);
            else
                Set7001Pin(29, INFO_LEVEL.PASS);
            if (status.Pin30_Open || status.Pin30_Short)
                Set7001Pin(30, INFO_LEVEL.FAIL);
            else
                Set7001Pin(30, INFO_LEVEL.PASS);
            */
        }
        #endregion

        #region 设置7001单个pin脚
        delegate void Set7001PinCallback(int num, INFO_LEVEL result);
        private void Set7001Pin(int num, INFO_LEVEL result)
        {           
            if (mRK7001Pin[num - 1].InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!mRK7001Pin[num -1].IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (mRK7001Pin[num - 1].Disposing || mRK7001Pin[num - 1].IsDisposed)
                        return;
                }
                Set7001PinCallback d = new Set7001PinCallback(Set7001Pin);
                mRK7001Pin[num - 1].Invoke(d, new object[] { num, result });
            }
            else
            {
                switch(result)
                {
                    case INFO_LEVEL.FAIL:
                        mRK7001Pin[num - 1].BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PASS:
                        mRK7001Pin[num - 1].BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.PROCESS:
                        mRK7001Pin[num - 1].BackColor = Color.Yellow;
                        break;
                    case INFO_LEVEL.INIT:
                        mRK7001Pin[num - 1].BackColor = Color.White;
                        break;
                    case INFO_LEVEL.GREY:
                        mRK7001Pin[num - 1].BackColor = Color.Gray;
                        break;
                }                             
            }
        }
        #endregion

        #region 设置RK4103的PIN脚图
        private void SetRK4103PinList(PIN_STATUS status, INFO_LEVEL level)
        {
            if (level == INFO_LEVEL.INIT)
            {
                for (int i = 2; i <= 30; i++)
                {
                   
                    if (i == 16 || i == 17 || i == 19 || i == 26 || i== 15 || i == 20 
                        || i== 5 || i == 2 || i== 29 || i == 30 || i == 6)
                        SetRK4103Pin(i, INFO_LEVEL.GREY);
                    else
                        SetRK4103Pin(i, INFO_LEVEL.INIT);
                }
                return;
            }
            else if (level == INFO_LEVEL.PROCESS)
            {
                for (int i = 2; i <= 30; i++)
                {                    
                    if (i == 16 || i == 17 || i == 19 || i == 26 || i == 15 || i == 20 
                        || i == 5 || i == 2 || i == 29 || i == 30 || i == 6)
                        SetRK4103Pin(i, INFO_LEVEL.GREY);
                    else
                        SetRK4103Pin(i, INFO_LEVEL.PROCESS);
                }
                return;
            }
            else if (level == INFO_LEVEL.PASS)
            {
                for (int i = 2; i <= 30; i++)
                {                    
                    if (i == 16 || i == 17 || i == 19 || i == 26 || i == 15 || i == 20 
                        || i == 5 || i == 2 || i== 29 || i== 30 || i == 6)
                        SetRK4103Pin(i, INFO_LEVEL.GREY);
                    else
                        SetRK4103Pin(i, INFO_LEVEL.PASS);
                }
                return;
            }

            //判断错误
            /*
            if (status.Pin1_Open || status.Pin1_Short)
                SetRK4103Pin(1, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(1, INFO_LEVEL.PASS);
            */
            /*
            if (status.Pin2_Open || status.Pin2_Short)
                SetRK4103Pin(2, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(2, INFO_LEVEL.PASS);
            */
            if (status.Pin3_Open || status.Pin3_Short)
                SetRK4103Pin(3, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(3, INFO_LEVEL.PASS);
            if (status.Pin4_Open || status.Pin4_Short)
                SetRK4103Pin(4, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(4, INFO_LEVEL.PASS);
            /*
            if (status.Pin5_Open || status.Pin5_Short)
                SetRK4103Pin(5, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(5, INFO_LEVEL.PASS);
            */
            /*
            if (status.Pin6_Open || status.Pin6_Short)
                SetRK4103Pin(6, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(6, INFO_LEVEL.PASS);
            */
            if (status.Pin7_Open || status.Pin7_Short)
                SetRK4103Pin(7, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(7, INFO_LEVEL.PASS);
            if (status.Pin8_Open || status.Pin8_Short)
                SetRK4103Pin(8, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(8, INFO_LEVEL.PASS);
            
            if (status.Pin9_Open || status.Pin9_Short)
                SetRK4103Pin(9, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(9, INFO_LEVEL.PASS);
            
            if (status.Pin10_Open || status.Pin10_Short)
                SetRK4103Pin(10, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(10, INFO_LEVEL.PASS);
            if (status.Pin11_Open || status.Pin11_Short)
                SetRK4103Pin(11, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(11, INFO_LEVEL.PASS);
            if (status.Pin12_Open || status.Pin12_Short)
                SetRK4103Pin(12, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(12, INFO_LEVEL.PASS);
            if (status.Pin13_Open || status.Pin13_Short)
                SetRK4103Pin(13, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(13, INFO_LEVEL.PASS);
            if (status.Pin14_Open || status.Pin14_Short)
                SetRK4103Pin(14, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(14, INFO_LEVEL.PASS);
            /*
            if (status.Pin15_Open || status.Pin15_Short)
                SetRK4103Pin(15, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(15, INFO_LEVEL.PASS);
            */
            /*
            if (status.Pin16_Open || status.Pin16_Short)
                SetRK4103Pin(16, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(16, INFO_LEVEL.PASS);
            if (status.Pin17_Open || status.Pin17_Short)
                SetRK4103Pin(17, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(17, INFO_LEVEL.PASS);
            */
            if (status.Pin18_Open || status.Pin18_Short)
                SetRK4103Pin(18, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(18, INFO_LEVEL.PASS);
            /*
            if (status.Pin19_Open || status.Pin19_Short)
                SetRK4103Pin(19, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(19, INFO_LEVEL.PASS);
            */
            /*
            if (status.Pin20_Open || status.Pin20_Short)
                SetRK4103Pin(20, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(20, INFO_LEVEL.PASS);
            */
            if (status.Pin21_Open || status.Pin21_Short)
                SetRK4103Pin(21, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(21, INFO_LEVEL.PASS);
            if (status.Pin22_Open || status.Pin22_Short)
                SetRK4103Pin(22, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(22, INFO_LEVEL.PASS);
            if (status.Pin23_Open || status.Pin23_Short)
                SetRK4103Pin(23, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(23, INFO_LEVEL.PASS);
            if (status.Pin24_Open || status.Pin24_Short)
                SetRK4103Pin(24, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(24, INFO_LEVEL.PASS);
            if (status.Pin25_Open || status.Pin25_Short)
                SetRK4103Pin(25, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(25, INFO_LEVEL.PASS);
            /*
            if (status.Pin26_Open || status.Pin26_Short)
                SetRK4103Pin(26, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(26, INFO_LEVEL.PASS);
            */
            if (status.Pin27_Open || status.Pin27_Short)
                SetRK4103Pin(27, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(27, INFO_LEVEL.PASS);
            if (status.Pin28_Open || status.Pin28_Short)
                SetRK4103Pin(28, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(28, INFO_LEVEL.PASS);
            /*
            if (status.Pin29_Open || status.Pin29_Short)
                SetRK4103Pin(29, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(29, INFO_LEVEL.PASS);
            */
            /*
            if (status.Pin30_Open || status.Pin30_Short)
                SetRK4103Pin(30, INFO_LEVEL.FAIL);
            else
                SetRK4103Pin(30, INFO_LEVEL.PASS);
            */
        }
        #endregion

        #region 设置RK4103单个pin
        delegate void SetRK4103PinCallback(int num, INFO_LEVEL result);
        private void SetRK4103Pin(int num, INFO_LEVEL result)
        {
            if (mRK4103Pin[num - 1].InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!mRK4103Pin[num - 1].IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (mRK4103Pin[num - 1].Disposing || mRK4103Pin[num - 1].IsDisposed)
                        return;
                }
                SetRK4103PinCallback d = new SetRK4103PinCallback(SetRK4103Pin);
                mRK4103Pin[num - 1].Invoke(d, new object[] { num, result });
            }
            else
            {
                switch (result)
                {
                    case INFO_LEVEL.FAIL:
                        mRK4103Pin[num - 1].BackColor = Color.Red;
                        break;
                    case INFO_LEVEL.PASS:
                        mRK4103Pin[num - 1].BackColor = Color.Green;
                        break;
                    case INFO_LEVEL.PROCESS:
                        mRK4103Pin[num - 1].BackColor = Color.Yellow;
                        break;
                    case INFO_LEVEL.INIT:
                        mRK4103Pin[num - 1].BackColor = Color.White;
                        break;
                    case INFO_LEVEL.GREY:
                        mRK4103Pin[num - 1].BackColor = Color.Gray;
                        break;

                }
            }
        }
        #endregion

        #region 关闭界面
        private void MainForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region key键事件
        private void KeyDown_Start(object sender, KeyEventArgs e)
        {         
            if (e.KeyCode == Keys.Enter)
            {
                if (mCoreTask.bTaskRunning)
                    return;
                if (mCoreTask.bServerActivated)
                {
                    if(this.tbInputSN.TextLength == 10)
                        StartTask();
                }
                else
                {
                    this.tbInputSN.Text = mCoreTask.DefaultSN;
                    StartTask();
                }                
            }                   
        }
        #endregion
    }
}
