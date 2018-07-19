using System.Text;
using Newtonsoft.Json;

namespace Epicoin.Library.Net
{
    internal static class Formatter
    {
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            string json = JsonConvert.SerializeObject(obj);
            byte[] bytes = Encoding.Default.GetBytes(json);
            return bytes;
        }

        public static T ToObject<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(T);
            }

            string json = Encoding.Default.GetString(bytes);
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
    }
}