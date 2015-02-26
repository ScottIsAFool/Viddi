using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinRTXamlToolkit.Controls.Extensions;

namespace Viddy.Controls
{
    public class LoadingListView : ListView
    {
        private ScrollViewer _scrollViewer;
        private ContentPresenter _goToTopButton;
        private ContentControl _emptyContent;
        private ContentControl _initialisingContent;
        private ContentControl _loadFailedContent;
        private ItemsPresenter _itemsPresenter;

        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(
            "IsEmpty", typeof(bool), typeof(LoadingListView), new PropertyMetadata(default(bool), StateChanged));

        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            set { SetValue(IsEmptyProperty, value); }
        }

        public static readonly DependencyProperty EmptyContentProperty = DependencyProperty.Register(
            "EmptyContent", typeof(DataTemplate), typeof(LoadingListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate EmptyContent
        {
            get { return (DataTemplate)GetValue(EmptyContentProperty); }
            set { SetValue(EmptyContentProperty, value); }
        }

        public static readonly DependencyProperty InitialisingContentProperty = DependencyProperty.Register(
            "InitialisingContent", typeof(DataTemplate), typeof(LoadingListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate InitialisingContent
        {
            get { return (DataTemplate)GetValue(InitialisingContentProperty); }
            set { SetValue(InitialisingContentProperty, value); }
        }

        public static readonly DependencyProperty IsInitialisingProperty = DependencyProperty.Register(
            "IsInitialising", typeof(bool), typeof(LoadingListView), new PropertyMetadata(default(bool), StateChanged));

        public bool IsInitialising
        {
            get { return (bool)GetValue(IsInitialisingProperty); }
            set { SetValue(IsInitialisingProperty, value); }
        }

        public static readonly DependencyProperty EmptyContentStyleProperty = DependencyProperty.Register(
            "EmptyContentStyle", typeof(Style), typeof(LoadingListView), new PropertyMetadata(default(Style)));

        public Style EmptyContentStyle
        {
            get { return (Style)GetValue(EmptyContentStyleProperty); }
            set { SetValue(EmptyContentStyleProperty, value); }
        }

        public static readonly DependencyProperty InitialisingContentStyleProperty = DependencyProperty.Register(
            "InitialisingContentStyle", typeof(Style), typeof(LoadingListView), new PropertyMetadata(default(Style)));

        public Style InitialisingContentStyle
        {
            get { return (Style)GetValue(InitialisingContentStyleProperty); }
            set { SetValue(InitialisingContentStyleProperty, value); }
        }

        public static readonly DependencyProperty LoadFailedContentProperty = DependencyProperty.Register(
            "LoadFailedContent", typeof(DataTemplate), typeof(LoadingListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate LoadFailedContent
        {
            get { return (DataTemplate)GetValue(LoadFailedContentProperty); }
            set { SetValue(LoadFailedContentProperty, value); }
        }

        public static readonly DependencyProperty LoadFailedProperty = DependencyProperty.Register(
            "LoadFailed", typeof(bool), typeof(LoadingListView), new PropertyMetadata(default(bool), StateChanged));

        public bool LoadFailed
        {
            get { return (bool)GetValue(LoadFailedProperty); }
            set { SetValue(LoadFailedProperty, value); }
        }

        public static readonly DependencyProperty IsAtTopProperty = DependencyProperty.Register(
            "IsAtTop", typeof(bool), typeof(LoadingListView), new PropertyMetadata(default(bool)));

        public bool IsAtTop
        {
            get { return (bool)GetValue(IsAtTopProperty); }
            set { SetValue(IsAtTopProperty, value); }
        }

        public static readonly DependencyProperty GoToTopButtonProperty = DependencyProperty.Register(
            "GoToTopButton", typeof(object), typeof(LoadingListView), new PropertyMetadata(default(object)));

        public object GoToTopButton
        {
            get { return (object)GetValue(GoToTopButtonProperty); }
            set { SetValue(GoToTopButtonProperty, value); }
        }

        public static readonly DependencyProperty GoToTopButtonTemplateProperty = DependencyProperty.Register(
            "GoToTopButtonTemplate", typeof(DataTemplate), typeof(LoadingListView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate GoToTopButtonTemplate
        {
            get { return (DataTemplate)GetValue(GoToTopButtonTemplateProperty); }
            set { SetValue(GoToTopButtonTemplateProperty, value); }
        }

        public static readonly DependencyProperty ShowGoToTopButtonProperty = DependencyProperty.Register(
            "ShowGoToTopButton", typeof(bool), typeof(LoadingListView), new PropertyMetadata(true));

        public bool ShowGoToTopButton
        {
            get { return (bool)GetValue(ShowGoToTopButtonProperty); }
            set { SetValue(ShowGoToTopButtonProperty, value); }
        }

        public LoadingListView()
        {
            DefaultStyleKey = typeof(LoadingListView);
        }

        public void GoToTop()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToVerticalOffsetWithAnimation(0);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (_scrollViewer != null)
            {
                _scrollViewer.ViewChanged += ScrollViewerOnViewChanged;
                _scrollViewer.ViewChanging += ScrollViewerOnViewChanging;
                _scrollViewer.IsScrollInertiaEnabled = true;
            }

            _goToTopButton = GetTemplateChild("GoToTopButtonPresenter") as ContentPresenter;
            if (_goToTopButton != null)
            {
                _goToTopButton.Tapped += GoToTopButtonOnTapped;
            }

            _emptyContent = GetTemplateChild("EmptyContentControl") as ContentControl;
            _initialisingContent = GetTemplateChild("InitialisingContentControl") as ContentControl;
            _loadFailedContent = GetTemplateChild("LoadFailedContentControl") as ContentControl;
            _itemsPresenter = GetTemplateChild("ItemsPresenter") as ItemsPresenter;

            ShowParts();
        }

        private static void StateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var listview = sender as LoadingListView;
            if (listview != null)
            {
                listview.ShowParts();
            }
        }

        private void ShowParts()
        {
            if (_itemsPresenter != null)
            {
                if ((LoadFailed || IsEmpty || IsInitialising))
                {
                    _itemsPresenter.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _itemsPresenter.Visibility = Visibility.Visible;
                }
            }

            if (_loadFailedContent != null)
            {
                if (LoadFailed)
                {

                    _loadFailedContent.Visibility = Visibility.Visible;
                    return;
                }
                
                _loadFailedContent.Visibility = Visibility.Collapsed;
            }

            if (_emptyContent != null)
            {
                if (IsEmpty && !IsInitialising)
                {
                    _emptyContent.Visibility = Visibility.Visible;
                }
                else
                {
                    _emptyContent.Visibility = Visibility.Collapsed;
                }
            }

            if (_initialisingContent != null)
            {
                _initialisingContent.Visibility = IsInitialising ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ScrollViewerOnViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.IsInertial)
            {
                if (!IsAtTop)
                {
                    if (ShowGoToTopButton)
                    {
                        // Show go to top button
                        VisualStateManager.GoToState(this, "ShowGoToTopButton", true);
                    }
                }
            }
        }

        private void GoToTopButtonOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            GoToTop();
        }

        private void ScrollViewerOnViewChanged(object sender, ScrollViewerViewChangedEventArgs scrollViewerViewChangedEventArgs)
        {
            if (_scrollViewer == null)
            {
                return;
            }

            IsAtTop = _scrollViewer.VerticalOffset == 0;

            if (IsAtTop)
            {
                // Hide go to top button
                VisualStateManager.GoToState(this, "HideGoToTopButton", true);
            }
        }
    }
}
