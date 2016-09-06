using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky
{
    public class SerialPortTaskParams
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }

        public SerialPortTaskParams(string _PortName, int _BaudRate)
        {
            this.PortName = _PortName;
            this.BaudRate = _BaudRate;
        }
    }
}
