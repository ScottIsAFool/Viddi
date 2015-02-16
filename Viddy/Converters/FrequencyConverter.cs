using System;
using Windows.UI.Xaml.Data;
using Viddy.Model;

namespace Viddy.Converters
{
    public class FrequencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var frequency = (UpdateFrequency)value;
            switch (frequency)
            {
                case UpdateFrequency.ThirtyMinutes:
                    return "30 minutes";
                case UpdateFrequency.OneHour:
                    return "1 hour";
                case UpdateFrequency.SixHours:
                    return "6 hours";
                case UpdateFrequency.TwelveHours:
                    return "12 hours";
                case UpdateFrequency.OneDay:
                    return "1 day";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
