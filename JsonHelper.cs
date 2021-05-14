using System.Text.Encodings.Web;
using System.Text.Json;

namespace DotNetTool
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options;
        private static readonly JsonSerializerOptions _options_without_indent;

        static JsonHelper()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                PropertyNamingPolicy = null
            };

            _options_without_indent = new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                PropertyNamingPolicy = null
            };
        }

        public static string Object2JsonStr(object model)
        {
            return JsonSerializer.Serialize(model, _options);
        }

        public static string Object2JsonStrWithoutIndent(object model)
        {
            return JsonSerializer.Serialize(model, _options_without_indent);
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
