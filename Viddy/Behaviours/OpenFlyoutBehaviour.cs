using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Cimbalino.Toolkit.Behaviors;

namespace Viddi.Behaviours
{
    public class OpenFlyoutBehaviour : Behavior<FrameworkElement>
    {
        private FlyoutBase _flyout;

        public static readonly DependencyProperty FlyoutIsOpenProperty = DependencyProperty.Register(
            "FlyoutIsOpen", typeof (bool), typeof (OpenFlyoutBehaviour), new PropertyMetadata(default(bool), FlyoutIsOpenChanged));

        public bool FlyoutIsOpen
        {
            get { return (bool) GetValue(FlyoutIsOpenProperty); }
            set { SetValue(FlyoutIsOpenProperty, value); }
        }

        private static void FlyoutIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var b = sender as OpenFlyoutBehaviour;
            if (b != null)
            {
                b.ShowHide();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _flyout = FlyoutBase.GetAttachedFlyout(AssociatedObject);
            if (_flyout != null)
            {
                _flyout.Closed += FlyoutOnClosed;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (_flyout != null)
            {
                _flyout.Closed -= FlyoutOnClosed;
            }
        }

        private void FlyoutOnClosed(object sender, object o)
        {
            FlyoutIsOpen = false;
        }

        private void ShowHide()
        {
            if (FlyoutIsOpen)
            {
                FlyoutBase.ShowAttachedFlyout(AssociatedObject);
            }
            else
            {
                if (_flyout != null)
                {
                    _flyout.Hide();
                }
            }
        }
    }
}
