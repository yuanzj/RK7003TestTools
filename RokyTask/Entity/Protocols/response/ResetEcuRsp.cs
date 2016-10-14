using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class ResetEcuRsp : BaseProtocolImpl<ResetEcuRsp>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }        
        [ProtocolAttribute("RspType", 1, 1)]
        public int RspType { get; set; }

        [ProtocolAttribute("Parameter1", 2, 4)]
        public int Parameter1 { get; set; }

        [ProtocolAttribute("Parameter2", 6, 4)]
        public int Parameter2 { get; set; }

        public override int GetCommand()
        {
            return Const.RESET_ECU_RSP;
        }


    }
}
