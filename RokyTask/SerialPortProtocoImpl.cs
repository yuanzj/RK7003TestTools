using CommonUtils;
using Roky.SerialPortHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask
{
    //此串口对象 封装了一些 算法
    public class SerialPortProtocoImpl<T> : ISerialProtocol where T : class, IEntityProtocol, new()
    {

        public T Entity { get; set; }

        public bool CheckOK(List<byte> buf)
        {
            int _dataLength = DataLength(buf);
            if (_dataLength <= 0)
                return false;
            //和校验算法
            int sumValue = 0;
            for (int i = 0; i < (_dataLength - 1); i++)
            {
                sumValue += buf[i];
            }
            if (ByteProcess.intToByteArray(sumValue)[3] == buf[_dataLength - 1])
            {
                return true;
            }
            return false;
        }

        public int DataLength(List<byte> buf)
        {
            return buf[1];
        }

        public IEntityProtocol Decode(byte[] args)
        {
            byte Command = args[0];
            byte Length = args[1];

            byte[] Args = null;

            if ((args.Length - 3) > 0)
            {
                var bytesNew = new byte[args.Length - 3];
                Array.Copy(args, 2, bytesNew, 0, bytesNew.Length);

                Args = bytesNew;
            }

            byte CheckValue = args[args.Length - 1];

            if (null != Args)
            {
                T mT = new T();
                this.Entity = mT.Decode(Args) as T;
                return Entity;
            }
            else
            {
                return default(T);
            }

        }

        public byte[] Encode()
        {
            byte[] Args = this.Entity.Encode();
            int sum = 0;
            byte[] bytesNew = new byte[Args.Length + MinLength()];
            bytesNew[0] = (byte)Entity.GetCommand();
            bytesNew[1] = (byte)bytesNew.Length;

            sum += bytesNew[0];
            sum += bytesNew[1];
            for (int i = 0; i < Args.Length; i++)
            {
                bytesNew[2 + i] = Args[i];
                sum += Args[i];
            }
            bytesNew[bytesNew.Length - 1] = ByteProcess.intToByteArray(sum)[3];

            return bytesNew;
        }

        public int MinLength()
        {
            return 3;
        }

        public bool StartInProtocol(List<byte> buf)
        {
            T mT = new T();
            if (buf[0] == mT.GetCommand())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int StartLength()
        {
            return 1;
        }

        public int GetCommand()
        {
            if (this.Entity == null)
            {
                this.Entity = new T();
            }
            return this.Entity.GetCommand();
        }

    }
}
