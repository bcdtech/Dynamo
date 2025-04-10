using System.Collections.Generic;

namespace CoreNodes.ChartHelpers
{
    public class XYLineChartFunctions
    {
        private static List<string> defaultTitles = new List<string> { "Plot 1", "Plot 2", "Plot 3" };
        private static double[][] defaultXValues = new double[][]
        {
            new double[]{ 0, 1, 2, 3 },
            new double[]{ 0, 1, 2, 3 },
            new double[]{ 0, 1, 2, 3 }
        };

        private static double[][] defaultYValues = new double[][]
        {
            new double[]{ 0, 1, 2, 3 },
            new double[]{ 1, 2, 3, 4 },
            new double[]{ 2, 3, 4, 5 }
        };
        private XYLineChartFunctions() { }


    }
}
