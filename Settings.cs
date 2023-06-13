using System;
using System.IO;

namespace GetDrinksParser
{
    internal class Settings
    {
        internal static readonly string DevDirectory = GetDevDirectory();
        internal static readonly string DataDirectory = GetDataDirectory();
        internal static readonly string UntappdClientId = Environment.GetEnvironmentVariable("UNTAPPD_CLIENT_ID");
        internal static readonly string UntappdClientSecret = Environment.GetEnvironmentVariable("UNTAPPD_CLIENT_SECRET");

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
            if (string.IsNullOrEmpty(UntappdClientId) || string.IsNullOrEmpty(UntappdClientSecret))
            {
                Console.WriteLine("Не заданы ключи для интеграции с Untappd. Загрузка данных с Untappd выключена.");
                RateLimitExceeded = true;
            }
        }
    }
}