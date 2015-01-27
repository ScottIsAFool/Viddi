using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Viddy.ViewModel;

namespace Viddy.Controls
{
    public class VideoItemTypeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate AdTemplate { get; set; }
        public DataTemplate ReviewTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var listType = item as IListType;
            if (listType == null)
            {
                return base.SelectTemplateCore(item);
            }

            switch (listType.ListType)
            {
                case ListType.Normal:
                    return NormalTemplate;
                case ListType.Ad:
                    return AdTemplate;
                case ListType.Review:
                    return ReviewTemplate;
                default:
                    return base.SelectTemplateCore(item);
            }
        }
    }
}
