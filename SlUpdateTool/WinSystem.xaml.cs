using System;
using System.Collections.Generic;
using System.Linq;
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
    /// WinSystem.xaml 的交互逻辑
    /// </summary>
    public partial class WinSystem : Window
    {
        public WinSystem()
        {
            InitializeComponent();
            btnSave.Click += BtnSave_Click;
            this.Loaded += WinSetting_Loaded;

        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.SaveSettings();
        }

        private void WinSetting_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadSettings();
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadSettings()
        {
            txtPort.Text = global::XFaceUpdateTool.Properties.Settings.Default.port.ToString();
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                string port = txtPort.Text.Trim();
                if (!Regex.IsMatch(port, @"^\d{1,5}$"))
                {
                    System.Windows.MessageBox.Show("服务端口不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                global::XFaceUpdateTool.Properties.Settings.Default.port = int.Parse(port);
                global::XFaceUpdateTool.Properties.Settings.Default.Save();//保存配置
                this.Close();
            }
            catch { }
        }

    }
}
