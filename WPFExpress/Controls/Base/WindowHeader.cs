using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFExpress.Controls.Base
{
    /// <summary>
    /// 标题控件
    /// </summary>
    class WindowHeader : Label
    {
        Window win = null;
        bool canMax = false;//是否允许最大化
        public WindowHeader()
        {
            this.Loaded += WindowHeader_Loaded;
        }

        private void WindowHeader_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.win = Window.GetWindow(this);
            if (win != null)
            {

                win.MaxHeight = SystemParameters.WorkArea.Height;//限制最大化
            }
            switch (this.ThemeKey)
            {
                case WindowTheme.CanResizeWindowKey:
                    canMax = true;
                    break;
                case WindowTheme.CanMinimizeWindowKey:
                    canMax = false;
                    break;
                case WindowTheme.NoResizeWindowKey:
                    canMax = false;
                    break;
                case WindowTheme.NoneWindowKey:
                    canMax = false;
                    break;

            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

        }
        /**使用隧道事件后DragMove会抢导致双击事件失效**/

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (win == null)
                return;
            if (!canMax) return;//不允许最大化
            if (win.WindowState != WindowState.Maximized)
            {
                win.WindowState = WindowState.Maximized;

            }
            else
                win.WindowState = WindowState.Normal;

        }


        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (win == null) return;
            win.DragMove();
        }



        public WindowTheme ThemeKey
        {
            get { return (WindowTheme)GetValue(ThemeKeyProperty); }
            set { SetValue(ThemeKeyProperty, value); }
        }

        public static readonly DependencyProperty ThemeKeyProperty =
            DependencyProperty.Register("ThemeKey", typeof(WindowTheme), typeof(WindowHeader), new PropertyMetadata(WindowTheme.CanResizeWindowKey));



        //public Boolean CanMaximized
        //{
        //    get { return (Boolean)GetValue(CanMaximizedProperty); }
        //    set { SetValue(CanMaximizedProperty, value); }
        //}

        //public static readonly DependencyProperty CanMaximizedProperty =
        //    DependencyProperty.Register("CanMaximized", typeof(Boolean), typeof(WindowHeader), new PropertyMetadata(true));
    }
}
