using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class ParameterSettingReq : BaseProtocolImpl<ParameterSettingReq>
    {
        [ProtocolAttribute("status", 0, 1)]
        public int status { get; set; }

        public override int GetCommand()
        {
            return Const.PARAM_SETTING_REQ;
        }
    }
}
