using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class btTestReq : BaseProtocolImpl<btTestReq>
    {
        [ProtocolAttribute("edrResult", 0, 1)]
        public int edrResult { get; set; }

        [ProtocolAttribute("bleResult", 1, 1)]
        public int bleResult { get; set; }

        public override int GetCommand()
        {
            return Const.BT_TESTRESULT_REQ;
        }
    }
}
