using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class get7001Result : BaseProtocolImpl<get7001Result>
    {
        [ProtocolAttribute("ack_device", 0, 1)]
        public int ack_device { get; set; }

        [ProtocolAttribute("ecu_status", 1, 1)]
        public int ecu_status { get; set; }

        [ProtocolAttribute("server_mode", 2, 1)]
        public int server_mode { get; set; }

        [ProtocolAttribute("reserv", 3, 1)]
        public int reserv { get; set; }

        [ProtocolAttribute("limit_per", 4, 1)]
        public int limit_per { get; set; }

        [ProtocolAttribute("level_ctrl", 5, 2)]
        public int level_ctrl { get; set; }

        [ProtocolAttribute("trigger_ctrl", 7, 1)]
        public int trigger_ctrl { get; set; }

        [ProtocolAttribute("backlight", 8, 3)]
        public int backlight { get; set; }

        [ProtocolAttribute("batt_soc", 11, 1)]
        public int batt_soc { get; set; }

        public override int GetCommand()
        {
            return Const.GET_7001_RESULT;
        }
    }
}
