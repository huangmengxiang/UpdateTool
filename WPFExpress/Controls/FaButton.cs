using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFExpress.Controls
{
    /// <summary>
    /// FontAwesomer矢量图标按钮
    /// </summary>
    public class FaButton:Button
    {
        public FaButton()
        {
           
        }
        /// <summary>
        /// 图标大小
        /// </summary>
        public int IconSize
        {
            get { return (int)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        /// <summary>
        /// 图标大小依赖属性
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(int), typeof(FaButton), new PropertyMetadata(16));

        /// <summary>
        /// 图标依靠
        /// </summary>
        public Dock IconDock
        {
            get { return (Dock)GetValue(IconDockProperty); }
            set { SetValue(IconDockProperty, value); }
        }
        /// <summary>
        /// 图标依靠依赖属性
        /// </summary>
        public static readonly DependencyProperty IconDockProperty =
            DependencyProperty.Register("IconDock", typeof(Dock), typeof(FaButton), new PropertyMetadata(Dock.Top));


    }
}
