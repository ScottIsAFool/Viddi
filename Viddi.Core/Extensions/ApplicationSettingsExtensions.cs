using Cimbalino.Toolkit.Services;
using Newtonsoft.Json;

namespace Viddi.Core.Extensions
{
    public static class ApplicationSettingsExtensions
    {
        public static void SetS<T>(this IApplicationSettingsServiceHandler handler, string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            handler.Set(key, json);
        }

        public static T GetS<T>(this IApplicationSettingsServiceHandler handler, string key)
        {
            if (!handler.Contains(key))
            {
                return default(T);
            }

            var json = handler.Get<string>(key);
            var item = JsonConvert.DeserializeObject<T>(json);
            return item;
        }

        public static T GetS<T>(this IApplicationSettingsServiceHandler handler, string key, T defaultValue)
        {
            return handler.Contains(key) ? handler.GetS<T>(key) : defaultValue;
        }
    }
}
