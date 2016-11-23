using Roky;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RokyTask
{
    public class PhoneTestTask : ITaskManager
    {
        #region 常量
        public int KeyNumber { get; set; }
        public bool bTaskRunning { get; set; }
        #endregion

        #region 注册事件
        public event EventHandler UpdateValidSNHandler;
        public event EventHandler BindKey1Handler;
        public event EventHandler BindKey2Handler;
        #endregion

        #region 构造函数
        public PhoneTestTask()
        {

        }
        #endregion

        #region 构造任务
        private void TaskBuilder()
        {

        }
        #endregion





        //执行任务
        public void ExcuteTask()
        {
            
        }
    }
}
