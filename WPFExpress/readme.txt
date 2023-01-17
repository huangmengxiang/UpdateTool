莫使用用户控件，会造成重新生成UI时，设计界面报错。必须cs与xaml文件人工分离
Turquoise：青绿色
Emerald：祖母绿; 翠绿色
Belizehole:河蓝色
Asphalt：沥青色
类库需要用到 Microsoft.Expression.Drawing.dll库
[ValueConversion(typeof(DateTime), typeof(String))]
public class DateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        DateTime date = (DateTime)value;
        return date.ToShortDateString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string strValue = value as string;
        DateTime resultDateTime;
        if (DateTime.TryParse(strValue, out resultDateTime))
        {
            return resultDateTime;
        }
        return DependencyProperty.UnsetValue;
    }
}



<TextBlock Grid.Row="2" Grid.Column="0" Margin="0,0,8,0"
           Name="startDateTitle"
           Style="{StaticResource smallTitleStyle}">Start Date:</TextBlock>
<TextBlock Name="StartDateDTKey" Grid.Row="2" Grid.Column="1" 
    Text="{Binding Path=StartDate, Converter={StaticResource dateConverter}}" 
    Style="{StaticResource textStyleTextBlock}"/>