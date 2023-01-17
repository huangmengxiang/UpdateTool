using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// BorderResize.xaml 的交互逻辑
    /// </summary>
    partial class BorderResize : Border
    {
        double left = 0;
        double top = 0;
        double width = 0;
        double height = 0;
        double minWidth = SystemParameters.MinimizedWindowWidth;//最小宽度
        double minHeight = SystemParameters.MinimumWindowHeight;//最小高度
        Point srcPoint;
        Point desPoint;
        Window window = null;
        public BorderResize()
        {
            InitializeComponent();

            this.Init();
        }
        private void Init()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += BorderResize_Loaded;
                this.MouseLeftButtonDown += UBorderResize_MouseLeftButtonDown;
                this.MouseMove += UBorderResize_MouseMove;
                this.MouseLeftButtonUp += UBorderResize_MouseLeftButtonUp;
            }
        }

        void BorderResize_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
        }



        void UBorderResize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Released)
            {

                if (window != null)
                {
                    this.ReleaseMouseCapture();

                    desPoint = e.GetPosition(window);
                    if (this.Direction == DirectionType.E)
                    {
                        this.SizeE();
                    }
                    else if (this.Direction == DirectionType.S)
                    {
                        this.SizeS();
                    }
                    else if (this.Direction == DirectionType.W)
                    {
                        this.SizeW();
                    }
                    else if (this.Direction == DirectionType.N)
                    {
                        this.SizeN();
                    }
                    else if (this.Direction == DirectionType.SE)
                    {
                        this.SizeS();
                        this.SizeE();
                    }
                    else if (this.Direction == DirectionType.NE)
                    {
                        this.SizeN();
                        this.SizeE();
                    }
                    else if (this.Direction == DirectionType.NW)
                    {
                        this.SizeN();
                        this.SizeW();
                    }
                    else if (this.Direction == DirectionType.SW)
                    {
                        this.SizeS();
                        this.SizeW();
                    }


                    srcPoint = desPoint;

                }

            }

        }
        void UBorderResize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (window != null)
            {
                srcPoint = e.GetPosition(window);
                this.CaptureMouse();
            }
        }


        void UBorderResize_MouseMove(object sender, MouseEventArgs e)
        {

        }


        public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register("Direction",
        typeof(DirectionType),
        typeof(BorderResize),
        null
        );
        /// <summary>
        /// 方向
        /// </summary>
        public DirectionType Direction
        {
            set
            {
                SetValue(DirectionProperty, value);
            }
            get { return (DirectionType)GetValue(DirectionProperty); }
        }
        #region 调整句柄方法
        private void SizeS()
        {
            if (window != null)
            {
                height = window.Height + (desPoint.Y - srcPoint.Y);
                if (height < minHeight)
                    height = minHeight;
                window.Height = height;
            }
        }
        private void SizeE()
        {
            if (window != null)
            {
                width = window.Width + (desPoint.X - srcPoint.X);
                if (width < minWidth)
                    width = minWidth;

                window.Width = width;
            }
        }
        private void SizeW()
        {
            if (window != null)
            {
                double offset = desPoint.X - srcPoint.X;

                left = window.Left + offset;
                width = window.Width - offset;
                if (width < minWidth)
                    width = minWidth;
                //先X轴再宽度不闪烁
                window.Left = left;
                window.Width = width;

            }
        }
        private void SizeN()
        {
            if (window != null)
            {
                top = window.Top + (desPoint.Y - srcPoint.Y);
                height = window.Height - (desPoint.Y - srcPoint.Y);
                if (height < minHeight)
                    height = minHeight;
                //先Y轴再高度不闪烁
                window.Top = top;
                window.Height = height;
            }
        }
        #endregion
    }
    /// <summary>
    /// 方向枚举
    /// </summary>
    public enum DirectionType
    {
        None,
        /// <summary>
        /// 北
        /// </summary>
        N,
        /// <summary>
        /// 西
        /// </summary>
        W,
        /// <summary>
        /// 东
        /// </summary>
        E,
        /// <summary>
        /// 南
        /// </summary>
        S,
        /// <summary>
        /// 西北
        /// </summary>
        NW,
        /// <summary>
        /// 东北
        /// </summary>
        NE,
        /// <summary>
        /// 东南
        /// </summary>
        SE,
        /// <summary>
        /// 西南
        /// </summary>
        SW
    }
}
