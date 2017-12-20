using System.Web;

namespace Recipe.Core.Helper
{
    public static class PlatformHelper
    {
        public static string GetUserPlatform(HttpRequest request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
            {
                return "Android";
            }
            else if (ua.Contains("iPad"))
            {
                return "iPad";
            }
            else if (ua.Contains("iPhone"))
            {
                return "iPhone";
            }
            else if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
            {
                return "Linux";
            }
            else if (ua.Contains("RIM Tablet") || (ua.Contains("BB")))
            {
                return "BlackBerry";
            }
            else if (ua.Contains("Windows Phone"))
            {
                return "WindowsPhone";
            }
            else if (ua.Contains("Mac OS"))
            {
                return "MacOS";
            }
            else if (ua.Contains("Windows"))
            {
                return "Windows";
            }

            return string.Empty;
        }
    }
}
