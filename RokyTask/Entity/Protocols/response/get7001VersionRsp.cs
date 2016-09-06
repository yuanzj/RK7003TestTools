using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.response
{
    class get7001VersionRsp : BaseProtocolImpl<get7001VersionRsp>
    {
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        [ProtocolAttribute("uid1", 1, 1)]
        public int uid1 { get; set; }
        [ProtocolAttribute("uid2", 2, 1)]
        public int uid2 { get; set; }
        [ProtocolAttribute("uid3", 3, 1)]
        public int uid3 { get; set; }
        [ProtocolAttribute("uid4", 4, 1)]
        public int uid4 { get; set; }
        [ProtocolAttribute("uid5", 5, 1)]
        public int uid5 { get; set; }
        [ProtocolAttribute("uid6", 6, 1)]
        public int uid6 { get; set; }
        [ProtocolAttribute("uid7", 7, 1)]
        public int uid7 { get; set; }
        [ProtocolAttribute("uid8", 8, 1)]
        public int uid8 { get; set; }
        [ProtocolAttribute("uid9", 9, 1)]
        public int uid9 { get; set; }
        [ProtocolAttribute("uid10", 10, 1)]
        public int uid10 { get; set; }
        [ProtocolAttribute("uid11", 11, 1)]
        public int uid11 { get; set; }
        [ProtocolAttribute("uid12", 12, 1)]
        public int uid12 { get; set; }

        [ProtocolAttribute("hw1", 13, 1)]
        public int hw1 { get; set; }
        [ProtocolAttribute("hw2", 14, 1)]
        public int hw2 { get; set; }
        [ProtocolAttribute("hw3", 15, 1)]
        public int hw3 { get; set; }
        [ProtocolAttribute("hw4", 16, 1)]
        public int hw4 { get; set; }

        [ProtocolAttribute("sw1", 17, 1)]
        public int sw1 { get; set; }
        [ProtocolAttribute("sw2", 18, 1)]
        public int sw2 { get; set; }
        [ProtocolAttribute("sw3", 19, 1)]
        public int sw3 { get; set; }
        [ProtocolAttribute("sw4", 20, 1)]
        public int sw4 { get; set; }


        [ProtocolAttribute("Reserver", 21, 1)]
        public int Reserver { get; set; }

        public override int GetCommand()
        {
            return Const.GET_7001_VERSION_RSP;
        }


    }
}
