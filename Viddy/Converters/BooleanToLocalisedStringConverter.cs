using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Ioc;
using Viddy.Core.Services;

namespace Viddy.Converters
{
    public class BooleanToLocalisedStringConverter : DependencyObject, IValueConverter
    {
        private static ILocalisationLoader _loader;

        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(
            "TrueValue", typeof (string), typeof (BooleanToLocalisedStringConverter), new PropertyMetadata(default(string)));

        public string TrueValue
        {
            get { return (string) GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
            "FalseValue", typeof (string), typeof (BooleanToLocalisedStringConverter), new PropertyMetadata(default(string)));

        public string FalseValue
        {
            get { return (string) GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = (bool) value;
            return item 
                ? StringToDisplay(TrueValue) 
                : StringToDisplay(FalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static string StringToDisplay(string originalString)
        {
            if(_loader == null) _loader = SimpleIoc.Default.GetInstance<ILocalisationLoader>();

            if (string.IsNullOrEmpty(originalString)) return originalString;
            var localisedString = _loader.GetString(originalString);
            return string.IsNullOrEmpty(localisedString) ? originalString : localisedString;
        }
    }
}
