using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XFaceUpdateTool
{
    /// <summary>
    /// WinSetting.xaml 的交互逻辑
    /// </summary>
    public partial class WinSetting : Window
    {
        public WinSetting()
        {
            InitializeComponent();
            btnSelect.Click += BtnSelect_Click;
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

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = dlg.SelectedPath;
            }
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadSettings()
        {
            txtVersion.Text = global::XFaceUpdateTool.Properties.Settings.Default.version;
            txtPath.Text = global::XFaceUpdateTool.Properties.Settings.Default.path;
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                string version = txtVersion.Text.Trim();
                string path = txtPath.Text.Trim();
                if (string.IsNullOrEmpty(version))
                {
                    System.Windows.MessageBox.Show("版本号不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (string.IsNullOrEmpty(path))
                {
                    System.Windows.MessageBox.Show("请选择升级路径！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                global::XFaceUpdateTool.Properties.Settings.Default.version = version;
                global::XFaceUpdateTool.Properties.Settings.Default.path = path;
                global::XFaceUpdateTool.Properties.Settings.Default.Save();//保存配置
                //global::XFaceUpdateTool.Properties.Settings.Default.Reload();//更新内存变量
                this.Close();
            }
            catch { }
        }
    }
}
