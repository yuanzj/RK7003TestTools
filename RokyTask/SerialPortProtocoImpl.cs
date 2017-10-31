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
            if (CRCITU.IsCrc16Good(buf.ToArray()))
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
            
            List<byte> buf = new List<byte>();
            buf.AddRange(new byte[] { (byte)Entity.GetCommand() , (byte)(Args.Length + MinLength()) });
            buf.AddRange(Args);
            byte[] crc = CRCITU.GetCrc16(buf.ToArray());
            buf.AddRange(new byte[] { crc [1], crc [0]});

            return buf.ToArray();
        }

        public int MinLength()
        {
            return 4;
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
