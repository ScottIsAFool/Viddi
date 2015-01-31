using System;
using System.Globalization;
using System.Linq;
using Cimbalino.Toolkit.Converters;

namespace Viddy.Converters
{
    public class CommentEnabledConverter : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(x => x == null))
            {
                return false;
            }

            return values.All(x => (bool) x);
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
