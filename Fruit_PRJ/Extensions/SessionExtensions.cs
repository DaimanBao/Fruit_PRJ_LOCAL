using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Fruit_PRJ.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
           => session.SetString(key, JsonSerializer.Serialize(value));

        public static T GetObject<T>(this ISession session, string key)
            => session.GetString(key) == null
                ? default
                : JsonSerializer.Deserialize<T>(session.GetString(key));
    }
}
