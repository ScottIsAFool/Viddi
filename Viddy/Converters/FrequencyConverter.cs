using System;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Ioc;
using Viddy.Core.Services;
using Viddy.Model;

namespace Viddy.Converters
{
    public class FrequencyConverter : IValueConverter
    {
        private static ILocalisationLoader _loader;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (_loader == null) _loader = SimpleIoc.Default.GetInstance<ILocalisationLoader>();

            var frequency = (UpdateFrequency)value;
            return _loader.GetString(frequency.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
