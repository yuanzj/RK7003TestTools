using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class CheckTSReq : BaseProtocolImpl<CheckTSReq>
    {
        //支持结果设备类型：
        //0：所有设备都支持
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("reserveValue", 1, 8)]
        public byte[] reserveValue { get; set; }

        public override int GetCommand()
        {
            return Const.PC_CHECK_TS_REQ;
        }
    }
}
