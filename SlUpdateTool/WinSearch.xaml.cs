using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using XFaceUpdateTool.Util;

namespace XFaceUpdateTool
{
    public enum UDP_PACKAGE_TYPE :uint
    {
	    TYPE_UDP_UNKNOWN = 0,
	    TYPE_UDP_RELOADCONFIG = 1,
	    TYPE_UDP_VERSION = 2,
	    TYPE_UDP_SEARCH = 3,
	    TYPE_UDP_DEVINFO = 4,
	    TYPE_UDP_HEARBEAT = 5,
        TYPE_UDP_SETSERVERIP = 6,

	    TYPE_UPDATE_GET_UPDATE_FILES = 0x100,
	
    };

    /// <summary>
    /// WinSearch.xaml 的交互逻辑
    /// </summary>
    public partial class WinSearch : Window
    {
        private UdpClient client;
        private IPEndPoint endpoint;
        private Boolean bExit;
        IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
        ObservableCollection<SearchDevice> searchDevs = new ObservableCollection<SearchDevice>();
        private const int MIN_UDP_PACAGESIZE = 12;
        public WinSearch()
        {
            InitializeComponent();
            bExit = false;
            this.Loaded += WinSearch_Loaded;
            this.Closed += WinSearch_Closed;
            this.btnReSearch.Click += btnReSearch_Click;
            btnOK.Click += btnOK_Click;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnReSearch_Click(object sender, RoutedEventArgs e)
        {
            SendSearchPackage();
        }

        private void ServerIp_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string number = (string)btn.Tag;
            SearchDevice dev = searchDevs.FirstOrDefault(x => x.number == number);
            if(dev != null)
            {
                WinSetDevServerIp win = new WinSetDevServerIp(dev);
                win.Owner = this;
                win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                win.ShowDialog();
            }
        }
        
        private void WinSearch_Closed(object sender, EventArgs e)
        {
            bExit = true;
        }

        private void WinSearch_Loaded(object sender, RoutedEventArgs e)
        {
            searDevs.ItemsSource = searchDevs;
            Loaded -= WinSearch_Loaded;
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 8888);//默认向全世界所有主机发送即可，路由器自动给你过滤，只发给局域网主机
           // String ip = "host:" + Dns.GetHostEntry(Dns.GetHostName()).AddressList.Last().ToString();//对外广播本机的ip地址
           // byte[] ipByte = Encoding.UTF8.GetBytes(ip);

           //创建无参的线程
            Thread thread1 = new Thread(new ThreadStart(ThreadPro));
            thread1.Start();

            SendSearchPackage();

        }

        private void ThreadPro()
        {
            byte[] recBuffer;
            //远端IP

            while (false == bExit)
            {
                try
                {
                    recBuffer = client.Receive(ref remotePoint);
                    if (recBuffer != null)
                    {
                        if (recBuffer.Length > MIN_UDP_PACAGESIZE)
                        {
                            ParsePackage(recBuffer);
                        }
                      //  string str = System.Text.Encoding.UTF8.GetString(recBuffer, 0, recBuffer.Length);
                       
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            client.Close();
        }

        private void ParsePackage(byte[] recBuffer)
        {
            if(recBuffer[0] == 'S'&&recBuffer[1] == 'L'&&recBuffer[2] == '8'&&recBuffer[3] == 'X')
            {
                byte[] tmp = new byte[4];
                Array.Copy(recBuffer, 4, tmp, 0, 4);
                Array.Reverse(tmp); //反转数组转成大端
                UInt32 type = BitConverter.ToUInt32(tmp, 0);           // 从字节数组转换成 int
                
                if (type == (UInt32)UDP_PACKAGE_TYPE.TYPE_UDP_SEARCH)
                {
                    Array.Copy(recBuffer, 8, tmp, 0, 4);
                    Array.Reverse(tmp); //反转数组转成大端
                    UInt32 length = BitConverter.ToUInt32(tmp, 0);           // 从字节数组转换成 int
                    string body = System.Text.Encoding.Default.GetString(recBuffer, 12, (int)length);
                    //body = "{\"number\":\"123\",\"ip\":\"192.168.1.2\"}";
                    var json = JsonConvert.DeserializeObject<SearchDevice>(body);
                    if (json != null)
                    {
                        if (json.number != null && json.number.Length > 0)//设备标识
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                SearchDevice device = searchDevs.FirstOrDefault(x => x.number == json.number);
                                if (device == null)//
                                {
                                    device = new SearchDevice();
                                    device.sockIp = remotePoint.Address.ToString();

                                    device.number = json.number;
                                    device.type = json.type;
                                    device.ip = json.ip;

                                    searchDevs.Add(device);
                                }

                                LogWriter.Instance.OnWrite(string.Format("{0} 设备搜索成功", device.number));

                            }));
                        }

                    }
                     
                }
            }
        }

        private void SendSearchPackage()
        {
            UdpDataPackage data = new UdpDataPackage();
            data.type = (UInt32)UDP_PACKAGE_TYPE.TYPE_UDP_SEARCH;
            data.length = 0;

            byte[] Bytes = data.ToBytes();
            client.Send(Bytes, Bytes.Length, endpoint);

        }

    }
}
