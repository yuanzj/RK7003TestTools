using CommonUtils;
using Roky.SerialPortHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky
{
    public abstract class SerialPortTask<Progress, R, T> : ITask<SerialPortTaskParams, Progress, T> where Progress : class, new() where R : class, IEntityProtocol, new() where T : class, IEntityProtocol, new()
    {

        public string TAG { get; set; }

        public ISerialProtocol RequestProtocol { get; set; }

        public byte[] recvByteArray;

        public ISerialProtocol MyProtocol { get; set; }

        public SerialPortTaskParams Param { get; set; }

        public abstract void OnPreExecute();

        public abstract void OnProgressUpdate(params Progress[] _Params);

        public abstract void OnPostExecute(T _Result, Exception _E);

        public void Excute()
        {
            OnPreExecute(); //会调用重写的方法
            SerialPortController mSerialPortController = SerialPortController.GetInstance();//这个类只会new一次
            mSerialPortController.PortName = Param.PortName;
            mSerialPortController.BaudRate = Param.BaudRate;
            mSerialPortController.Initialization((object sender, EventArgs e) =>
            {
                SerialPortEventArgs mSerialPortEventArgs = e as SerialPortEventArgs;
                //通知异常
                OnPostExecute(default(T), new Exception(mSerialPortEventArgs.ErrorMessage));
            }, (object sender, EventArgs e) =>
            {

                SerialPortEventArgs mSerialPortEventArgs = e as SerialPortEventArgs;
                recvByteArray = mSerialPortEventArgs.Data;
                if (null != MyProtocol && null != mSerialPortEventArgs)
                {
                    //会调用重写的方法
                    OnPostExecute(MyProtocol.Decode(mSerialPortEventArgs.Data) as T, null);
                }
                else
                {
                    //通知异常
                    OnPostExecute(default(T), new Exception("返回数据为空"));
                }

            }, MyProtocol);

            if (RequestProtocol != null && RequestProtocol.GetCommand() != SerialPortConst.COMMAND_EMPTY_VIRTUAL)
            {
                mSerialPortController.SendCommand(RequestProtocol.Encode());
                Console.WriteLine("-----------------send value-----------------\n" + Util.ToHexString(RequestProtocol.Encode()));
            }

        }

        public void ExitTask()
        {
            SerialPortController.GetInstance().ClosePort();
        }

        public void ClearAllEvent()
        {
            SerialPortController.GetInstance().ClearAllEvent();
        }


    }
}
