using Dynamo.Graph.Nodes;
using System.Globalization;
using System.Windows.Data;

namespace Dynamo.Controls
{
    public class InPortAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var alignment = (PortAlinement)value;
            return alignment switch
            {

                PortAlinement.Left => 0,
                PortAlinement.Top => 90,
                PortAlinement.Right => 180,
                PortAlinement.Bottom => 270,
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
