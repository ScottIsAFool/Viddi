using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viddy.Controls
{
    public class InitialisingListView : ListView
    {
        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(
            "IsEmpty", typeof (bool), typeof (InitialisingListView), new PropertyMetadata(default(bool)));

        public bool IsEmpty
        {
            get { return (bool) GetValue(IsEmptyProperty); }
            set { SetValue(IsEmptyProperty, value); }
        }

        public static readonly DependencyProperty EmptyContentProperty = DependencyProperty.Register(
            "EmptyContent", typeof (DataTemplate), typeof (InitialisingListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate EmptyContent
        {
            get { return (DataTemplate) GetValue(EmptyContentProperty); }
            set { SetValue(EmptyContentProperty, value); }
        }

        public static readonly DependencyProperty InitialisingContentProperty = DependencyProperty.Register(
            "InitialisingContent", typeof (DataTemplate), typeof (InitialisingListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate InitialisingContent
        {
            get { return (DataTemplate) GetValue(InitialisingContentProperty); }
            set { SetValue(InitialisingContentProperty, value); }
        }

        public static readonly DependencyProperty IsInitialisingProperty = DependencyProperty.Register(
            "IsInitialising", typeof (bool), typeof (InitialisingListView), new PropertyMetadata(default(bool)));

        public bool IsInitialising
        {
            get { return (bool) GetValue(IsInitialisingProperty); }
            set { SetValue(IsInitialisingProperty, value); }
        }

        public static readonly DependencyProperty EmptyContentStyleProperty = DependencyProperty.Register(
            "EmptyContentStyle", typeof (Style), typeof (InitialisingListView), new PropertyMetadata(default(Style)));

        public Style EmptyContentStyle
        {
            get { return (Style) GetValue(EmptyContentStyleProperty); }
            set { SetValue(EmptyContentStyleProperty, value); }
        }

        public static readonly DependencyProperty InitialisingContentStyleProperty = DependencyProperty.Register(
            "InitialisingContentStyle", typeof (Style), typeof (InitialisingListView), new PropertyMetadata(default(Style)));

        public Style InitialisingContentStyle
        {
            get { return (Style) GetValue(InitialisingContentStyleProperty); }
            set { SetValue(InitialisingContentStyleProperty, value); }
        }

        public static readonly DependencyProperty ExtraContentProperty = DependencyProperty.Register(
            "ExtraContent", typeof(DataTemplate), typeof(InitialisingListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ExtraContent
        {
            get { return (DataTemplate)GetValue(ExtraContentProperty); }
            set { SetValue(ExtraContentProperty, value); }
        }

        public static readonly DependencyProperty DisplayExtraContentProperty = DependencyProperty.Register(
            "DisplayExtraContent", typeof (bool), typeof (InitialisingListView), new PropertyMetadata(default(bool)));

        public bool DisplayExtraContent
        {
            get { return (bool) GetValue(DisplayExtraContentProperty); }
            set { SetValue(DisplayExtraContentProperty, value); }
        }

        public static readonly DependencyProperty DisplayExtraContentFrequencyProperty = DependencyProperty.Register(
            "DisplayExtraContentFrequency", typeof (int), typeof (InitialisingListView), new PropertyMetadata(default(int)));

        public int DisplayExtraContentFrequency
        {
            get { return (int) GetValue(DisplayExtraContentFrequencyProperty); }
            set { SetValue(DisplayExtraContentFrequencyProperty, value); }
        }

        public InitialisingListView()
        {
            DefaultStyleKey = typeof (InitialisingListView);
        }
    }
}
