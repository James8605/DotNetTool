using System.Text.Encodings.Web;
using System.Text.Json;

namespace XGS.James.Tool
{
    public class JsonHelper
    {
        public static string Object2JsonStr(object model)
        {
            return JsonSerializer.Serialize(model,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                });
        }

        public static T JsonStr2Object<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static T DeepCopy<T>(object o)
        {
            var json = Object2JsonStr(o);

            return JsonStr2Object<T>(json);
        }
    }
}
