using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Ioc;
using Viddi.Services;
using Viddi.ViewModel;

namespace Viddi.Views
{
    public class PinnableBasePage : BasePage
    {
        protected async Task SaveTileImage(UIElement mediumTile, UIElement wideTile = null)
        {
            var vm = DataContext as ViewModelBase;
            if (vm != null)
            {
                var tileService = SimpleIoc.Default.GetInstance<ITileService>();
                var filename = vm.GetPinFileName();
                await tileService.SaveVisualElementToFile(mediumTile, filename, 360, 360);
                if (wideTile != null)
                {
                    filename = vm.GetPinFileName(true);
                    await tileService.SaveVisualElementToFile(wideTile, filename, 360, 691);
                }
                vm.PinUnpin();
            }
        }
    }
}