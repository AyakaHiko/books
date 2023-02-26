using System.Text.Json;
using System.Text.Json.Serialization;

namespace books.Extensions
{
    public static class SessionsExtension
    {
        public static T GetOrAdd<T>(
            this ISession session,
            string key,
            Func<T> creator) where T : class
        {
            var data = session.Get<T>(key);
            if (data == null)
            {
                data = creator();
                Set(session, key, data);
            }
            return data;
        }
        public static void Set<T>(
            this ISession session,
            string key,
            T value)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };
            session.SetString(key, JsonSerializer.Serialize<T>(value, options));
        }
        public static T? Get<T>(this ISession session, string key)
        {
            string? value = session.GetString(key);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve, 
            };
            return value == null ?
                default(T) :
                JsonSerializer.Deserialize<T>(value, options);
        }
        public static void Remove<T>(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
}
