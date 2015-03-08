using Windows.ApplicationModel.Resources;

namespace Viddi.Core.Services
{
    public class LocalisationLoader : ILocalisationLoader
    {
        private static ResourceLoader _loader;

        private static ResourceLoader Loader
        {
            get { return _loader ?? (_loader = ResourceLoader.GetForCurrentView("Viddi.Localisation/Resources")); }
        }

        public string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            return Loader.GetString(key);
        }
    }
}