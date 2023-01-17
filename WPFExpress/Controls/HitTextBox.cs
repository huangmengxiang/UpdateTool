using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFExpress.Fonts;

namespace WPFExpress.Controls
{
    /// <summary>
    /// 带提示的文本框
    /// </summary>
    public class HitTextBox : TextBox
    {
        public HitTextBox()
        {
            
        }


        /// <summary>
        /// 提示信息
        /// </summary>
        public string HitText
        {
            get { return (string)GetValue(HitTextProperty); }
            set { SetValue(HitTextProperty, value); }
        }
        /// <summary>
        /// 提示信息依赖属性
        /// </summary>
        public static readonly DependencyProperty HitTextProperty =
            DependencyProperty.Register("HitText", typeof(string), typeof(HitTextBox), new PropertyMetadata(null));


        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set
            {
                SetValue(IconProperty, value);

            }
        }
        /// <summary>
        /// 字体图标依赖属性
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(HitTextBox), new PropertyMetadata(null));

    }
}
