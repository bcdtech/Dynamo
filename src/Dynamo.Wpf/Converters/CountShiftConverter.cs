using Dynamo.Utilities;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dynamo.Controls;

public class CountShiftConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length > 2)
        {
            try
            {

                var diagram = values[0] as FrameworkElement;
                var zoomValue = (double)values[1];
                var pageunit = (PageUnit)values[2];
                var unit = Unit.Cm;
                if (pageunit > PageUnit.km)
                {
                    unit = Unit.Inch;
                }
                Vector vector = System.Windows.Media.VisualTreeHelper.GetOffset(diagram);

                if (parameter?.ToString() == "Y")
                {
                    var value = 0 - (unit == Unit.Cm ? ScreenHelper.WidthToCm(vector.Y - 20) : ScreenHelper.WidthToInch(vector.Y - 20)) / zoomValue;
                    return value;
                }
                else
                {
                    var value = 0 - (unit == Unit.Cm ? ScreenHelper.WidthToCm(vector.X - 20) : ScreenHelper.WidthToInch(vector.X - 20)) / zoomValue;
                    return value;
                }
            }
            catch { }
        }
        return 0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
