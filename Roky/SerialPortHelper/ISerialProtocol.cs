using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky.SerialPortHelper
{
    public interface ISerialProtocol : IEntityProtocol
    {
        int MinLength();
        int StartLength();
        bool StartInProtocol(List<byte> buf);
        bool CheckOK(List<byte> buf);
        int DataLength(List<byte> buf);
    }
}
