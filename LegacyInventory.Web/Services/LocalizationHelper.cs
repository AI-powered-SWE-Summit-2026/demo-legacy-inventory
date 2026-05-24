using System.Globalization;
using System.Reflection;
using System.Resources;

namespace LegacyInventory.Web.Services
{
    public class LocalizationHelper
    {
        private static ResourceManager _rm = new ResourceManager(
            "LegacyInventory.Web.Resources.SharedResources",
            Assembly.GetExecutingAssembly());

        public static string Get(string key, string culture = "en-US")
        {
            var ci = new CultureInfo(culture);
            return _rm.GetString(key, ci) ?? key;
        }

        public string GetText(string key, string culture = "en-US")
        {
            return Get(key, culture);
        }
    }
}
