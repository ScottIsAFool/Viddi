using System;
using Windows.UI.Xaml.Data;

namespace Viddi.Converters
{
    public class NullBooleanConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Invert)
            {
                return value == null;
            }

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}