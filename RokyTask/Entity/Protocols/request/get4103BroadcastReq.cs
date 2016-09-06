using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class get4103BroadcastReq : BaseProtocolImpl<get4103BroadcastReq>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("hardwareID", 1, 4)]
        public int hardwareID { get; set; }

        [ProtocolAttribute("softwareID", 5, 4)]
        public int softwareID { get; set; }


        public override int GetCommand()
        {
            return Const.RECV_BROADCAST_REQ;
        }
    }
}
