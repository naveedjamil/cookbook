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

            if (ua.Contains("iPad"))
            {
                return "iPad";
            }

            if (ua.Contains("iPhone"))
            {
                return "iPhone";
            }

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
            {
                return "Linux";
            }

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
            {
                return "BlackBerry";
            }

            if (ua.Contains("Windows Phone"))
            {
                return "WindowsPhone";
            }

            if (ua.Contains("Mac OS"))
            {
                return "MacOS";
            }

            if (ua.Contains("Windows"))
            {
                return "Windows";
            }
            return string.Empty;
        }

        private static string GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (int.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                {
                    break;
                }
            }

            return version;
        }
    }
}
