using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class WriteSnReq : BaseProtocolImpl<WriteSnReq>
    {
        [ProtocolAttribute("sn", 0, 10)]
        public byte[] sn { get; set; }

        [ProtocolAttribute("sntemp", 10, 4)]
        public byte[] sntemp { get; set; }

        [ProtocolAttribute("bleAddr", 14, 6)]
        public byte[] bleAddr { get; set; }

        [ProtocolAttribute("key", 20, 24)]
        public byte[] key { get; set; }

        [ProtocolAttribute("reserve", 44, 10)]
        public byte[] reserve { get; set; }

        public override int GetCommand()
        {
            return Const.PC_WRITE_SN_REQ;
        }
    }
}
