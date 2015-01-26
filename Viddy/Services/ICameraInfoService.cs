using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;

namespace Cimbalino.Toolkit.Services
{
    public interface ICameraInfoService
    {
        Task StartService(MediaCaptureInitializationSettings settings);
        Task StartService(CameraInfoService.CameraType cameraType = CameraInfoService.CameraType.Primary);
        Task<bool> HasPrimaryCamera();
        Task<bool> HasFrontFacingCamera();
        Task<bool> HasFlash();
        Task<bool> SupportsFocus();

#if !WINDOWS_PHONE
        Task<DeviceInformation> GetDevice(CameraInfoService.CameraType cameraType);
        MediaCapture MediaCapture { get; }
        bool IsInitialised { get; }
        Task DisposeMediaCapture();
        Task StartPreview(Action preStart = null);
        Task StopPreview();
        event EventHandler IsInitialisedChanged;
#endif
    }
}