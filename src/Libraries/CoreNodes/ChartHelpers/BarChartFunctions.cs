using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;

namespace CoreNodes.ChartHelpers
{
    public class BarChartFunctions
    {
        private static List<string> defaultLabels = new List<string> { "2019", "2020", "2021" };

        private static List<double> defaultValues = new List<double> { 5.0, 6.0, 7.0, 8.0 };

        private static List<List<double>> defaultNestedValues = new List<List<double>> {
            new List<double> { 5.0, 6.0, 7.0, 8.0 },
            new List<double> { 10.0, 12.0, 14.0, 16.0 },
            new List<double> { 15.0, 18.0, 12.0, 24.0 } };

        private BarChartFunctions() { }




        [IsVisibleInDynamoLibrary(false)]
        public static Dictionary<string, double> GetDefaultNodeInput()
        {
            var output = new Dictionary<string, double>();

            for (var i = 0; i < defaultLabels.Count; i++)
            {
                output.Add(defaultLabels[i], defaultValues[i]);
            }

            return output;
        }
    }
}
