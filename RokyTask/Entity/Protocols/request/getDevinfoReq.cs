using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class getDevinfoReq : BaseProtocolImpl<getDevinfoReq>
    {
        [ProtocolAttribute("devType", 0, 1)]
        public int devType { get; set; }

        public override int GetCommand()
        {
            return Const.GET_DEVINFO_REQ;
        }
    }
}
