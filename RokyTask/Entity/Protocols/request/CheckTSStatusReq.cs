using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class CheckTSStatusReq : BaseProtocolImpl<CheckTSStatusReq>
    {
        
        //0：查询写号状态
        //1：测试状态
        //2：查询遥控器状态
        //3：查询DCU状态
        //4：查询配置参数
        [ProtocolAttribute("checkType", 0, 1)]
        public int checkType { get; set; }

        [ProtocolAttribute("reserveValue", 1, 10)]
        public byte[] reserveValue { get; set; }

        public override int GetCommand()
        {
            return Const.CHECK_TS_STATUS_REQ;
        }
    }
}
