using System;
using System.Collections.Generic;

namespace CoreNodes.ChartHelpers
{
    public class ScatterPlotFunctions
    {
        private static Random rnd = new Random();
        private static List<string> defaultTitles = new List<string> { "Plot 1", "Plot 2", "Plot 3" };

        private ScatterPlotFunctions() { }


        private static List<double> GenerateRandomValuesList(int count)
        {
            List<double> values = new List<double>();

            for (int i = 0; i < count; i++)
            {
                values.Add(rnd.NextDouble() * 10);
            }

            return values;
        }
    }
}
