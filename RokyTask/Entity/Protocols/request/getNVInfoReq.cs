using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class getNVInfoReq : BaseProtocolImpl<getNVInfoReq>
    {


        public override int GetCommand()
        {
            return Const.BOARDTEST_RESULT_REQ;
        }
    }
}
