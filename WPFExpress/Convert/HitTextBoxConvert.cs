using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPFExpress.Convert
{
    /// <summary>
    /// 当文本框有输入内容时，提示信息不可见
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class HitTextBoxConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = value as string;
            if (!string.IsNullOrWhiteSpace(text))
                return Visibility.Collapsed;
            else
                return Visibility.Visible; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //不需要转换回来
            return DependencyProperty.UnsetValue;
        }
    }
}
