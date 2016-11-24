using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class getDevinfoRsp : BaseProtocolImpl<getDevinfoRsp>
    {
        [ProtocolAttribute("devType", 0, 1)]
        public int devType { get; set; }

        [ProtocolAttribute("devSN", 1, 10)]
        public byte[] devSN { get; set; }

        [ProtocolAttribute("devBTaddr", 11, 6)]
        public byte[] devBTaddr { get; set; }

        [ProtocolAttribute("devBLEaddr", 17, 6)]
        public byte[] devBLEaddr { get; set; }

        [ProtocolAttribute("devKeyt", 23, 24)]
        public byte[] devKeyt { get; set; }

        [ProtocolAttribute("reserv", 47, 10)]
        public byte[] reserv { get; set; }

        public override int GetCommand()
        {
            return Const.GET_DEVINFO_RSP;
        }
    }
}
