using Windows.System.Display;

namespace Viddy.Services
{
    public interface IDisplayRequestService
    {
        void Request();
        void Release();
        bool IsActive { get; }
    }

    public class DisplayRequestService : IDisplayRequestService
    {
        private DisplayRequest _displayRequest;
        private bool _isActive;

        public DisplayRequestService()
        {
            _displayRequest = new DisplayRequest();
        }

        public bool IsActive { get { return _isActive; } }

        public void Request()
        {
            if (_displayRequest == null)
            {
                _displayRequest = new DisplayRequest();
            }

            _displayRequest.RequestActive();
            _isActive = true;
        }

        public void Release()
        {
            if (_displayRequest == null)
            {
                return;
            }

            _displayRequest.RequestRelease();
            _displayRequest = null;
            _isActive = false;
        }
    }
}
