using System;
using Cimbalino.Toolkit.Services;

namespace Viddy
{
    public static class Utils
    {
        public static string GetDeviceId(IApplicationSettingsService handler)
        {
            if (handler.Roaming.Contains(Constants.StorageSettings.WindowsPhoneDeviceIdSetting))
            {
                return handler.Roaming.Get<string>(Constants.StorageSettings.WindowsPhoneDeviceIdSetting);
            }

            var guid = Guid.NewGuid().ToString();
            handler.Roaming.Set(Constants.StorageSettings.WindowsPhoneDeviceIdSetting, guid);
            return guid;
        }
    }
}
