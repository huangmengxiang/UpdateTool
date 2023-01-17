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
    /// ClosedButton.xaml 的交互逻辑
    /// </summary>
     partial  class ClosedButton : Button
    {
        public ClosedButton()
        {
            InitializeComponent();
            this.Click += ClosedButton_Click;
        }

        void ClosedButton_Click(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            if (win != null)
            {
                win.Close();
            }
        }
    }
}
