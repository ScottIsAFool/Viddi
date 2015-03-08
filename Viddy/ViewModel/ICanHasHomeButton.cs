using GalaSoft.MvvmLight.Command;

namespace Viddi.ViewModel
{
    public interface ICanHasHomeButton
    {
        bool ShowHomeButton { get; set; }
        RelayCommand NavigateHomeCommand { get; }
    }
}