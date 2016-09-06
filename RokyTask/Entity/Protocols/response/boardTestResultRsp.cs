using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class boardTestResultRsp : BaseProtocolImpl<boardTestResultRsp>
    {
        [ProtocolAttribute("msgType", 0, 1)]
        public int msgType { get; set; }

        public override int GetCommand()
        {
            return Const.BOARDTEST_RESULT_RSP;
        }
    }
}
