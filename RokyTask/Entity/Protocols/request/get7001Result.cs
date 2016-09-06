using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class get7001Result : BaseProtocolImpl<get7001Result>
    {
        [ProtocolAttribute("ecu_status", 0, 1)]
        public int ecu_status { get; set; }

        [ProtocolAttribute("remote_code", 1, 1)]
        public int remote_code { get; set; }

        [ProtocolAttribute("gear_level", 2, 1)]
        public int gear_level { get; set; }

        [ProtocolAttribute("limit_per", 3, 1)]
        public int limit_per { get; set; }

        [ProtocolAttribute("level_ctrl", 4, 2)]
        public int level_ctrl { get; set; }

        [ProtocolAttribute("trigger_ctrl", 6, 1)]
        public int trigger_ctrl { get; set; }

        [ProtocolAttribute("backlight", 7, 3)]
        public int backlight { get; set; }

        [ProtocolAttribute("batt_soc", 10, 1)]
        public int batt_soc { get; set; }

        public override int GetCommand()
        {
            return Const.GET_7001_RESULT;
        }
    }
}
