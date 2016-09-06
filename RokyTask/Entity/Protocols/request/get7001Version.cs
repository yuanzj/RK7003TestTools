using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class get7001Version : BaseProtocolImpl<get7001Version>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("battVolatage", 1, 1)]
        public int battVolatage { get; set; }

        [ProtocolAttribute("battCurrent", 2, 1)]
        public int battCurrent { get; set; }

        [ProtocolAttribute("underVoltage", 3, 1)]
        public int underVoltage { get; set; }

        public override int GetCommand()
        {
            return Const.GET_7001_VERSION;
        }
    }
}
