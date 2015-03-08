using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Viddi.ViewModel;
using Viddi.ViewModel.Item;

namespace Viddi.Controls
{
    public sealed partial class VideoControl
    {
        public VideoControl()
        {
            InitializeComponent();
        }

        private void ContainerGrid_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            var vm = DataContext as VideoItemViewModel;
            if (vm == null || !vm.IsOwner)
            {
                return;
            }

            var grid = sender as Grid;
            FlyoutBase.ShowAttachedFlyout(grid);
        }
    }
}
