using System;
using Windows.UI.Xaml.Data;

namespace Viddy.Converters
{
    public class VoteConverter : IValueConverter
    {
        public bool IsDownVote { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var vote = (int) value;
            if (IsDownVote)
            {
                return vote == -1;
            }
            
            return vote == 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
