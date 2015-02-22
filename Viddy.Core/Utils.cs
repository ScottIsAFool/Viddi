﻿using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace Viddy.Core
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
                return Guid.NewGuid().ToString();
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
            const int WEEK = 7 * DAY;
            const int MONTH = 30 * DAY;

            DateTime theDate = value;
            int offset = TimeZoneInfo.Local.BaseUtcOffset.Hours;
            theDate = theDate.AddHours(offset);

            var ts = DateTime.Now.Subtract(theDate);
            double seconds = ts.TotalSeconds;

            // Less than one minute
            if (seconds < 1 * MINUTE)
                return ts.Seconds == 1 ? "1s ago" : string.Format("{0}s ago", ts.Seconds);

            if (seconds < 60 * MINUTE)
                return ts.Minutes == 1 ? "1m ago" : string.Format("{0}m ago", ts.Minutes);

            if (seconds < 120 * MINUTE)
                return "1h ago";

            if (seconds < 24 * HOUR)
                return string.Format("{0}h ago", ts.Hours);

            if (seconds < 48 * HOUR)
                return "1d ago";

            if (seconds < 7 * DAY)
                return string.Format("{0}d ago", ts.Days);

            if (seconds > 1 * WEEK)
            {
                var weeks = Convert.ToInt32(Math.Floor((double)ts.Days / 7));
                return weeks <= 1 ? "1w ago" : string.Format("{0}w ago", weeks);
            }

            return string.Empty;
        }
    }
}
