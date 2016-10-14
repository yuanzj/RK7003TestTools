using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class pcTakeoverReq : BaseProtocolImpl<pcTakeoverReq>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("firmVer", 1, 4)]
        public int firmVer { get; set; }

        [ProtocolAttribute("softVer", 5, 4)]
        public int softVer { get; set; }

        public override int GetCommand()
        {
            return Const.PC_TAKEOVER_REQ;
        }
    }
}
