using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Viddy.ViewModel;
using Viddy.ViewModel.Item;

namespace Viddy.Controls
{
    public sealed partial class CommentControl
    {
        public CommentControl()
        {
            InitializeComponent();
        }

        private void ContainerGrid_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            var vm = DataContext as CommentViewModel;
            if (vm == null || !vm.CanDelete)
            {
                return;
            }

            var grid = sender as Grid;
            FlyoutBase.ShowAttachedFlyout(grid);
        }
    }
}
