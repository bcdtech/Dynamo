using Dynamo.Graph.Nodes;
using System.Globalization;
using System.Windows.Data;

namespace Dynamo.Controls
{
    public class OutPortAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var alignment = (PortAlignment)value;
            return alignment switch
            {
                PortAlignment.Right => 0,
                PortAlignment.Bottom => 90,
                PortAlignment.Left => 180,
                PortAlignment.Top => 270,
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
