using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Viddy.Core.Services;

namespace Viddy.Core
{
    public static class Utils
    {
        private static ILocalisationLoader _loader;
        private static EasClientDeviceInformation _info;
        
        /// <summary>
        /// Gets the unique device identifier.
        /// </summary>
        /// <value>
        /// The unique device identifier.
        /// </value>
        public static string UniqueDeviceIdentifier
        {
            get
            {
                var myToken = HardwareIdentification.GetPackageSpecificToken(null);
                var hardwareId = myToken.Id;

                var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
                var hashed = hasher.HashData(hardwareId);

                var hashedString = CryptographicBuffer.EncodeToHexString(hashed);
                return hashedString;
            }
        }


        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public static string DeviceId
        {
            get
            {
                //// wp-<manufacturer>-<devicename>
                var sb = new StringBuilder();
                sb.Append("wp-");
                sb.Append(Info.SystemManufacturer);
                sb.Append("-");
                sb.Append(Info.SystemProductName);

                var generated = sb.ToString().Replace(' ', '*');

                if ("wp--" == generated)
                {
                    generated = "wp-generic";
                }

                return generated;
            }
        }

        /// <summary>
        /// Gets the info.
        /// </summary>
        private static EasClientDeviceInformation Info
        {
            get { return _info ?? (_info = new EasClientDeviceInformation()); }
        }

        public static string GetAnonVideoKeyName(string videoId)
        {
            return string.Format("{0}-{1}", UniqueDeviceIdentifier, videoId);
        }

        public static string DaysAgo(DateTime value)
        {
            if(_loader == null) _loader = new LocalisationLoader();
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int WEEK = 7 * DAY;
            const int MONTH = 30 * DAY;

            DateTime theDate = value;
            int offset = TimeZoneInfo.Local.BaseUtcOffset.Hours;
            theDate = theDate.AddHours(offset);

            var ts = DateTime.Now.Subtract(theDate);
            double seconds = ts.TotalSeconds;

            // Less than one minute
            if (seconds < 1 * MINUTE)
                return string.Format(_loader.GetString("SecondsAgo"), ts.Seconds);

            if (seconds < 60 * MINUTE)
                return string.Format(_loader.GetString("MinutesAgo"), ts.Minutes);

            if (seconds < 120 * MINUTE)
                return string.Format(_loader.GetString("HoursAgo"), 1);

            if (seconds < 24 * HOUR)
                return string.Format(_loader.GetString("HoursAgo"), ts.Hours);

            if (seconds < 48 * HOUR)
                return string.Format(_loader.GetString("DaysAgo"), 1);

            if (seconds < 7 * DAY)
                return string.Format(_loader.GetString("DaysAgo"), ts.Days);

            if (seconds > 1 * WEEK)
            {
                var weeks = Convert.ToInt32(Math.Floor((double)ts.Days / 7));
                return string.Format(_loader.GetString("WeeksAgo"), weeks);
            }

            return string.Empty;
        }
    }
}
