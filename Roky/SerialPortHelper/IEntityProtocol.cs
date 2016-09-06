using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky.SerialPortHelper
{
    /// <summary>
    /// 用于解析参数成实体对象!!!!
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public interface IEntityProtocol
    {
        int GetCommand();
        byte[] Encode();
        IEntityProtocol Decode(byte[] args);
    }
}
