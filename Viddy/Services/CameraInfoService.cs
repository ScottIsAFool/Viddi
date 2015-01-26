using System.Linq;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using System.Collections.Generic;
using Windows.Phone.Media.Capture;
using Microsoft.Devices;
#else
using System;
using Windows.Media.Capture;
using Windows.Devices.Enumeration;
#endif

namespace Cimbalino.Toolkit.Services
{
    public class CameraInfoService : ICameraInfoService
    {
        public enum CameraType
        {
            FrontFacing,
            Primary
        };
#if WINDOWS_PHONE
        public IReadOnlyList<FlashState> GetAvailableFlashStates()
        {
            IReadOnlyList<object> rawValueList = PhotoCaptureDevice.GetSupportedPropertyValues(CameraSensorLocation.Back, KnownCameraPhotoProperties.FlashMode);
            List<FlashState> flashStates = new List<FlashState>(rawValueList.Count);
            foreach (object rawValue in rawValueList) flashStates.Add((FlashState) (uint) rawValue);
            return flashStates.AsReadOnly();
        }
#else
        public CameraInfoService()
        {
            _captureManager = new MediaCapture();
        }
        
        private static DeviceInformationCollection _deviceCollection;
        private static MediaCapture _captureManager;

        private async Task<bool> HasCameraType(CameraType cameraType)
        {
            var device = await GetDevice(cameraType);
            return device != null;
        }

        public async Task<DeviceInformation> GetDevice(CameraType cameraType)
        {
            if (_deviceCollection == null)
            {
                _deviceCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            }

            var panel = cameraType == CameraType.FrontFacing ? Panel.Front : Panel.Back;

            var device = _deviceCollection.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == panel);

            return device;
        }

        public MediaCapture MediaCapture
        {
            get { return _captureManager; }
        }

        public bool IsInitialised { get; private set; }

        private bool _previewStarted;
        public async Task StartPreview(Task preStart = null)
        {
            if (_captureManager == null)
            {
                _captureManager = new MediaCapture();
            }

            if (IsInitialised)
            {
                if (preStart != null)
                {
                    await preStart;
                }

                await _captureManager.StartPreviewAsync();
                _previewStarted = true;
            }
        }

        public async Task StopPreview()
        {
            if (_captureManager == null)
            {
                return;
            }

            if (_previewStarted)
            {
                await _captureManager.StopPreviewAsync();
                _previewStarted = false;
            }
        }

        public event EventHandler IsInitialisedChanged;

        public async Task DisposeMediaCapture()
        {
            if (_captureManager == null)
            {
                return;
            }

            await StopPreview();

            _captureManager.Dispose();
            _captureManager = null;
            IsInitialised = false;
            InitialisedChanged();
        }

        private void InitialisedChanged()
        {
            var changed = IsInitialisedChanged;
            if (changed != null)
            {
                changed(this, EventArgs.Empty);
            }
        }

        public async Task StartService(CameraType cameraType = CameraType.Primary)
        {
            var device = await GetDevice(cameraType);
            if (device == null)
            {
                await StartService(null);
            }
            else
            {
                await StartService(new MediaCaptureInitializationSettings {VideoDeviceId = device.Id});
            }
        }
#endif
       
        public async Task StartService(MediaCaptureInitializationSettings settings)
        {
#if !WINDOWS_PHONE
            try
            {
                if (_captureManager == null)
                {
                    _captureManager = new MediaCapture();
                }

                if (!IsInitialised)
                {
                    IsInitialised = true;
                    if (settings == null)
                    {
                        var device = await GetDevice(CameraType.Primary);
                        if (device != null)
                        {
                            await _captureManager.InitializeAsync(new MediaCaptureInitializationSettings {VideoDeviceId = device.Id});
                        }
                        else
                        {
                            await _captureManager.InitializeAsync();
                        }
                    }
                    else
                    {
                        await _captureManager.InitializeAsync(settings);
                    }
                    InitialisedChanged();
                }
            }
            catch { }
#endif
        }

        public async Task<bool> HasPrimaryCamera()
        {
#if WINDOWS_PHONE
                return PhotoCamera.IsCameraTypeSupported(CameraType.Primary);
#else
            return await HasCameraType(CameraType.Primary);
#endif
        }

        public async Task<bool> HasFrontFacingCamera()
        {
#if WINDOWS_PHONE
                return PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing);
#else
            return await HasCameraType(CameraType.FrontFacing);
#endif
        }

        public async Task<bool> HasFlash()
        {
#if WINDOWS_PHONE
                return GetAvailableFlashStates().Any(r => r != FlashState.Off);
#else
            return _captureManager.VideoDeviceController != null
                   && _captureManager.VideoDeviceController.FlashControl != null
                   && _captureManager.VideoDeviceController.FlashControl.Supported
                   && _captureManager.VideoDeviceController.FlashControl.PowerSupported;
#endif
        }

        public async Task<bool> SupportsFocus()
        {
#if WINDOWS_PHONE
            return PhotoCaptureDevice.IsFocusSupported(CameraSensorLocation.Back);
#else
            return _captureManager.VideoDeviceController != null
                   && _captureManager.VideoDeviceController.FocusControl != null
                   && _captureManager.VideoDeviceController.FocusControl.Supported;
#endif
        }
    }
}