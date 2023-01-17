using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XFaceUpdateTool.Entitys
{
    /// <summary>
    /// 日志项
    /// </summary>
    class DownloadItem : INotifyPropertyChanged
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
        private string _file;
        /// <summary>
        /// 文件名与Json一致
        /// </summary>
        public string file
        {
            get
            {
                return _file;
            }
            set
            {
                _file = value;
                OnPropertyChanged("file");
            }
        }
        private int _sn;
        /// <summary>
        /// 包序列号
        /// </summary>
        public int sn
        {
            get
            {
                return _sn;
            }
            set
            {
                _sn = value;
                OnPropertyChanged("sn");

            }
        }
        private int _result;
        /// <summary>
        /// 下载结果1成功，0失败(默认)
        /// </summary>
        public int result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                OnPropertyChanged("result");

            }
        }

        private string _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string remark {
            get
            {
                return _remark;
            }
            set
            {
                _remark = value;
                OnPropertyChanged("remark");
            }
        }


    }
}
