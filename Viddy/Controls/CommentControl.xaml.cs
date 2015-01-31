using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Viddy.ViewModel;

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
