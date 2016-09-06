using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class ParameterSettingRsp : BaseProtocolImpl<ParameterSettingRsp>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("testItem", 1, 2)]
        public int testItem { get; set; }

        [ProtocolAttribute("sn", 3, 10)]
        public byte[] sn { get; set; }

        [ProtocolAttribute("sntemp", 13, 4)]
        public byte[] sntemp { get; set; }

        [ProtocolAttribute("edrAddr", 17, 6)]
        public byte[] edrAddr { get; set; }

        [ProtocolAttribute("bleAddr", 23, 6)]
        public byte[] bleAddr { get; set; }

        [ProtocolAttribute("key", 29, 24)]
        public byte[] key { get; set; }

        [ProtocolAttribute("adcParam", 53, 4)]
        public byte[] adcParam { get; set; }

        [ProtocolAttribute("v15Param", 57, 4)]
        public byte[] v15Param { get; set; }


        public override int GetCommand()
        {
            return Const.PARAM_SETTING_RSP;
        }
    }
}
