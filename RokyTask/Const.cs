using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask
{
    public class Const
    {
        //串口
        public static string COM_PORT = "COM1";

        public static int BAUD_RATE = 115200;

        public const int SERIAL_SUCCESS = 0;

        public const int MAX_TRY_COUNT = 150;
        //
        public const int GET_7001_RESULT = 0X28;

        public const int GET_7001_RESULT_RSP = 0XA8;

        public const int GET_7001_VERSION = 0X08;

        public const int GET_7001_VERSION_RSP = 0X88;

        public const int CHECK_4103_SERVER = 0X02;

        public const int CHECK_4103_SERVER_RSP = 0X82;

        public const int RECV_BROADCAST_REQ = 0X06;

        public const int SEND_BROADCAST_RSP = 0X86;

        public const int PARAM_SETTING_REQ = 0X03;

        public const int PARAM_SETTING_RSP = 0X83;

        public const int BOARDTEST_RESULT_REQ = 0X23;

        public const int BOARDTEST_RESULT_RSP = 0XA3;

        public const int SAVE_NV_REQ = 0X24;

        public const int SAVE_NV_RSP = 0XA4;

        public const int BT_TESTRESULT_REQ = 0X26;

        public const int BT_TESTRESULT_RSP = 0XA6;

        public const int PC_TAKEOVER_REQ = 0X06;

        public const int PC_TAKEOVER_RSP = 0X86;

        public const int RESET_ECU_REQ = 0X0A;

        public const int RESET_ECU_RSP = 0X92;


        public const byte TESTSERVER_KEY_PRESS = 0X01;

        public const byte TESTSERVER_KEY_RELEASE = 0X02;

        public const byte KEY_PRESS_MODE = 0X03;

        public const byte TESTSERVER_KSI_ON = 0X11;

        public const byte TESTSERVER_KSI_OFF = 0X12;

        public const byte REMOTE_CONNECTED = 0X21;

        public const byte REMOTE_NO_RECV = 0X22;

        public const byte REMOTE_CHECK_FAIL = 0X23;

        public const byte TESTSERVER = 0X01;

        public const byte PCU = 0X07;

    }
}
