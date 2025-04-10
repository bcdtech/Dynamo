using System.Collections.Generic;

namespace CoreNodes.ChartHelpers
{
    public class BasicLineChartFunctions
    {
        private static List<string> defaultLabels = new List<string> { "Series 1", "Series 2", "Series 3" };

        private static List<List<double>> defaultNestedValues = new List<List<double>> {
            new List<double> { 4, 6, 5, 2, 4 },
            new List<double> { 6, 7, 3, 4, 6 },
            new List<double> { 4, 2, 7, 2, 7 } };

        private BasicLineChartFunctions() { }


    }
}
