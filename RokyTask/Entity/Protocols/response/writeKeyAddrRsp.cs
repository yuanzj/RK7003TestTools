using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class writeKeyAddrRsp : BaseProtocolImpl<writeKeyAddrRsp>
    {
        
        [ProtocolAttribute("Result", 0, 1)]
        public int Result { get; set; }

        [ProtocolAttribute("KeyNumber", 1, 1)]
        public int KeyNumber { get; set; }

        [ProtocolAttribute("Key1Index", 2, 1)]
        public int Key1Index { get; set; }

        [ProtocolAttribute("Key1Value", 3, 3)]
        public byte[] Key1Value { get; set; }

        [ProtocolAttribute("Key2Index", 6, 1)]
        public int Key2Index { get; set; }

        [ProtocolAttribute("Key2Value", 7, 3)]
        public byte[] Key2Value { get; set; }

        public override int GetCommand()
        {
            return Const.WRITE_KEY_RSP;
        }
    }
}
