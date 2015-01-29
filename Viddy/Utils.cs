using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace Viddy
{
    public static class Utils
    {
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
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            DateTime theDate = value;
            int offset = TimeZoneInfo.Local.BaseUtcOffset.Hours;
            theDate = theDate.AddHours(offset);

            var ts = DateTime.Now.Subtract(theDate);
            double seconds = ts.TotalSeconds;

            // Less than one minute
            if (seconds < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : string.Format("{0} seconds ago", ts.Seconds);

            if (seconds < 60 * MINUTE)
                return ts.Minutes == 1 ? "one minute ago" : string.Format("{0} minutes ago", ts.Minutes);

            if (seconds < 120 * MINUTE)
                return "an hour ago";

            if (seconds < 24 * HOUR)
                return String.Format("{0} hours ago", ts.Hours);

            if (seconds < 48 * HOUR)
                return "yesterday";

            if (seconds < 30 * DAY)
                return String.Format("{0} days ago", ts.Days);

            if (seconds < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "One month ago" : string.Format("{0} months ago", months);
            }

            return string.Empty;
        }
    }
}
