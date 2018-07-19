using Newtonsoft.Json;

namespace Epicoin.Library.Tools
{
    public static class Serialyze
    {
        public static string Serialize(object objet)
        {
            return JsonConvert.SerializeObject(objet);
        }

        public static T Unserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}