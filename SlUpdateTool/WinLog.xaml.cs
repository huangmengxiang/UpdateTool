using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XFaceUpdateTool.Util;

namespace XFaceUpdateTool
{
    /// <summary>
    /// WinLog.xaml 的交互逻辑
    /// </summary>
    public partial class WinLog : Window
    {
        const int maxLen = 100;
        public WinLog()
        {
            InitializeComponent();
            this.Loaded += WinLog_Loaded;
        }

        private void WinLog_Loaded(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            LogWriter.Instance.Write += Instance_Write;
        }
        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="text"></param>
        private void Instance_Write(string text, bool toDisk, DateTime time)
        {
            try
            {
                listBox.Dispatcher.Invoke(new Action(() => {
                
                    if (listBox.Items.Count > maxLen)
                        listBox.Items.RemoveAt(maxLen - 1);//超过记录数删除最后一条记录
                    string formatText = string.Format("[{0}] {1}", time.ToString("HH:mm:ss:fff"), text);
                    //调试模式输出信息
                    listBox.Items.Insert(0, formatText);
                    //if (toDisk)//写到硬盘
                    //{
                    //    LogWriter.Instance.Save(formatText, time);
                    //}
                                     
                }));               
            }
            catch
            { }
        }
    }
}
