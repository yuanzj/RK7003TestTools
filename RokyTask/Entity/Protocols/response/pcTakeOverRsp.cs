using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class pcTakeOverRsp : BaseProtocolImpl<pcTakeOverRsp>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("hardVare", 1, 1)]
        public int hardVare { get; set; }

        [ProtocolAttribute("firmVer", 2, 3)]
        public int firmVer { get; set; }

        public override int GetCommand()
        {
            return Const.PC_TAKEOVER_RSP;
        }


    }
}
