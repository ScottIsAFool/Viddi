using Windows.System.Display;

namespace Viddi.Services
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

        public DisplayRequestService()
        {
            _displayRequest = new DisplayRequest();
        }

        public bool IsActive { get; private set; }

        public void Request()
        {
            if (_displayRequest == null)
            {
                _displayRequest = new DisplayRequest();
            }

            _displayRequest.RequestActive();
            IsActive = true;
        }

        public void Release()
        {
            if (_displayRequest == null)
            {
                return;
            }

            _displayRequest.RequestRelease();
            _displayRequest = null;
            IsActive = false;
        }
    }
}
