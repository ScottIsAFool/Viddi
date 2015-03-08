using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Coding4Fun.Toolkit.Controls;

namespace Viddi.Services
{
    public class ToastService : IToastService
    {
        private bool _toastOpen;
        public void Show(string message, string title = "", Action tapAction = null)
        {
            if (_toastOpen)
            {
                return;
            }

            var prompt = new ToastPrompt
            {
                Title = title,
                Message = message,
                Foreground = new SolidColorBrush(Colors.White)
            };

            if (tapAction != null)
            {
                prompt.Tapped += (sender, args) => tapAction();
            }

            prompt.Completed += (sender, args) => _toastOpen = false;

            _toastOpen = true;
            prompt.Show();
        }
    }
}