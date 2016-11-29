using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class pcTakeOverRsp : BaseProtocolImpl<pcTakeOverRsp>
    {
        [ProtocolAttribute("DeviceType", 0, 1)]
        public int DeviceType { get; set; }

        [ProtocolAttribute("HardWareID", 1, 1)]
        public int HardWareID { get; set; }

        [ProtocolAttribute("FirmID", 2, 3)]
        public int FirmID { get; set; }

        public override int GetCommand()
        {
            return Const.PC_TAKEOVER_RSP;
        }


    }
}
