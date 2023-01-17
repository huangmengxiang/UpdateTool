using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XFaceUpdateTool.Entitys;
using XFaceUpdateTool.Util;

namespace XFaceUpdateTool
{
    /// <summary>
    /// WinDownload.xaml 的交互逻辑
    /// </summary>
    public partial class WinDownload : Window
    {
        string downloadPath = "";
        OnLineDevice _device = null;
        ObservableCollection<DownloadItem> logs = new ObservableCollection<DownloadItem>();
        System.Timers.Timer timer = null;
        int waitSecond = 0;
        const string downloadInit = "等待下载";
        const string downloading = "正在下载";
        const string downloadOk = "下载成功";
        const string downloadFail = "下载失败";
        const string downloadTimeout = "下载超时";

        public WinDownload(OnLineDevice device)
        {
            InitializeComponent();
            _device = device;
            btnDownload.Click += BtnDownload_Click;
            this.Loaded += WinSetting_Loaded;
            btnSelect.Click += BtnSelect_Click;
        }
        /// <summary>
        /// 初始化下载路径
        /// </summary>
        private void InitPath()
        {
            try
            {
                downloadPath = global::XFaceUpdateTool.Properties.Settings.Default.downloadPath;
                if (!Directory.Exists(downloadPath))
                {
                    downloadPath = AppDomain.CurrentDomain.BaseDirectory;//若指定的路径不存在则设置为当前执行文件的路径
                }
                txtPath.Text = downloadPath;
            }
            catch { }
        }
        /// <summary>
        /// 初始化下载等待定时器
        /// </summary>
        private void InitTimer()
        {
            timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += (s, e) =>
            {
                try
                {
                    int count = logs.Count();
                    int finished = logs.Count(x => x.result == 1);//获取完成的数量

                    this.Dispatcher.Invoke(new Action(delegate
                    {

                        if (waitSecond > 0 && (finished != count))
                        {
                            tbWait.Text = string.Format("倒计时:{0}秒", waitSecond);
                            waitSecond--;
                        }
                        else
                        {
                            tbWait.Text = "";
                            timer.Enabled = false;//关闭定时器
                            btnDownload.IsEnabled = true;
                            btnSelect.IsEnabled = true;
                            foreach (var item in logs)
                            {
                                if (item.result != 1)//下载超时
                                {
                                    item.remark = downloadTimeout;
                                }
                            }
                        }
                    }));
                }
                catch { }
            };
        }
        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.downloadPath = dlg.SelectedPath;
                txtPath.Text = this.downloadPath;

            }
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = logs.Count;
                if (count < 1)
                {
                    System.Windows.MessageBox.Show("日志列表空！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                this.waitSecond = count * 60;//每个文件等待60秒
                foreach (var item in logs)
                {
                    item.result = 0;//复位
                    item.remark = downloadInit;
                }
                //请求下载日志
                if (_device != null)
                {
                    this.DoGetLogFileRequest(_device);
                }
            }
            catch { }
        }

        private void WinSetting_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitPath();
            this.InitTimer();
            dgLog.ItemsSource = logs;
            try
            {
                MainWindow win = App.Current.MainWindow as MainWindow;
                if (win != null)
                {
                    win.host.PackageReceived += Host_PackageReceived;
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite("注册服务事件异常" + ex.Message);
            }
            //请求日志列表
            if (_device != null)
            {
                this.DoGetLogInfoRequest(_device);
            }
        }

        private void Host_PackageReceived(System.Net.Sockets.Socket client, Package package)
        {
            try
            {
                if (package.Body != null)
                {
                    if (package.ContentType == ContentType.GetLogInfo)//获取日志列表客户端响应
                    {
                        this.DoGetLogInfoResponse(client, package);
                    }
                    else if (package.ContentType == ContentType.GetLogFile)//取日志文件响应
                    {
                        this.DoGetLogFileResponse(client, package);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite("处理协议异常" + ex.Message);
            }
        }
        /// <summary>
        /// 服务端请求获取日志列表
        /// </summary>
        private void DoGetLogInfoRequest(OnLineDevice device)
        {
            string remote = "";
            try
            {
                remote = device.Socket.RemoteEndPoint.ToString();

                Package package = new Package();
                device.Seq++;
                package.SN = device.Seq;
                package.Magic = device.Magic;
                package.ContentType = ContentType.GetLogInfo;
                package.Body = null;
                package.ContentLength = 0;
                byte[] bs = package.ToBytes();
                device.Socket.Send(bs, SocketFlags.None);//发送响应     
                LogWriter.Instance.OnWrite(string.Format("已下发{0}读取日志列表指令", remote));
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite(string.Format("下发{0}读取日志列表指令异常{1}", remote, ex.Message));
            }
        }
        /// <summary>
        /// 客户端响应获取日志列表
        /// </summary>
        /// <param name="client"></param>
        /// <param name="package"></param>
        private void DoGetLogInfoResponse(Socket client, Package package)
        {
            string remote = "";
            try
            {
                remote = client.RemoteEndPoint.ToString();

                string text = Encoding.UTF8.GetString(package.Body);
                LogWriter.Instance.OnWrite(string.Format("接收{0}日志响应\r\n{1}", remote, package.ToString()));
                var json = JsonConvert.DeserializeObject<GetLogResponse>(text);
                if (json != null)
                {
                    if (json.list != null)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            logs.Clear();
                            //日志列表
                            foreach (var item in json.list)
                            {
                                DownloadItem newItem = new DownloadItem();
                                newItem.file = item.file;
                                newItem.remark = downloadInit;
                                newItem.sn = 0;//此处暂不填写序号
                                logs.Add(newItem);

                            }
                        }));

                    }

                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite(string.Format("处理{0}日志响应异常{1}", remote, ex.Message));
            }
        }
        /// <summary>
        /// 服务端请求下载指定日志文件
        /// </summary>
        private void DoGetLogFileRequest(OnLineDevice device)
        {
            string remote = "";
            try
            {
                remote = device.Socket.RemoteEndPoint.ToString();
                foreach (var item in logs)
                {
                    Package package = new Package();
                    device.Seq++;
                    package.SN = device.Seq;
                    item.sn = device.Seq;//列表要记录序列号，文件返回时要比对
                    package.Magic = device.Magic;
                    package.ContentType = ContentType.GetLogFile;
                    var obj = new { file = item.file };//指定文件
                    string text = JsonConvert.SerializeObject(obj);//序列化为字符串
                    package.Body = Encoding.UTF8.GetBytes(text);
                    package.ContentLength = package.Body.Length;
                    byte[] bs = package.ToBytes();
                    device.Socket.Send(bs, SocketFlags.None);//发送响应   
                    item.remark = downloading;
                    LogWriter.Instance.OnWrite(string.Format("已下发{0}下载日志指令", remote));
                }
                btnDownload.IsEnabled = false;//禁用按钮
                btnSelect.IsEnabled = false;
                timer.Enabled = true;//启用定时器
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite(string.Format("下发{0}下载日志指令异常{1}", remote, ex.Message));
            }
        }
        /// <summary>
        /// 客户端响应下载日志
        /// </summary>
        /// <param name="client"></param>
        /// <param name="package"></param>
        private void DoGetLogFileResponse(Socket client, Package package)
        {
            string remote = "";
            bool flag = false;
            DownloadItem cur = null;
            try
            {
                remote = client.RemoteEndPoint.ToString();
                LogWriter.Instance.OnWrite(string.Format("接收{0}日志下载响应\r\n 文件长度{1}", remote, package.ContentLength));
                if (package.Body != null)
                {
                    //此处不需要加锁，由Socket.Receive解析成数据包后才会执行当前回调（异步线程中的同步），为安全可以加
                    foreach (var item in logs)
                    {
                        try
                        {
                            if (item.sn == package.SN)//sn一致
                            {
                                cur = item;
                                string savePath = System.IO.Path.Combine(downloadPath, item.file);//保存路径
                                //不独占写入
                                using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                                {
                                    fs.Write(package.Body, 0, package.ContentLength);
                                    fs.Flush();
                                    fs.Close();
                                }
                                flag = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogWriter.Instance.OnWrite(string.Format("保存{0}日志异常{1}", cur.file, ex.Message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite(string.Format("处理{0}日志响应异常{1}", remote, ex.Message));
            }
            //刷新下载结果
            try
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    if (cur != null)
                    {
                        cur.result = flag ? 1 : 0;//下载成功或失败
                        cur.remark = (cur.result == 1) ? downloadOk : downloadFail;
                    }
                }));

            }
            catch { }
        }
    }
}
