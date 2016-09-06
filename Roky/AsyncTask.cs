using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roky
{
    //抽象类，定义重写的方法  异步task
    public abstract class AsyncTask<Params, Progress, Result> : ITask<Params, Progress, Result> where Params : class, new() where Progress : class, new() where Result : class, new()
    {
        private BackgroundWorker backgroundWorker; //多线程后台处理的类

        private Exception mException;

        public string TAG { get; set; }

        public Params Param { get; set; }

        //这些要重写
        public abstract void OnPreExecute();

        public abstract Result DoInBackground(Params _Params);

        public abstract void OnProgressUpdate(params Progress[] _Params);

        public abstract void OnPostExecute(Result _Result, Exception _E);

        public AsyncTask()
        {
            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += bw_DoWork; //注册DoWork事件
            this.backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged; //注册ProgressChanged事件
            this.backgroundWorker.RunWorkerCompleted += bw_RunWorkerCompleted; //注册RunWorkerCompleted事件
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = DoInBackground(e.Argument as Params);
            }
            catch (Exception error)
            {
                mException = error;
            }

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgressUpdate(e.UserState as Progress);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPostExecute(e.Result as Result, mException);
        }

        public void PublishProgress(params Progress[] _Progress)
        {
            this.backgroundWorker.ReportProgress(-1, _Progress);
        }

        public void Excute()
        {
            OnPreExecute();
            mException = null;
            if (this.backgroundWorker.IsBusy)
            {
                CancelAsync();
            }
            this.backgroundWorker.RunWorkerAsync(Param);
        }

        public void CancelAsync()
        {
            this.backgroundWorker.CancelAsync();
        }

    }
}
