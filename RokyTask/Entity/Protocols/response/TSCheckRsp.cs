using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity.Protocols.request
{
    class TSCheckRsp : BaseProtocolImpl<TSCheckRsp>
    {

        //支持结果设备类型：
        //0X11：新TESTSERVER
        [ProtocolAttribute("deviceType", 0, 1)]
        public int deviceType { get; set; }

        //0：成功接收，准备就绪
        //1：板测仪器未准备就绪
        //2：不支持测试
        [ProtocolAttribute("result", 1, 1)]
        public int result { get; set; }

        public override int GetCommand()
        {
            return Const.TS_CHECK_RSP;
        }
    }
}
