using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Dynamo.Controls
{
    /// <summary>
    /// Converts double values to compact strings with optional scientific notation and superscripts.
    /// </summary>
    public class NumberFormatterConverter : IValueConverter
    {
        private static readonly string SuperscriptDigits = "?123??????";
        private static readonly char SuperscriptMinus = '?';
        private const int MaxLength = 8;

        /// <summary>
        /// Formats a numeric value into a short string (max 8 characters) with rounding and superscripts if needed.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double d)
                return value?.ToString() ?? "";

            double abs = Math.Abs(d);

            // Scientific Notation for Very Large or Small Numbers
            if ((abs >= 1_000_000 || (abs < 0.0001 && d != 0)))
                return FormatScientific(d);

            // Whole Numbers
            if (d % 1 == 0)
            {
                string whole = d.ToString("F0", CultureInfo.InvariantCulture);
                return whole.Length <= MaxLength ? whole : $"~{whole.Substring(0, MaxLength - 1)}";
            }

            // Decimals with Dynamic Precision
            string formatted = FormatDecimalWithPrecision(d, abs);
            if (formatted.Length <= MaxLength)
                return formatted;

            // Fallback: Truncate General Format
            string fallback = d.ToString("G", CultureInfo.InvariantCulture);
            return fallback.Length <= MaxLength
                ? fallback
                : $"~{fallback.Substring(0, MaxLength - 1)}";
        }

        private string FormatScientific(double d)
        {
            string sci = d.ToString("0.#####E+0", CultureInfo.InvariantCulture);
            var match = Regex.Match(sci, @"([\d\.]+)E([+-]?\d+)");
            if (!match.Success)
                return d.ToString("E2", CultureInfo.InvariantCulture);

            string basePart = match.Groups[1].Value;
            string exponent = match.Groups[2].Value;
            string superExp = ToSuperscript(exponent);
            string formatted = $"{basePart}×10{superExp}";

            if (formatted.Length <= MaxLength)
                return formatted;

            while (formatted.Length > MaxLength && basePart.Length > 1)
            {
                basePart = basePart[..^1];
                formatted = $"{basePart}×10{superExp}";
            }

            return $"~{formatted}";
        }

        private string FormatDecimalWithPrecision(double d, double abs)
        {
            string wholePart = Math.Floor(abs).ToString(CultureInfo.InvariantCulture);
            int allowedDecimals = MaxLength - wholePart.Length - 1;

            if (allowedDecimals < 1)
                return d.ToString("F0", CultureInfo.InvariantCulture);

            string full = d.ToString("G", CultureInfo.InvariantCulture);

            string formatted = d.ToString($"F{allowedDecimals}", CultureInfo.InvariantCulture)
                                .TrimEnd('0').TrimEnd('.');

            bool wasTrimmed = formatted.Length < full.Length;

            if (formatted.Length <= MaxLength)
                return wasTrimmed ? $"~{formatted}" : formatted;

            return formatted;
        }

        private string ToSuperscript(string digits)
        {
            var sb = new StringBuilder();
            foreach (char c in digits)
            {
                if (c == '-') sb.Append(SuperscriptMinus);
                else if (c == '+') continue;
                else if (char.IsDigit(c)) sb.Append(SuperscriptDigits[c - '0']);
                else sb.Append(c);
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
