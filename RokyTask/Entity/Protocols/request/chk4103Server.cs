using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class chk4103Server : BaseProtocolImpl<chk4103Server>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        public override int GetCommand()
        {
            return Const.CHECK_4103_SERVER;
        }
    }
}
