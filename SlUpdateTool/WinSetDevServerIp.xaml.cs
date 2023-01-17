using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XFaceUpdateTool
{
    /// <summary>
    /// SetDevServerIpWin.xaml 的交互逻辑
    /// </summary>
    public partial class WinSetDevServerIp : Window
    {
        private const int MIN_UDP_PACAGESIZE = 12;
        IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
        SearchDevice sDev;
        string ipv4; 
        IPAddress[] ipadrlist;
        public WinSetDevServerIp()
        {
            InitializeComponent();
            this.Loaded += winLoaded;
        }

        public WinSetDevServerIp(SearchDevice dev)
        {
            InitializeComponent();
            sDev = dev;
            this.Loaded += winLoaded;
        }

        private void btnCancle_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void winLoaded(object sender, RoutedEventArgs e)
        {
            btnOk.Click += btnOk_click;
            btnCancle.Click += btnCancle_click;
            string name = Dns.GetHostName();
            ipadrlist = Dns.GetHostAddresses(name); 
            foreach (IPAddress ipa in ipadrlist)  
            {  
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipv4 = ipa.ToString();
                }
                  //     Console.Writeline(ipa.ToString());  
            }
            if (sDev != null)
            {
                txNumber.Text = sDev.number;
            }
            if(ipv4 != null)
            {
                string[] arrTemp = ipv4.Split('.');
                ip0.Text = arrTemp[0];
                ip1.Text = arrTemp[1];
                ip2.Text = arrTemp[2];
                ip3.Text = arrTemp[3];
            }
        }

        private void btnOk_click(object sender, RoutedEventArgs e)
        {
            try
            {
                UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(sDev.sockIp), 8888);//
                client.Client.SendTimeout = 1000;
                client.Client.ReceiveTimeout = 1000;
                SendSetPackage(client, endpoint);
                try
                {
                    byte[] recBuffer = client.Receive(ref remotePoint);
                    if (recBuffer != null)
                    {
                        if (recBuffer.Length > MIN_UDP_PACAGESIZE)
                        {
                            ParsePackage(recBuffer);
                        }
                        //  string str = System.Text.Encoding.UTF8.GetString(recBuffer, 0, recBuffer.Length);

                    }
                }
                catch(Exception e0)
                {
                    Console.WriteLine( e0.Message);
                }


            }
            catch(Exception e1)
            {
                
            }

        }

        private void ParsePackage(byte[] recBuffer)
        {
            if (recBuffer[0] == 'S' && recBuffer[1] == 'L' && recBuffer[2] == '8' && recBuffer[3] == 'X')
            {
                byte[] tmp = new byte[4];
                Array.Copy(recBuffer, 4, tmp, 0, 4);
                Array.Reverse(tmp); //反转数组转成大端
                UInt32 type = BitConverter.ToUInt32(tmp, 0);           // 从字节数组转换成 int

                if (type == (UInt32)UDP_PACKAGE_TYPE.TYPE_UDP_SETSERVERIP)
                {
                    Array.Copy(recBuffer, 8, tmp, 0, 4);
                    Array.Reverse(tmp); //反转数组转成大端
                    UInt32 length = BitConverter.ToUInt32(tmp, 0);           // 从字节数组转换成 int
                    string body = System.Text.Encoding.Default.GetString(recBuffer, 12, (int)length);
                    try
                    {
                        int ret = int.Parse(body);
                        if(ret == 0)
                        {
                            MessageBox.Show("设置成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else 
                        {
                            MessageBox.Show("设置失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("设置返回异常！"+e.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    }

                }
            }
        }
        private void SendSetPackage(UdpClient client, IPEndPoint endpoint)
        {
            UdpDataPackage data = new UdpDataPackage();
            data.type = (UInt32)UDP_PACKAGE_TYPE.TYPE_UDP_SETSERVERIP;
            string ipaddr = ip0.Text + "." + ip1.Text + "." + ip2.Text + "." + ip3.Text;
            data.length = (UInt32)ipaddr.Length;
            data.Body = System.Text.Encoding.Default.GetBytes ( ipaddr );

            byte[] Bytes = data.ToBytes();
            client.Send(Bytes, Bytes.Length, endpoint);

        }
        private void textChange(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Regex rgx = new Regex(@"^(2[0-4]\d|25[0-5]|[0|1]?\d\d?|(2[0-4]\d|25[0-5]|[0|1]?\d\d?\.))$");
            string ss = tb.Text;
            if (!rgx.IsMatch(ss))
            {
                tb.Text = "";
            }
            else
            {
                int temp = ss.IndexOf(".");
                if (temp > -1)
                {
                    ss = ss.Replace(".", "");
                    tb.Text = ss;
                    FrameworkElement fsource = e.Source as FrameworkElement;
                    fsource.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                if (ss.Length >= 3)
                {
                    FrameworkElement fsource = e.Source as FrameworkElement;
                    fsource.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }

    }
}
