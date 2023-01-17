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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFExpress.Controls.Base
{
    /// <summary>
    /// MaximizedButton.xaml 的交互逻辑
    /// </summary>
     partial class MaximizedButton : Button
    {
        Window win = null;
        public MaximizedButton()
        {
            InitializeComponent();
            this.Loaded += MaximizedButton_Loaded;
            this.Click += MaximizedButton_Click;
        }

        void MaximizedButton_Click(object sender, RoutedEventArgs e)
        {
            if (win != null)
            {
                if (win.WindowState == WindowState.Maximized)
                {
                    win.WindowState = WindowState.Normal;
                }
                else if (win.WindowState == WindowState.Normal)
                {
                    win.WindowState = WindowState.Maximized;
                }
            }
        }

        void MaximizedButton_Loaded(object sender, RoutedEventArgs e)
        {
            win = Window.GetWindow(this);
            if (win != null)
            {
                win.StateChanged += win_StateChanged;
            }
        }

        void win_StateChanged(object sender, EventArgs e)
        {
            try
            {
                Rectangle _max = this.Template.FindName("_max", this) as Rectangle;

                if (win.WindowState == WindowState.Maximized)
                {
                    _max.Visibility = Visibility.Visible;
                }
                else if (win.WindowState == WindowState.Normal)
                {
                    _max.Visibility = Visibility.Collapsed;

                }
            }
            catch { }
        }
        ///// <summary>
        ///// 获取设置窗体是否最大化
        ///// </summary>
        //public bool IsMaximized
        //{
        //    get { return (bool)GetValue(IsMaximizedProperty); }
        //    set { SetValue(IsMaximizedProperty, value); }
        //}

        //public static readonly DependencyProperty IsMaximizedProperty =
        //    DependencyProperty.Register("IsMaximized", typeof(bool), typeof(MaximizedButton), new PropertyMetadata(false));

    }

}
