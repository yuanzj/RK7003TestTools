using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky
{
    //任务接口
    public interface ITask<Params, Progress, Result>
    {
        string TAG { get; set; }
        Params Param { get; set; }
        void Excute();
    }
}
