using System.Configuration;

namespace System
{
    public static class AppSettings
    {
        public static T Get<T>(string key) where T : struct
        {
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }

        public static bool IsOffline { get { return Get<bool>("offline"); } }
    }
}
