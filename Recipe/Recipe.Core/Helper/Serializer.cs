using System.Net;

namespace Recipe.Core.Helper
{
    public class Serializer
    {
        public static dynamic CreateObject(HttpStatusCode code, string message, object obj)
        {
            dynamic response = new { meta = new { code = code, message = message }, data = obj };
            return response;
        }

        public static dynamic CreateObject(HttpStatusCode code, string message, object obj, string dateTime)
        {
            dynamic response = new { meta = new { code = code, message = message, date = dateTime }, data = obj };
            return response;
        }
    }
}
