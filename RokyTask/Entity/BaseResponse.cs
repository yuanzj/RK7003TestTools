using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask.Entity
{
    public class BaseResponse<T>
    {
        public T data { get; set; }
        public int state { get; set; }
        public string message { get; set; }
    }
}
