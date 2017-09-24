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
    /// Maps a range of possible source values to valid target values.
    /// </summary>
    public class SwitchConverter : IValueConverter
    {
        // maps a specific source value to the target value
        // using a mapping represented by the parameter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // so far, parameter is always assumed to be a string of case-value pairs
            var cases = parameter.ToString().Split(';').ToDictionary(
                s => s.Remove(s.IndexOf(':')).Trim(),
                s => s.Substring(s.IndexOf(':') + 1).Trim()
                );

            var key = value.ToString();
            object result;

            if (cases.ContainsKey(key))
                result = cases[key];
            else if (cases.ContainsKey(""))
                result = cases[""];
            else
                return DependencyProperty.UnsetValue;

            // returning the object depending on the type
            if (targetType == typeof(string))
                return result.ToString();
            else if (targetType == typeof(bool))
                return bool.Parse(result.ToString());
            else if (targetType == typeof(object))
            {
                return result.ToString();
            }
            else
                throw new NotSupportedException($"The objects of type {targetType.FullName} are not supported.");
        }

        // so far, only one-way binding scenarios require this converter
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
