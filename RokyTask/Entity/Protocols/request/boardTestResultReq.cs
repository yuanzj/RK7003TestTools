using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class boardTestResultReq : BaseProtocolImpl<boardTestResultReq>
    {
        [ProtocolAttribute("result", 0, 2)]
        public int result { get; set; }

        [ProtocolAttribute("inputPin", 2, 4)]
        public int inputPin { get; set; }

        [ProtocolAttribute("outpin1", 6, 1)]
        public int outpin1 { get; set; }

        [ProtocolAttribute("outpin2", 7, 1)]
        public int outpin2 { get; set; }

        [ProtocolAttribute("extensionOut", 8, 2)]
        public int extensionOut { get; set; }

        [ProtocolAttribute("reserv2", 10, 4)]
        public int reserv2 { get; set; }

        [ProtocolAttribute("adc1", 14, 4)]
        public int adc1 { get; set; }

        [ProtocolAttribute("adc2", 18, 4)]
        public int adc2 { get; set; }

        [ProtocolAttribute("lightsensor", 22, 2)]
        public int lightsensor { get; set; }

        [ProtocolAttribute("reserv3", 24, 2)]
        public int reserv3 { get; set; }

        public override int GetCommand()
        {
            return Const.BOARDTEST_RESULT_REQ;
        }
    }
}
