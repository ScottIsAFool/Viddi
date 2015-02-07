using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Ioc;
using Viddy.Services;
using Viddy.ViewModel;

namespace Viddy.Views
{
    public class PinnableBasePage : BasePage
    {
        protected async Task SaveTileImage(UIElement tileElement)
        {
            var vm = DataContext as ViewModelBase;
            if (vm != null)
            {
                var tileService = SimpleIoc.Default.GetInstance<ITileService>();
                var filename = vm.GetPinFileName();
                await tileService.SaveVisualElementToFile(tileElement, filename, 360, 360);
                vm.PinUnpin();
            }
        }
    }
}