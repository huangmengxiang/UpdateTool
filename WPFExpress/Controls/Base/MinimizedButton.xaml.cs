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
    /// MinimizedButton.xaml 的交互逻辑
    /// </summary>
     partial class MinimizedButton : Button
    {
         Window win = null;

        public MinimizedButton()
        {
            InitializeComponent();
            this.Loaded += MinimizedButton_Loaded;
            this.Click += MinimizedButton_Click;
        }

        void MinimizedButton_Click(object sender, RoutedEventArgs e)
        {
            if (win != null)
            {
                
                win.WindowState = WindowState.Minimized;
            }
        }

        void MinimizedButton_Loaded(object sender, RoutedEventArgs e)
        {
            win = Window.GetWindow(this);

        }
    }
}
