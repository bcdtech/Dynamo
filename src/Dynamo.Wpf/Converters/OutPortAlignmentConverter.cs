using Dynamo.Graph.Nodes;
using System.Globalization;
using System.Windows.Data;

namespace Dynamo.Controls
{
    public class OutPortAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var alignment = (PortAlinement)value;
            return alignment switch
            {
                PortAlinement.Right => 0,
                PortAlinement.Bottom => 90,
                PortAlinement.Left => 180,
                PortAlinement.Top => 270,
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
