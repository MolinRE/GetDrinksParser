using System;
using System.IO;

namespace GetDrinksParser
{
    internal class Settings
    {
        internal static readonly string DevDirectory = Settings.GetDevDirectory();
        internal static readonly string DataDirectory = Settings.GetDataDirectory();
        
        internal static bool RateLimitExceeded;
        
        internal static string GetDataDirectory()
        {
            var data = Environment.CurrentDirectory;
            data = data.Remove(data.IndexOf("GetDrinksParser") + 15);
            data = Path.Combine(data, "Data");
            return data;
        }

        public static string GetDevDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dev");
        }
        
        internal static void ValidateSaisonConfig()
        {
            var clientId = Environment.GetEnvironmentVariable("UntappdClientId");
            var secret = Environment.GetEnvironmentVariable("UntappdSecret");
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(secret))
            {
                Console.WriteLine("Не заданы ключи для интеграции с Untappd. Загрузка данных с Untappd выключена.");
                RateLimitExceeded = true;
            }
        }
    }
}