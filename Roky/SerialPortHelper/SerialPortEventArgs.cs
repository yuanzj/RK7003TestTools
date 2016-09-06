using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky.SerialPortHelper
{
    public class SerialPortEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }

        public byte[] Data { get; set; }
    }
}
