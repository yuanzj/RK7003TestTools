using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class TSWriteSnStatusRsp : BaseProtocolImpl<TSWriteSnStatusRsp>
    {
        
        //0：查询写号状态
        //1：测试状态
        //2：查询遥控器状态
        //3：查询DCU状态
        //4：查询配置参数
        [ProtocolAttribute("checkType", 0, 1)]
        public int checkType { get; set; }

        //子状态	1	0X00~0XFF	0：正常 1：CCU 报文未停止 2：CCU未响应
        [ProtocolAttribute("subStatus", 1, 1)]
        public int subStatus { get; set; }

        //中控SN	14		缺省为0
        [ProtocolAttribute("sn", 2, 10)]
        public byte[] sn { get; set; }
        [ProtocolAttribute("sntemp", 12, 4)]
        public byte[] sntemp { get; set; }

        //BLE地址	6		缺省为0
        [ProtocolAttribute("bleAddr", 16, 6)]
        public byte[] bleAddr { get; set; }

        //鉴权码	24		缺省为0
        [ProtocolAttribute("key", 22, 24)]
        public byte[] key { get; set; }

        //保留	11		预留：0
        [ProtocolAttribute("reserve", 46, 11)]
        public byte[] reserve { get; set; }

        public override int GetCommand()
        {
            return Const.TS_CHECK_STATUS_RSP;
        }
    }
}
