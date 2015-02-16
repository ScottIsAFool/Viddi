namespace Viddy.Core
{
    public static class Constants
    {
        public const string ClientId = "c2073268824b46cead9262f183d2433f";
        public const string ClientSecret = "9d965c6a61840e8b557245ba79c6f4a2ceb4c5c3b267aaffd967cc6be7584198";
        public const string CallBackUrl = "http://ferretlabs.com/viddy";

        public const string FourSquareClientId = "XVBPN1RI30UYZJ21WG1OQ0PXXOCOQDYJB00ER0Z35BQOZPEL";
        public const string FourSquareClientSecret = "J1IUOTC0M0DCNVBRU4JNPI12DKD5UDYWXNREYLTN3YZTIYXT";
        public const string FourSquareSearchUrl = "https://api.foursquare.com/v2/venues/search?client_id={0}&client_secret={1}&v=20150212&ll={2},{3}&limit=10";
        public const string FourSquareGeneralAreaUrl = FourSquareSearchUrl + "&intent=checkin&radius=1000&categoryId=4f2a25ac4b909258e854f55f";
        public const string ViddyNotificationAgent = "Viddy.NotificationBackgroundTask";
        public const string ViddyNotificationAgentStartPoint = "Viddy.BackgroundTask.NotificationTask";

        public static class StorageSettings
        {
            public const string AuthenticationSettings = "AuthenticationSettings";
            public const string WindowsPhoneDeviceIdSetting = "WindowsPhoneDeviceIdSetting";
            public const string LaunchedCountSetting = "LaunchedCountSetting";
            public const string PhoneAlreadyRespondedSetting = "PhoneAlreadyRespondedSetting";
            public const string ApplicationSettings = "ApplicationSettings";
            public const string PreviousToasts = "PreviousToasts";
        }

        public static class Messages
        {
            public const string AuthCodeMsg = "AuthCodeMsg";
            public const string ProfileFileMsg = "ProfileFileMsg";
            public const string VideoFileMsg = "VideoFileMsg";
            public const string AppLaunchedMsg = "AppLaunchedMsg";
            public const string HideReviewsMsg = "HideReviewsMsg";
            public const string ClearSearchMsg = "ClearSearchMsg";
            public const string UserDetailMsg = "UserDetailMsg";
            public const string NewAppAddedMsg = "NewAppAddedMsg";
            public const string EditVideoMsg = "EditVideoMsg";
        }
    }
}
