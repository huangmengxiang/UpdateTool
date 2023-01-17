using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XFaceUpdateTool.Entitys;
using XFaceUpdateTool.Util;

namespace XFaceUpdateTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const int maxFile = 3000;//升级路径最大文件数量
        WinLog log = null;
        /// <summary>
        /// 侦听服务
        /// </summary>
        internal Host host = null;
        ObservableCollection<OnLineDevice> devices = new ObservableCollection<OnLineDevice>();

        DevicePool devPool = new DevicePool();

        public MainWindow()
        {
            InitializeComponent();
            host = new Host();
            host.PackageReceived += Host_PackageReceived;
            btnDebug.Click += BtnDebug_Click;
            btnUpdate.Click += BtnUpdate_Click;
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            chkAll.Click += ChkAll_Click;
            btnSetting.Click += BtnSetting_Click;
            btnSystem.Click += BtnSystem_Click;
            btnHelp.Click += BtnHelp_Click;
            btnSerch.Click += BtnSearch_Click;
            btnDeviceSearch.Click += BtnDeviceSearch_Click;
            btnDeviceAdd.Click += BtnDeviceAdd_Click;
            btnSearchPool.Click += BtnSearchPool_Click;
            this.InitVersion();
        }
        
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            WinSearch win = new WinSearch();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();
        }
        private void BtnDeviceAdd_Click(object sender, RoutedEventArgs e)
        {
            string deviceSn = btnDeviceSn.Text.ToUpper().Trim();
            if(devPool.addDevno(deviceSn) == true)
            {
                MessageBox.Show("添加成功！");
            }
            else
            {
                MessageBox.Show("添加失败！");
            }
        }

        private void BtnSearchPool_Click(object sender, RoutedEventArgs e)
        {
            string deviceSn = btnDeviceSn.Text.ToUpper().Trim();
            if (devPool.isValid(deviceSn) == true)
            {
                MessageBox.Show("在设备池中！");
            }
            else
            {
                MessageBox.Show("不在设备池中！");
            }
        }

        private void BtnDeviceSearch_Click(object sender, RoutedEventArgs e)
        {
            //btnDeviceSearch.
            /*
            WinSearch win = new WinSearch();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();
            */
           
           string deviceSn = btnDeviceSn.Text.ToUpper().Trim();
            int index= -1;
           
            for (int i = 0; i < devices.Count; i++) 
            {
                OnLineDevice device = devices[i];
                bool matched =deviceSn.Equals(device.Sn.ToUpper());
                if (matched)
                {
                    index = i;
                    break;
                }
            }

            if(index>=0)
            {
                if(index != 0)
                {
                    OnLineDevice dev0 = devices[0];
                    OnLineDevice devIndex = devices[index];
                    devices[0] = devIndex;
                    devices[index] = dev0;
                }

                MessageBox.Show("成功,更新到第一个");
            }
            else
            {
                MessageBox.Show("不在上线列表中");
            }
            //MessageBox.Show(deviceSn);
        } 
               
        private void TestExcel()
        {
            ExcelEdit myExcel = new ExcelEdit();
            myExcel.Create();
            myExcel.Open("e:\\device.xlsx");
            Microsoft.Office.Interop.Excel.Worksheet ws = myExcel.GetSheet("dev");

            int rowCount = 0;//有效行，索引从1开始
            try
            {
                int iRowCount = ws.UsedRange.Rows.Count;
                int iColCount = ws.UsedRange.Columns.Count;
                //生成列头
                bool hasTitle = false;
                string data = "";

                //生成行数据
                Microsoft.Office.Interop.Excel.Range range;
                int rowIdx = hasTitle ? 2 : 1;
                for (int iRow = 1; iRow <= iRowCount; iRow++)
                {
                //    DataRow dr = dt.NewRow();
                    for (int iCol = 1; iCol <= iColCount; iCol++)
                    {
                        range = (Microsoft.Office.Interop.Excel.Range)ws.Cells[iRow, iCol];
                        data = (range.Value2 == null) ? "" : range.Text.ToString();
                    }
                   // dt.Rows.Add(dr);
                }
              //  return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                
            }
            Console.WriteLine("ok");
        }


//       private DataTable TestMyExcel(bool hasTitle = false)
//{
//    OpenFileDialog openFile = new OpenFileDialog();
//    openFile.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
//    openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
//    openFile.Multiselect = false;

//    var excelFilePath = openFile.FileName;

//    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
//    Microsoft.Office.Interop.Excel.Sheets sheets;
//    object oMissiong = System.Reflection.Missing.Value;
//    Microsoft.Office.Interop.Excel.Workbook workbook = null;
//    DataTable dt = new DataTable();

//    try
//    {
//        if (app == null) return null;
//        workbook = app.Workbooks.Open(excelFilePath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
//            oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
//        sheets = workbook.Worksheets;

//        //将数据读入到DataTable中
//        Excel.Worksheet worksheet = (Excel.Worksheet)sheets.get_Item(1);//读取第一张表  
//        if (worksheet == null) return null;

//        int iRowCount = worksheet.UsedRange.Rows.Count;
//        int iColCount = worksheet.UsedRange.Columns.Count;
//        //生成列头
//        for (int i = 0; i < iColCount; i++)
//        {
//            var name = "column" + i;
//            if (hasTitle)
//            {
//                var txt = ((Excel.Range)worksheet.Cells[1, i + 1]).Text.ToString();
//                if (!string.IsNullOrWhiteSpace(txt)) name = txt;
//            }
//            while (dt.Columns.Contains(name)) name = name + "_1";//重复行名称会报错。
//            dt.Columns.Add(new DataColumn(name, typeof(string)));
//        }
//        //生成行数据
//        Excel.Range range;
//        int rowIdx = hasTitle ? 2 : 1;
//        for (int iRow = rowIdx; iRow <= iRowCount; iRow++)
//        {
//            DataRow dr = dt.NewRow();
//            for (int iCol = 1; iCol <= iColCount; iCol++)
//            {
//                range = (Excel.Range)worksheet.Cells[iRow, iCol];
//                dr[iCol - 1] = (range.Value2 == null) ? "" : range.Text.ToString();
//            }
//            dt.Rows.Add(dr);
//        }
//        return dt;
//    }
//    catch { return null; }
//    finally
//    {
//        workbook.Close(false, oMissiong, oMissiong);
//        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
//        workbook = null;
//        app.Workbooks.Close();
//        app.Quit();
//        System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
//        app = null;
//    }

//        }
        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            WinHelp win = new WinHelp();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();
        }

        private void BtnSystem_Click(object sender, RoutedEventArgs e)
        {
            WinSystem win = new WinSystem();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();
            this.SetStatusBar();//设置状态栏
        }

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            WinSetting win = new WinSetting();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();
            this.SetStatusBar();//设置状态栏
        }


        /// <summary>
        /// 软件升级 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.DoRequestUpdate();
        }

        //下载日志
        private void Download_Click(object sender, RoutedEventArgs e)
        {

            OnLineDevice device = dgDevice.SelectedItem as OnLineDevice;
            if (device != null)
            {
                WinDownload win = new WinDownload(device);
                win.Owner = this;
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                win.ShowDialog();
            }
        }
        private void BtnDebug_Click(object sender, RoutedEventArgs e)
        {
            if (log == null)
            {
                log = new WinLog();
                log.Owner = this;
                log.Closing += Log_Closing;
                log.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                log.Show();
            }
            else
            {
                log.WindowState = WindowState.Normal;//窗体变为正常
                log.Activate();
            }
        }
        private void InitVersion()
        {
            //程序集版本
            try
            {
                this.Title = string.Format("远程升级工具 V{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
            catch { }
        }
        private void Host_PackageReceived(Socket client, Package package)
        {
            try
            {
                if (package.Body != null)
                {
                    if (package.ContentType == ContentType.Beat)//心跳请求
                    {
                        this.DoBeat(client, package);
                    }
                    else if (package.ContentType == ContentType.PostFileInfo)//推送响应
                    {
                        //TODO 不处理
                    }
                    else if (package.ContentType == ContentType.GetFile)//拉取请求
                    {
                        this.DoGetFile(client, package);
                    }

                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite("处理协议异常" + ex.Message);
            }
        }

        /// <summary>
        /// 心跳处理
        /// </summary>
        private void DoBeat(Socket client, Package package)
        {
            int status = 0;//1 成功，2有升级版本，0失败
            string latestVersion = global::XFaceUpdateTool.Properties.Settings.Default.version;
            string remote = "";

            try
            {
                remote = client.RemoteEndPoint.ToString();
                string text = Encoding.UTF8.GetString(package.Body);
                LogWriter.Instance.OnWrite(string.Format("接收{0}心跳指令\r\n{1}", remote, package.ToString()));
                var json = JsonConvert.DeserializeObject<BeatRequest>(text);
                if (json != null)
                {
                    if (json.sn != null && json.version != null)//设备标识
                    {
                        if (devPool.isValid(json.sn))
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                OnLineDevice device = devices.FirstOrDefault(x => x.Sn == json.sn);
                                if (device == null)//未上线过
                                {
                                    device = new OnLineDevice();
                                    device.IP = client.RemoteEndPoint.ToString();
                                    device.Time = DateTime.Now.ToString();
                                    device.LatestVersion = latestVersion;

                                    device.DeviceVersion = json.version;
                                    device.Sn = json.sn;

                                    device.ProtocolVersion = package.Version;
                                    device.Magic = package.Magic;
                                    device.Seq = 1;//刚上线时默认1 

                                    device.Socket = client;

                                    devices.Add(device);


                                }
                                else
                                {
                                    device.IP = client.RemoteEndPoint.ToString();
                                    device.Time = DateTime.Now.ToString();
                                    device.LatestVersion = latestVersion;
                                    device.DeviceVersion = json.version;
                                    device.Sn = json.sn;
                                    device.ProtocolVersion = package.Version;
                                    device.Magic = package.Magic;
                                    //device.Seq = 1;重复上线时，seq不能变

                                    device.Socket = client;
                                }
                                //新版本字符串必须大于旧版本 且 未选中的设备不能升级
                                if (device.Selected && string.Compare(latestVersion.ToLower(), json.version.ToLower()) == 1)
                                {
                                    status = 2;
                                }

                                LogWriter.Instance.OnWrite(string.Format("{0} 心跳包解析成功", remote));

                            }));
                        }//is valid

                    }//

                }//json!=null
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite(string.Format("处理{0}心跳异常{1}", remote, ex.Message));
            }
            this.DoResponseBeat(client, package, status);

        }
        /// <summary>
        /// 批量下发软件升级请求
        /// </summary>
        private void DoRequestUpdate()
        {
            bool flag = false;
            try
            {
                string savePath = global::XFaceUpdateTool.Properties.Settings.Default.path;
                string latestVersion = global::XFaceUpdateTool.Properties.Settings.Default.version;

                if (string.IsNullOrEmpty(savePath))
                {
                    MessageBox.Show("请设置升级路径！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                //防止搞整个磁盘升级卡死
                if (Regex.IsMatch(savePath, @"^[a-zA-Z]\:\\{1,2}$"))
                {
                    MessageBox.Show("升级路径不能是磁盘根目录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                UpdateRequest data = new UpdateRequest();
                data.version = latestVersion;//最新版本

                data.list = this.GetFiles(savePath);
                if (data.list.Count > 0)
                {
                    if (data.list.Count > maxFile)
                    {
                        MessageBox.Show(string.Format("升级路径文件数量超过上限{0}！", maxFile), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    string text = JsonConvert.SerializeObject(data);//序列化为字符串
                    int i = devices.Count - 1;
                    //降序遍历，安全点
                    for (; i >= 0; i--)
                    {
                        OnLineDevice device = devices[i];
                        try
                        {
                            if (device.Selected)
                            {
                                flag = true;
                                if (device.Socket != null)
                                {
                                    Package package = new Package();
                                    device.Seq++;
                                    package.SN = device.Seq;
                                    package.Magic = device.Magic;
                                    package.ContentType = ContentType.PostFileInfo;//主动推送要更新文件
                                    package.Body = Encoding.UTF8.GetBytes(text);
                                    package.ContentLength = package.Body.Length;
                                    byte[] bs = package.ToBytes();
                                    device.Socket.Send(bs, SocketFlags.None);//发送响应     
                                    LogWriter.Instance.OnWrite(string.Format("已下发设备{0} 升级指令", device.Sn));

                                }
                            }
                        }
                        catch
                        {
                            LogWriter.Instance.OnWrite(string.Format("下发设备{0} 升级指令异常", device.Sn));
                        }
                    }
                    if (!flag)
                    {
                        MessageBox.Show("至少选择一台升级设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("升级路径文件数量空！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite("下发软件升级异常" + ex.Message);
            }

        }
        /// <summary>
        /// 响应拉取文件
        /// </summary>
        private void DoGetFile(Socket client, Package package)
        {
            bool flag = false;
            byte[] fileBytes = null;
            string remote = "";
            try
            {
                remote = client.RemoteEndPoint.ToString();
                string text = Encoding.UTF8.GetString(package.Body);
                LogWriter.Instance.OnWrite(string.Format("接收{0}拉取指令\r\n{1}",
                    remote,
                    package.ToString()));
                var json = JsonConvert.DeserializeObject<GetFileRequest>(text);
                if (json != null)
                {
                    if (json.path != null)
                    {
                        string rootPath = global::XFaceUpdateTool.Properties.Settings.Default.path;
                        string filePath = rootPath + json.path;//由于json.path带反斜杠，不能Path.Combine拼接
                        if (File.Exists(filePath))
                        {
                            //文件要可共享打开
                            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                fileBytes = new byte[fs.Length];
                                fs.Read(fileBytes, 0, fileBytes.Length);
                                flag = true;
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite(string.Format("处理{0}文件读取异常{1}", remote, ex.Message));
            }
            try
            {
                if (flag)
                {
                    package.Body = fileBytes;
                    package.ContentLength = fileBytes.Length;
                }
                else//文件不存在
                {
                    package.Body = null;
                    package.ContentLength = 0;
                }
                byte[] bs = package.ToBytes();
                client.Send(bs, SocketFlags.None);//发送响应
                LogWriter.Instance.OnWrite(string.Format("已响应{0}读取文件", remote));
            }
            catch (Exception ex2)
            {
                LogWriter.Instance.OnWrite(string.Format("响应{0}文件读取异常{1}", remote, ex2.Message));
            }
        }
        /// <summary>
        /// 心跳响应
        /// </summary>
        /// <param name="client"></param>
        /// <param name="package">心跳数据包</param>
        /// <param name="status">1 成功，2有升级版本，0失败</param>
        private void DoResponseBeat(Socket client, Package package, int status)
        {
            string remote = "";
            try
            {
                remote = client.RemoteEndPoint.ToString();

                if (status == 2)//有新版本
                {
                    string version = global::XFaceUpdateTool.Properties.Settings.Default.version;
                    string savePath = global::XFaceUpdateTool.Properties.Settings.Default.path;
                    List<FileItem> list = this.GetFiles(savePath);
                    //正确获取升级文件数量范围
                    if (list.Count > 0 && list.Count < maxFile)
                    {
                        var obj = new
                        {
                            result = status == 1 ? 1 : 0,
                            version = version,
                            list = list

                        };
                        string text = JsonConvert.SerializeObject(obj);//序列化为字符串
                        package.Body = Encoding.UTF8.GetBytes(text);
                        package.ContentLength = package.Body.Length;
                        byte[] bs = package.ToBytes();
                        client.Send(bs, SocketFlags.None);//发送响应
                        LogWriter.Instance.OnWrite(string.Format("已响应{0}心跳", remote));

                    }
                    else//超过范围不给升级
                    {
                        status = 0;
                    }
                }
                if (status == 0 || status == 1)
                {
                    var obj = new { result = status == 1 ? 1 : 0 };
                    string text = JsonConvert.SerializeObject(obj);//序列化为字符串
                    package.Body = Encoding.UTF8.GetBytes(text);
                    package.ContentLength = package.Body.Length;
                    byte[] bs = package.ToBytes();
                    client.Send(bs, SocketFlags.None);//发送响应
                    LogWriter.Instance.OnWrite(string.Format("已响应{0}心跳", remote));

                }

            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite(string.Format("响应{0}心跳异常{1}", remote, ex.Message));
            }
        }

        /// <summary>
        /// 获取更新文件列表
        /// </summary>
        /// <param name="path">本地文件路径</param>
        /// <returns></returns>
        private List<FileItem> GetFiles(string path)
        {
            List<FileItem> list = new List<FileItem>();
            try
            {
                if (Directory.Exists(path))
                {
                    int pathLen = path.Length;
                    string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            FileInfo info = new FileInfo(file);
                            FileItem item = new FileItem();
                            item.path = file.Substring(pathLen);//相对路径如 \abc\1.dll
                            item.size = (float)Math.Round((float)info.Length / 1024f, 3);
                            list.Add(item);
                        }
                    }
                }
            }
            catch { }
            return list;
        }
        /// <summary>
        /// 设置状态栏
        /// </summary>
        private void SetStatusBar()
        {
            try
            {
                lblPort.Text = "端口: " + global::XFaceUpdateTool.Properties.Settings.Default.port;
                lblPath.Text = "升级路径: " + global::XFaceUpdateTool.Properties.Settings.Default.path;
                lblVersion.Text = "升级版本: " + global::XFaceUpdateTool.Properties.Settings.Default.version;
                lbMode.Text = "模式：" + devPool.Mode;
                //刷新各上线设备的最新版本
                int n = devices.Count() - 1;
                for (; n >= 0; n--)
                {
                    devices[n].LatestVersion = global::XFaceUpdateTool.Properties.Settings.Default.version;
                }
            }
            catch { }

        }
        private void ChkAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in devices)
                {
                    item.Selected = chkAll.IsChecked == true ? true : false;
                }
            }
            catch { }
        }

        //列表项某一项不选中时，全选也不选中
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk.IsChecked == false)
                chkAll.IsChecked = false;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgDevice.ItemsSource = devices;

            if (!host.Start())
            {

                lblServer.Text = "服务启动失败";
                lblServer.Foreground = Brushes.Red;
            }
            else
            {
                lblServer.Text = "服务启动成功";
                lblServer.Foreground = Brushes.Green;
            }
            this.SetStatusBar();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            host.Stop();//关闭服务
        }


        private void Log_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            log = null;//先清空本地实例

        }

        private void dgDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSetting_Click_1(object sender, RoutedEventArgs e)
        {

        }

    }
}
