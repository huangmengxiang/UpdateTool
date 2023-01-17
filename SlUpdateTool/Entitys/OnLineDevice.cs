using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XFaceUpdateTool.Entitys
{
    public class OnLineDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //[CallerMemberName]
        protected void OnPropertyChanged(string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        private bool _selected;
        /// <summary>
        /// 选中
        /// </summary>
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }
        private string _sn;
        /// <summary>
        /// 设备标识
        /// </summary>
        public string Sn
        {
            get
            {
                return _sn;
            }
            set
            {
                _sn = value;
                OnPropertyChanged("Sn");
            }
        }
        private string _deviceVersion;
        /// <summary>
        /// 设备版本
        /// </summary>
        public string DeviceVersion
        {
            get
            {
                return _deviceVersion;
            }
            set
            {
                _deviceVersion = value;
                OnPropertyChanged("DeviceVersion");
            }
        }
        private string _latestVersion;
        /// <summary>
        /// 最新版本
        /// </summary>
        public string LatestVersion
        {
            get
            {
                return _latestVersion;
            }
            set
            {
                _latestVersion = value;
                OnPropertyChanged("LatestVersion");
            }
        }

        private string _ip;
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                OnPropertyChanged("IP");
            }
        }

        private string _time;
        /// <summary>
        /// 上线时间
        /// </summary>
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }
        /// <summary>
        /// 通信对象
        /// </summary>
        public Socket Socket { set; get; }
        /// <summary>
        /// 通信协议版本(心跳时存储起来)
        /// </summary>
        public string ProtocolVersion { set; get; }
        /// <summary>
        /// 报文序列号(发送方控制递增)
        /// </summary>
        public int Seq { set; get; }
        /// <summary>
        /// 魔数(心跳时存储起来)
        /// </summary>
        public string Magic { set; get; }

    }
}
