using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XFaceUpdateTool.Util
{
    /// <summary>
    /// 日志输出类
    /// </summary>
    public class LogWriter
    {
        static readonly object lockThis = new object();
        static readonly object lockSave = new object();

        //AutoResetEvent autoEvent = new AutoResetEvent(false);

        //存储目录
        string dir = "logs";
        //存储存储
        string path = "";
        const int sleep = 60000;//每隔1分钟执行一次 
        const int keepDay = 7;//日志保留天数


        /// <summary>
        /// 日志委托
        /// </summary>
        /// <param name="text"></param>
        public delegate void WriteHandler(string text,bool toDisk,DateTime time);

        static LogWriter _instance = null;
        /// <summary>
        /// 日志事件
        /// </summary>
        public event WriteHandler Write = null;

        Thread thread;
        bool isStart = false;
        LogWriter()
        {
            this.InitPath();
            thread = new Thread(new ThreadStart(DoWork));
        }
        /// <summary>
        /// 初始化路径
        /// </summary>
        private void InitPath()
        {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// 获取访问实例
        /// </summary>
        public static LogWriter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockThis)
                    {
                        if (_instance == null)
                            _instance = new LogWriter();
                        return _instance;
                    }
                }
                else
                    return _instance;
            }
        }
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="toDisk">true 保存到硬盘,false默认</param>
        public void OnWrite(string text, bool toDisk = false)
        {
            DateTime time = DateTime.Now;
            //触发事件
            if (Write != null)
                Write(text, toDisk,DateTime.Now);
        }
        /// <summary>
        /// 保存日志到文件
        /// </summary>
        /// <param name="bs"></param>
        public void Save(string text,DateTime time )
        {
            try
            {
                lock (lockSave)
                {
                    byte[] bs = null;
                    if (!string.IsNullOrEmpty(text))
                    {
                        bs = Encoding.UTF8.GetBytes(text);
                    }
                    if (bs != null)
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string fileName = Path.Combine(path, DateTime.Now.ToString("yyyyMMdd") + ".txt");//文件名
                        using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            fs.Seek(0, SeekOrigin.End);
                            fs.Write(bs, 0, bs.Length);
                            fs.Flush();
                            fs.Close();
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 启动日志删除线程
        /// </summary>
        public void StartReclaim()
        {
            if (this.isStart) return;
            try
            {
                isStart = true;
                thread.Start();
            }
            catch { }
        }
        private void DoWork()
        {
            int tick = sleep;
            while (isStart)
            {
                if (tick < 1)
                {
                    tick = sleep;//恢复计数
                    try
                    {
                        string[] files = Directory.GetFiles(path, "*.txt", SearchOption.TopDirectoryOnly);

                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                if (!isStart) break;
                                try
                                {
                                    FileInfo info = new FileInfo(file);
                                    DateTime createTime = info.CreationTime.Date;
                                    if (createTime.AddDays(keepDay) < DateTime.Now.Date)//文件创建日期+保留天数结果 如果小于当前时间则删除
                                    {
                                        info.Delete();//删除文件
                                    }
                                }
                                catch { }//保证一个文件删除出错不影响后续文件删除
                            }
                        }
                    }
                    catch { }
                }
                tick -= 1000;
                Thread.Sleep(1000);
            }
            //autoEvent.Set();//设置终止状态

        }
        /// <summary>
        /// 停止日志删除线程
        /// </summary>
        public void StopReclaim()
        {
            isStart = false;
            //autoEvent.WaitOne(5000);//等待线程执行完成
        }
    }
}
