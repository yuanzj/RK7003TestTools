using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class get7001ResultRsp : BaseProtocolImpl<get7001ResultRsp>
    {
        [ProtocolAttribute("AccStatus", 0, 1)]
        public int AccStatus { get; set; }

        [ProtocolAttribute("DeviceFault", 1, 1)]
        public int DeviceFault { get; set; }

        [ProtocolAttribute("DcCurrent", 2, 1)]
        public int DcCurrent { get; set; }

        [ProtocolAttribute("PcuTemper", 3, 1)]
        public int PcuTemper { get; set; }

        [ProtocolAttribute("CutError", 4, 2)]
        public int CutError { get; set; }

        [ProtocolAttribute("ShortError", 6, 2)]
        public int ShortError { get; set; }

        public override int GetCommand()
        {
            return Const.GET_7001_RESULT_RSP;
        }
    }
}
