using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class chk4103ServerRsp : BaseProtocolImpl<chk4103ServerRsp>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("status", 1, 1)]
        public int status { get; set; }

        public override int GetCommand()
        {
            return Const.CHECK_4103_SERVER_RSP;
        }
    }
}
