using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class ResetEcuReq : BaseProtocolImpl<ResetEcuReq>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("cmdCode", 1, 1)]
        public int cmdCode { get; set; }

        [ProtocolAttribute("param1", 2, 4)]
        public int param1 { get; set; }

        [ProtocolAttribute("param2", 6, 4)]
        public int param2 { get; set; }

        public override int GetCommand()
        {
            return Const.RESET_ECU_REQ;
        }
    }
}
