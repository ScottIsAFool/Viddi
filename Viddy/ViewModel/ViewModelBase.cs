using ScottIsAFool.WindowsPhone.Logging;

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
    }
}
