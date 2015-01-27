namespace Viddy
{
    public static class Constants
    {
        public const string ClientId = "c2073268824b46cead9262f183d2433f";
        public const string ClientSecret = "9d965c6a61840e8b557245ba79c6f4a2ceb4c5c3b267aaffd967cc6be7584198";
        public const string CallBackUrl = "http://ferretlabs.com/viddy";

        public static class StorageSettings
        {
            public const string AuthenticationSettings = "AuthenticationSettings";
            public const string WindowsPhoneDeviceIdSetting = "WindowsPhoneDeviceIdSetting";
            public const string LaunchedCountSetting = "LaunchedCountSetting";
            public const string PhoneAlreadyRespondedSetting = "PhoneAlreadyRespondedSetting";
        }

        public static class Messages
        {
            public const string AuthCodeMsg = "AuthCodeMsg";
            public const string ProfileFileMsg = "ProfileFileMsg";
            public const string VideoFileMsg = "VideoFileMsg";
            public const string AppLaunchedMsg = "AppLaunchedMsg";
        }
    }
}
