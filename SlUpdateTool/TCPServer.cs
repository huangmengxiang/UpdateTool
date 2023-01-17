using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace XFaceUpdateTool
{
    /// <summary>
    /// TCP服务端
    /// </summary>
    public class TCPServer
    {
        /// <summary>
        /// 数据接收委托
        /// </summary>
        /// <param name="socket"></param>
        public delegate void DataReceivedHandler(Socket socket);
        /// <summary>
        /// 服务停止委托
        /// </summary>
        public delegate void ServerClosedHandler(TCPServer server);
        private bool _actived = false;//正在侦听
        private int _backlog = 100;//排队挂起数
        private int _receiveTimeout = 0;
        private int _sendTimeout = 0;

        private Socket _listener = null;
        private IPEndPoint localEP = null;//本地终结点
        /// <summary>
        /// 数据接收事件(由客户端关闭释放)
        /// </summary>
        public event DataReceivedHandler DataReceived = null;
        /// <summary>
        /// 服务停止事件
        /// </summary>
        public event ServerClosedHandler ServerClosed = null;

        /// <summary>
        /// 使用指定端口实例化TCPServer
        /// </summary>
        /// <param name="port">侦听端口</param>
        /// <param name="receiveTimeout">收超时毫秒</param>
        /// <param name="sendTimeout">发超时毫秒</param>
        public TCPServer(int port,int receiveTimeout = 120000,int sendTimeout=10000)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.localEP = new IPEndPoint(IPAddress.Any, port);
            this._receiveTimeout = receiveTimeout;
            this._sendTimeout = sendTimeout;
        }
        ///// <summary>
        ///// 使用指定IP、端口实例化TCPServer
        ///// </summary>
        ///// <param name="ip">本机IP</param>
        ///// <param name="port">侦听端口</param>
        //public TCPServer(string ip, int port)
        //{
        //    this.localEP = new IPEndPoint(IPAddress.Parse(ip), port);
        //}
        /// <summary>
        /// 获取侦听端口
        /// </summary>
        public int Port
        {
            get { return this.localEP.Port; }
        }
        /// <summary>
        /// 侦听消息(内含异步委托)
        /// </summary>
        /// <param name="backlog">允许最大请求挂起数</param>
        public bool Listen(int backlog = 100)
        {
            if (_listener == null)
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (this.localEP == null) return _actived;
            if (_actived) return _actived;
            try
            {
                this._backlog = backlog;
                _listener.Bind(this.localEP);
                _listener.Listen(backlog);
                _actived = true;
                //主线程异步执行
                Action action = () =>
                {
                    try
                    {
                        while (true)
                        {
                            if (!_actived) break;//请求停止信号
                            try
                            {
                                //执行回调
                                if (this.DataReceived != null)
                                {
                                    Socket client = _listener.Accept();//接收请求
                                    client.SendTimeout = this._sendTimeout;
                                    client.ReceiveTimeout = this._receiveTimeout;
                                    //AsyncCallback callBack = new AsyncCallback(ReceivedEnd);
                                    this.DataReceived.BeginInvoke(client, null, null);//异步执行回调
                                }

                            }
                            catch
                            {
                                Thread.Sleep(10);
                            }
                        }
                        _listener.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _listener.Close();
                    }
                    _actived = false;
                    //服务退出
                    if (ServerClosed != null)
                        ServerClosed(this);
                };
                action.BeginInvoke(null, null);//异步执行侦听
            }
            catch
            { }
            return _actived;
        }
        ///// <summary>
        ///// 客户端接收结束
        ///// </summary>
        ///// <param name="result"></param>
        //private void ReceivedEnd(IAsyncResult result)
        //{
        //    try
        //    {
        //        Socket socket = result.AsyncState as Socket;
        //        if (socket != null)
        //        {
        //            if (socket.Connected)
        //            {
        //                socket.Shutdown(SocketShutdown.Both);
        //                socket.Close();
        //            }
        //        }
        //    }
        //    catch { }
        //}

        ///// <summary>
        ///// 异步处理接收每一个请求的数据
        ///// </summary>
        ///// <param name="client"></param>
        ///// <param name="callback"></param>
        //private void ReceiveData(Socket client, NetNotice callback)
        //{
        //    ReceiveAction action = (clientEx, callbackEx) =>
        //    {
        //        byte[] bs = new byte[2048000];//2048kb
        //        int i = 0;
        //        IPEndPoint endPoint = clientEx.RemoteEndPoint as IPEndPoint;//客户端ip信息
        //        try
        //        {
        //            List<byte> data = new List<byte>();//指令缓存区(注意防止数据覆盖)
        //            clientEx.SendTimeout = this.TimeOut;
        //            clientEx.ReceiveTimeout = this.TimeOut;
        //            //this.WaitData(stream);
        //            try
        //            {
        //                try
        //                {
        //                    while ((i = clientEx.Receive(bs, 0, bs.Length, SocketFlags.None)) > 0)
        //                    {
        //                        if (_stopping) break;

        //                        if (i == bs.Length)//读满一个缓存区处理
        //                        {
        //                            data.AddRange(bs);
        //                        }
        //                        else//最后一次可能读不满缓存区处理
        //                        {
        //                            byte[] temp = new byte[i];
        //                            Array.Copy(bs, temp, i);
        //                            data.AddRange(temp);
        //                        }

        //                    }
        //                }
        //                catch { }//屏蔽每次读取超时异常
        //                if (!_stopping)
        //                    clientEx.WriteAck();//响应
        //                clientEx.Shutdown(SocketShutdown.Both);//不一定能执行到
        //            }
        //            catch { }
        //            clientEx.Close();//关闭连接
        //            if (!_stopping)
        //            {
        //                if (data.Count > 0)//接收完数据以回调方式处理
        //                {
        //                    try
        //                    {
        //                        if (callbackEx != null)
        //                        {
        //                            callbackEx.BeginInvoke(endPoint, data.ToArray(), null, null);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex1)
        //        {

        //        }
        //    };
        //    action.BeginInvoke(client, callback, null, null);
        //}
        /// <summary>
        /// 获取当前侦听器是否在工作
        /// </summary>
        public bool Actived
        {
            get { return _actived; }
        }
        /// <summary>
        /// 停止侦听器
        /// </summary>
        public void Stop()
        {
            _actived = false;
            try
            {
                _listener.Shutdown(SocketShutdown.Both);
            }
            catch { }
            finally
            {
                _listener.Close();
            }
            _listener = null;
        }
    }
}
