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
    public class GridLengthConverter : IValueConverter
    {
        // converts a number to a star notation grid length
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => new GridLength(System.Convert.ToDouble(value), GridUnitType.Star);

        // so far, only one-way binding scenarios require this converter
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
