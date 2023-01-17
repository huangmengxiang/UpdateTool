using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XFaceUpdateTool.Util;

namespace XFaceUpdateTool
{
    /// <summary>
    /// 数据管理类
    /// </summary>
    public class Host
    {
        TCPServer server = null;
        public const string version = "Version";
        public const string magic = "Magic";
        public const string sn = "SN";
        public const string contentType = "Content-Type";
        public const string contentLength = "Content-Length";
        const int versionSize = 7;
        public delegate void PackageReceivedHandler(Socket client, Package package);
        /// <summary>
        /// 数据包接收完事件
        /// </summary>
        public event PackageReceivedHandler PackageReceived = null;
        public Host()
        {

        }
        private bool InitServer()
        {
            bool flag = false;
            if (server != null) return flag;
            try
            {
                int port = global::XFaceUpdateTool.Properties.Settings.Default.port;
                int receiveTimeout = global::XFaceUpdateTool.Properties.Settings.Default.receiveTimeout;
                int sendTimeout = global::XFaceUpdateTool.Properties.Settings.Default.sendTimeout;

                server = new TCPServer(port, receiveTimeout, sendTimeout);
                server.DataReceived += Server_DataReceived;
                server.ServerClosed += Server_ServerClosed;
                server.Listen();
                flag = true;
            }
            catch
            {

            }
            return flag;
        }

        private void Server_ServerClosed(TCPServer server)
        {
            LogWriter.Instance.OnWrite("服务线程退出");
        }

        private void Server_DataReceived(Socket socket)
        {
            byte[] bs = new byte[10240];//10kb
            int readLen = 0;
            List<byte> buffer = new List<byte>();
            Package package = null;
            bool isNewed = true;//true 等待新数据包，false处理协议头  
            int i = 0;

            try
            {
                while ((readLen = socket.Receive(bs, 0, bs.Length, SocketFlags.None)) > 0)
                {
                    i = 0;
                    #region 等待新数据包
                    if (isNewed)
                    {
                        byte[] verBytes = new byte[versionSize];
                        for (; i < readLen - versionSize; i++)
                        {
                            if (readLen >= versionSize)
                            {
                                Array.Copy(bs, i, verBytes, 0, versionSize);
                                string ver = Encoding.UTF8.GetString(verBytes);
                                if (ver == version)//接收到新数据包
                                {
                                    buffer.Clear();
                                    buffer.AddRange(verBytes);
                                    i += versionSize;//下标下移
                                    package = new Package();
                                    isNewed = false;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 处理协议头及内容

                    for (; i < readLen - 1; i++)
                    {
                        if (bs[i] == 0x0D && bs[i + 1] == 0x0A)//回车换行 一行结束
                        {
                            #region 处理一行数据
                            string text = Encoding.UTF8.GetString(buffer.ToArray());
                            buffer.Clear();//立马清理掉
                            if (!string.IsNullOrEmpty(text))
                            {
                                //提取协议头信息
                                string[] arr = text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr.Length > 1)
                                {
                                    switch (arr[0])
                                    {
                                        case version:
                                            package.Version = arr[1];
                                            i++;//跳过回车换行
                                            break;
                                        case magic:
                                            package.Magic = arr[1];
                                            i++;//跳过回车换行
                                            break;
                                        case sn:
                                            package.SN = int.Parse(arr[1]);
                                            i++;//跳过回车换行
                                            break;
                                        case contentType:
                                            package.ContentType = arr[1];//格式File&type:xx&describe:xxx
                                            i++;//跳过回车换行
                                            break;
                                        case contentLength:
                                            {
                                                int bodyLen = 0;
                                                int.TryParse(arr[1], out bodyLen);
                                                if (bodyLen > 0)
                                                {
                                                    package.ContentLength = bodyLen;//格式File&type:xx&describe:xxx
                                                    package.Body = new byte[bodyLen];//开辟内存空间
                                                    i += 4;//跳过双回车换行
                                                    int dataLen = 0; //剩余数据长度
                                                    if (i + bodyLen > readLen)//剩余数据长度小于内容长度
                                                    {
                                                        dataLen = readLen - i;
                                                        Array.Copy(bs, i, package.Body, 0, dataLen);//复制内容

                                                        //接收剩余未读取的数据
                                                        while (bodyLen > dataLen)
                                                        {
                                                            int nextLen = socket.Receive(package.Body, dataLen, bodyLen - dataLen, SocketFlags.None);
                                                            if (nextLen + dataLen >= bodyLen)//两次读取合并长度与内容长度一致
                                                            {
                                                                if (this.PackageReceived != null)
                                                                    this.PackageReceived(socket, package);//回调
                                                                break;
                                                            }else
                                                            {
                                                                dataLen += nextLen;//长度累加
                                                            }
                                                            
                                                        }
                                                    }
                                                    else//剩余数据长度>=内容长度直接复制就行，多余丢弃
                                                    {
                                                        dataLen = bodyLen;
                                                        Array.Copy(bs, i, package.Body, 0, dataLen);//复制内容
                                                        if (this.PackageReceived != null)
                                                            this.PackageReceived(socket, package);//回调
                                                    }
                                                }
                                                isNewed = true;//等待新数据包到来
                                                break;
                                            }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            buffer.Add(bs[i]);//追加
                        }
                    }



                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.OnWrite("处理连接异常" + ex.Message);
            }
            //最后N分钟没数据关闭连接
            try
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                //socket.Dispose();
            }
            catch
            { }
        }



        public bool Start()
        {
            //LogWriter.Instance.StartReclaim();
            return this.InitServer(); //启动函数不要加日志输出           
        }
        public void Stop()
        {
            LogWriter.Instance.OnWrite("服务已停止");
            //LogWriter.Instance.StopReclaim();
            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }
    }
}
