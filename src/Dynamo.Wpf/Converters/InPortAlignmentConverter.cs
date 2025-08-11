using Dynamo.Graph.Nodes;
using System.Globalization;
using System.Windows.Data;

namespace Dynamo.Controls
{
    public class InPortAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var alignment = (PortAlignment)value;
            return alignment switch
            {

                PortAlignment.Left => 0,
                PortAlignment.Top => 90,
                PortAlignment.Right => 180,
                PortAlignment.Bottom => 270,
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
