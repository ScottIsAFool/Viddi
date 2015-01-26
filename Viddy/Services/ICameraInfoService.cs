using System;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace Cimbalino.Toolkit.Services
{
    public interface ICameraInfoService
    {
        Task StartService();
        Task<bool> HasPrimaryCamera();
        Task<bool> HasFrontFacingCamera();
        Task<bool> HasFlash();
        Task<bool> SupportsFocus();

#if !WINDOWS_PHONE
        MediaCapture MediaCapture { get; }
        bool IsInitialised { get; }
        Task DisposeMediaCapture();
        Task StartPreview(Action preStart = null);
        Task StopPreview();
        event EventHandler IsInitialisedChanged;
#endif
    }
}