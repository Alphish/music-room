using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Alphicsh.MusicRoom.Converters
{
    /// <summary>
    /// Handles conversion between strings and non-negative numeric values with defaults.
    /// The default value is represented with any negative number.
    /// </summary>
    public class NumericWithDefaultConverter : IValueConverter
    {
        // converts a numeric value to its string representation
        // the parameter is the string used when value to convert is negative
        // representing a general default value
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var def = parameter.ToString();

            // converting the value to appropriate strings
            if (value is int)
                return (int)value < 0 ? def : value.ToString();
            if (value is long)
                return (long)value < 0 ? def : value.ToString();

            throw new NotSupportedException("The value doesn't match any supported numeric types.");
        }

        // converts a string to a numeric value
        // if the string is not a valid number, a negative default-representing value is returned
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value.ToString();
            if (targetType == typeof(int))
            {
                int result;
                if (int.TryParse(input, out result))
                    return (result < 0) ? -1 : result;
                else
                    return -1;
            }
            if (targetType == typeof(long))
            {
                long result;
                if (long.TryParse(input, out result))
                    return (result < 0) ? -1 : result;
                else
                    return -1;
            }

            throw new NotSupportedException($"The conversion to type {targetType.FullName} is not supported.");
        }
    }
}
