using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class get7001ResultRsp : BaseProtocolImpl<get7001ResultRsp>
    {
        [ProtocolAttribute("ack_device", 0, 1)]
        public int ack_device { get; set; }

        [ProtocolAttribute("ack_value", 1, 1)]
        public int ack_value { get; set; }

        [ProtocolAttribute("AccStatus", 2, 1)]
        public int AccStatus { get; set; }

        [ProtocolAttribute("DeviceFault", 3, 1)]
        public int DeviceFault { get; set; }

        [ProtocolAttribute("DcCurrent", 4, 1)]
        public int DcCurrent { get; set; }

        [ProtocolAttribute("DcVoltage", 5, 1)]
        public int DcVoltage { get; set; }

        [ProtocolAttribute("CutError_1", 6, 1)]
        public int CutError_1 { get; set; }

        [ProtocolAttribute("CutError_2", 7, 1)]
        public int CutError_2 { get; set; }

        [ProtocolAttribute("ShortError_1", 8, 1)]
        public int ShortError_1 { get; set; }

        [ProtocolAttribute("ShortError_2", 9, 1)]
        public int ShortError_2 { get; set; }

        [ProtocolAttribute("RemoteCmd", 10, 1)]
        public int RemoteCmd { get; set; }

        public override int GetCommand()
        {
            return Const.GET_7001_RESULT_RSP;
        }
    }
}
