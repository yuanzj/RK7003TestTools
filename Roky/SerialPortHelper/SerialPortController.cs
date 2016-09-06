using CommonUtils;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky.SerialPortHelper
{
    public class SerialPortController
    {
        //使用了readonly，保证instanse不被更改
        private static readonly SerialPortController instance = new SerialPortController();
        //获取串口的类实例
        public static SerialPortController GetInstance()
        {
            return instance;
        }

        #region private
        private List<EventHandler> _ErrorEventHandleList;
        private List<EventHandler> _DataReceivedEventHandleList;
        private SerialPort comm = new SerialPort();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private long received_count = 0;//接收计数
        private long send_count = 0;//发送计数
        private volatile bool Listening = false;//是否没有执行完invoke相关操作
        private bool Closing = false;//是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke
        private List<byte> buffer = new List<byte>(4096);//默认分配1页内存，并始终限制不允许超过
        #endregion

        #region public

        public ISerialProtocol MyProtocol { get; set; }

        public event EventHandler _ErrorEvent;
        public event EventHandler ErrorEvent
        {
            add
            {
                ClearErrorEvent();
                _ErrorEvent += value;
                _ErrorEventHandleList.Add(value);
            }

            remove
            {
                _ErrorEvent -= value;
            }
        }
        private void ClearErrorEvent()
        {
            for (int i = 0; i < this._ErrorEventHandleList.Count; i++)
            {
                _ErrorEvent -= this._ErrorEventHandleList[i];
            }
        }

        public event EventHandler _DataReceivedEvent;
        public event EventHandler DataReceivedEvent
        {
            add
            {
                ClearDataReceivedEvent();
                _DataReceivedEvent += value;
                _DataReceivedEventHandleList.Add(value);
            }
            remove
            {
                _DataReceivedEvent -= value;
            }
        }
        private void ClearDataReceivedEvent()
        {
            for (int i = 0; i < this._DataReceivedEventHandleList.Count; i++)
            {
                _DataReceivedEvent -= this._DataReceivedEventHandleList[i];
            }
        }

        public string PortName { get; set; }

        public int BaudRate { get; set; }
        #endregion

        public SerialPortController()
        {
            _ErrorEventHandleList = new List<EventHandler>();
            _DataReceivedEventHandleList = new List<EventHandler>();
            //初始化SerialPort对象
            //comm.NewLine = "\r\n";
            //comm.RtsEnable = true;//根据实际情况吧。
            //添加事件注册
            comm.DataReceived += comm_DataReceived;
        }

        public void Initialization(EventHandler _ErrorEvent, EventHandler _DataReceivedEvent, ISerialProtocol _MyProtocol)
        {
            //在初始化之前，清除所有的Event  added by harryMeng
            ClearErrorEvent();
            ClearDataReceivedEvent();
            this.ErrorEvent += _ErrorEvent;
            this.DataReceivedEvent += _DataReceivedEvent;
            this.MyProtocol = _MyProtocol;
            this.OpenPort();
        }

        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Closing) return;//如果正在关闭，忽略操作，直接返回，尽快的完成串口监听线程的一次循环
            try
            {
                Listening = true;//设置标记，说明我已经开始处理数据，一会儿要使用系统UI的。
                int n = comm.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
                received_count += n;//增加接收计数
                comm.Read(buf, 0, n);//读取缓冲数据
                Console.WriteLine(Util.ToHexString(buf));
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //<协议解析>
                bool data_catched = false;//缓存记录数据是否捕获到
                byte[] binary_data = null;
                //1.缓存数据
                buffer.AddRange(buf);
                //2.完整性判断
                while (buffer.Count >= MyProtocol.MinLength())//至少要包含头（2字节）+长度（1字节）+校验（1字节）
                {
                    //请不要担心使用>=，因为>=已经和>,<,=一样，是独立操作符，并不是解析成>和=2个符号
                    //2.1 查找数据头
                    if (MyProtocol.StartInProtocol(buffer))
                    {
                        //2.2 探测缓存数据是否有一条数据的字节，如果不够，就不用费劲的做其他验证了
                        //前面已经限定了剩余长度>=4，那我们这里一定能访问到buffer[2]这个长度
                        int len = MyProtocol.DataLength(buffer);//数据长度
                        //数据完整判断第一步，长度是否足够
                        //len是数据段长度,4个字节是while行注释的3部分长度
                        if (buffer.Count < len) break;//数据不够的时候什么都不做
                                                      //这里确保数据长度足够，数据头标志找到，我们开始计算校验
                                                      //2.3 校验数据，确认数据正确
                                                      //异或校验，逐个字节异或得到校验码

                        if (!MyProtocol.CheckOK(buffer)) //如果数据校验失败，丢弃这一包数据
                        {
                            buffer.RemoveRange(0, buffer.Count);//从缓存中删除错误数据
                            continue;//继续下一次循环
                        }
                        //至此，已经被找到了一条完整数据。我们将数据直接分析，或是缓存起来一起分析
                        //我们这里采用的办法是缓存一次，好处就是如果你某种原因，数据堆积在缓存buffer中
                        //已经很多了，那你需要循环的找到最后一组，只分析最新数据，过往数据你已经处理不及时
                        //了，就不要浪费更多时间了，这也是考虑到系统负载能够降低。
                        binary_data = new byte[len];
                        buffer.CopyTo(0, binary_data, 0, len);//复制一条完整数据到具体的数据缓存
                        data_catched = true;
                        buffer.RemoveRange(0, len);//正确分析一条数据，从缓存中移除数据。
                    }
                    else
                    {
                        //这里是很重要的，如果数据开始不是头，则删除数据
                        buffer.RemoveAt(0);
                    }
                }
                //分析数据
                if (data_catched)
                {
                    if (_DataReceivedEvent != null)
                    {
                        SerialPortEventArgs mSerialPortEventArgs = new SerialPortEventArgs();
                        mSerialPortEventArgs.Data = binary_data;
                        _DataReceivedEvent(this, mSerialPortEventArgs);
                    }
                }
                //如果需要别的协议，只要扩展这个data_n_catched就可以了。往往我们协议多的情况下，还会包含数据编号，给来的数据进行
                //编号，协议优化后就是： 头+编号+长度+数据+校验
                //</协议解析>
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            finally
            {
                Listening = false;//我用完了，ui可以关闭串口了。
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenPort()
        {
            if (comm.IsOpen)
            {
                return;
            }
            //关闭时点击，则设置好端口，波特率后打开
            comm.PortName = PortName;
            comm.BaudRate = BaudRate;
            try
            {
                comm.Open();
            }
            catch (Exception ex)
            {
                //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                comm = new SerialPort();
                //现实异常信息给客户。
                if (_ErrorEvent != null)
                {
                    SerialPortEventArgs mSerialPortEventArgs = new SerialPortEventArgs();
                    mSerialPortEventArgs.ErrorMessage = ex.Message;
                    _ErrorEvent(this, mSerialPortEventArgs);
                }
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort()
        {
            //根据当前串口对象，来判断操作
            if (comm.IsOpen)
            {
                Closing = true;
                while (Listening)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                //打开时点击，则关闭串口                
                ClearErrorEvent();
                ClearDataReceivedEvent();
                /*
                    System.Timers.Timer aTimer1 = new System.Timers.Timer(500);
                    aTimer1.Elapsed += ((object source, ElapsedEventArgs ElapsedEventArgs) =>
                    {
                        aTimer1.Enabled = false;
                        comm.Close();
                    });
                    aTimer1.Enabled = true;
                */
                comm.Close();
            }
        }

        /// <summary>
        /// 清楚所有的Event，为下次赋值做准备
        /// </summary>
        public void ClearAllEvent()
        {
            ClearErrorEvent();
            ClearDataReceivedEvent();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void SendCommand(byte[] buf)
        {
            //定义一个变量，记录发送了几个字节
            int n = 0;
            //16进制发送
            {
                comm.Write(buf, 0, buf.Length);
                //记录发送的字节数
                n = buf.Length;
            }

            send_count += n;//累加发送字节数
        }
    }
}
