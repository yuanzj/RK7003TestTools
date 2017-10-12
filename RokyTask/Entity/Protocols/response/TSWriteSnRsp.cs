using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class TSWriteSnRsp : BaseProtocolImpl<TSWriteSnRsp>
    {
        [ProtocolAttribute("result", 0, 1)]
        public int result { get; set; }
        
        [ProtocolAttribute("reserve", 1, 10)]
        public byte[] reserve { get; set; }

        public override int GetCommand()
        {
            return Const.TS_WRITE_SN_RSP;
        }
    }
}
