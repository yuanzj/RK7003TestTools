using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class get4103BroadcastRsp : BaseProtocolImpl<get4103BroadcastRsp>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("hardwareID", 1, 1)]
        public int hardwareID { get; set; }

        [ProtocolAttribute("firmwareYears", 2, 1)]
        public int firmwareYears { get; set; }

        [ProtocolAttribute("firmwareWeeks", 3, 1)]
        public int firmwareWeeks { get; set; }

        [ProtocolAttribute("firmwareVersion", 4, 1)]
        public int firmwareVersion { get; set; }

        public override int GetCommand()
        {
            return Const.SEND_BROADCAST_RSP;
        }
    }
}
