using GalaSoft.MvvmLight.Command;

namespace Viddy.ViewModel
{
    public interface ICanHasHomeButton
    {
        bool ShowHomeButton { get; set; }
        RelayCommand NavigateHomeCommand { get; }
    }
}