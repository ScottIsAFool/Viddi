using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using ScottIsAFool.WindowsPhone.Logging;
using Viddy.Messaging;

namespace Viddy.ViewModel
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public ILog Log { get; set; }

        protected ViewModelBase()
        {
            if (!IsInDesignMode)
            {
                WireMessages();
                Log = new WinLogger(GetType().FullName);
            }
        }

        protected virtual void WireMessages()
        {
            Messenger.Default.Register<PinMessage>(this, m => RaisePropertyChanged(() => IsPinned));
        }

        public virtual bool IsPinned
        {
            get { return false; }
        }

        public void SetProgressBar(string text)
        {
            ProgressIsVisible = true;
            ProgressText = text;

            UpdateProperties();
        }

        public void SetProgressBar()
        {
            ProgressIsVisible = false;
            ProgressText = string.Empty;

            UpdateProperties();
        }

        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }

        public virtual void UpdateProperties() { }

        public virtual async Task PinUnpin() { }

        public virtual string GetPinFileName(bool isWideTile = false)
        {
            return string.Empty;
        }
    }
}
