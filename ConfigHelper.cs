using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

namespace DotNetTool
{
    public static class ConfigHelper
    {
        private static readonly object _lock = new object();
        private static IConfiguration Config { get; }

        static ConfigHelper()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Config = builder.Build();
        }

        public static IConfiguration GetInstance()
        {
            return Config;
        }

        public static string GetString(string key)
        {
            lock (_lock)
            {
                return Config[key];
            }

        }

        public static bool GetBool(string key)
        {
            lock (_lock)
            {
                return Config[key] == "1";
            }

        }

        public static int GetInt(string key)
        {
            lock (_lock)
            {
                return Convert.ToInt32(Config[key].Trim());
            }

        }

        public static void Update(string keystr, string value)
        {
            lock (_lock)
            {
                string appSettingsJsonFilePath = Path.Combine
                (AppContext.BaseDirectory, "appsettings.json");

                string json = File.ReadAllText(appSettingsJsonFilePath);
                Dictionary<string, object> jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                List<string> keys = keystr.Split(':').ToList();
                jsonObj = TraverseJson2Update(jsonObj, keys, value);

                string output = JsonSerializer.Serialize<Dictionary<string, object>>(
                    jsonObj, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(appSettingsJsonFilePath, output);
            }
        }

        private static Dictionary<string, object> TraverseJson2Update(
            Dictionary<string, object> o, List<string> keys, string value)
        {
            string key = keys[0];

            if (keys.Count == 1)
            {
                o[key] = value;
            }
            else
            {
                keys.RemoveAt(0);
                Dictionary<string, object> jsonObj = JsonSerializer
                    .Deserialize<Dictionary<string, object>>(o[key].ToString());
                o[key] = TraverseJson2Update(jsonObj, keys, value);
            }

            return o;

        }
    }
}
