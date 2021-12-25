using System.Globalization;

namespace GetDrinksParser.Helpers
{
    public static class StringExtensions
    {
        private static readonly CultureInfo CultureInfoRu = new CultureInfo("ru-RU");
        
        public static float ParseAsFloat(this string s)
        {
            return float.Parse(s.Replace('.', ','), NumberStyles.Any, CultureInfoRu);
        }
    }
}