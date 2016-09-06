using Roky;
using Roky.SerialPortHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RokyTask
{
    public class SimpleSerialPortTask<R, T> : SerialPortTask<SimpleTaskProgress, R, T> where R : class, IEntityProtocol, new() where T : class, IEntityProtocol, new()
    {
        private const int TIME_OUT_TICKED = 1500;

        private const int RETRY_MAX_COUNT = 3;

        public volatile int retry_count;

        private volatile bool ok;

        public bool EnableTimeOutHandler { get; set; }

        public int Timerout { get; set; }//超时时间

        public int RetryMaxCnts { get; set; }

        public event EventHandler SimpleSerialPortTaskOnPostExecute;

        System.Timers.Timer aTimer1;

        public SimpleSerialPortTask()
        {
            //SerialPortProtocoImpl放在SerialPortTask中实例化
            base.MyProtocol = new SerialPortProtocoImpl<T>();
            SerialPortProtocoImpl<R> _RequestProtocol = new SerialPortProtocoImpl<R>();
            _RequestProtocol.Entity = new R();
            base.RequestProtocol = _RequestProtocol;

            base.Param = new SerialPortTaskParams(Const.COM_PORT, Const.BAUD_RATE);

            this.EnableTimeOutHandler = true;

            this.Timerout = TIME_OUT_TICKED;

            this.RetryMaxCnts = RETRY_MAX_COUNT;
        }
        //new 修饰符，可以显式隐藏从基类继承的成员: 此方法不想从基类SerialPortTask里继承，所以用new修饰符
        public new void Excute()
        {
            if (EnableTimeOutHandler)
            {
                if (aTimer1 != null)
                {
                    aTimer1.Enabled = false;
                    aTimer1 = null;
                }
                aTimer1 = new System.Timers.Timer(this.Timerout);
                aTimer1.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs ElapsedEventArgs) =>
                {
                    //  Console.WriteLine("Retry_count: " + retry_count + "TIME:"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));                   
                    if (ok)
                    {
                        aTimer1.Enabled = false;
                        retry_count = 0;
                    }
                    else
                    {
                        Console.WriteLine("Excute:TIME_OUT_Handler");

                        if (retry_count >= this.RetryMaxCnts)
                        {
                            retry_count = 0;
                            aTimer1.Enabled = false;
                            //通知异常,继承的
                            OnPostExecute(default(T), new Exception("设备没有响应"));
                        }
                        else
                        {
                            base.Excute();
                            retry_count++;
                        }
                    }
                });
                aTimer1.Enabled = true;
            }
            base.Excute();
        }

        public override void OnPostExecute(T _Result, Exception _E)
        {
            ok = true;
            retry_count = 0;
            if (SimpleSerialPortTaskOnPostExecute != null)
            {
                SerialPortEventArgs<T> mSerialPortEventArgs = new SerialPortEventArgs<T>();
                mSerialPortEventArgs.Data = _Result;
                mSerialPortEventArgs.Error = _E;
                SimpleSerialPortTaskOnPostExecute(this, mSerialPortEventArgs);
            }
        }

        public override void OnPreExecute()
        {
            ok = false;
        }

        public override void OnProgressUpdate(params SimpleTaskProgress[] _Params)
        {

        }

        public R GetRequestEntity()
        {
            return ((SerialPortProtocoImpl<R>)this.RequestProtocol).Entity;
        }
    }

    public class SerialPortEventArgs<T> : EventArgs where T : class, IEntityProtocol, new()
    {
        public T Data { get; set; }
        public Exception Error { get; set; }
    }
}
