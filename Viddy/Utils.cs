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

    }
}
