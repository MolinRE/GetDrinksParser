using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Ganss.Excel;
using HtmlAgilityPack;
using Saison;
using Saison.Models.Beer;

namespace GetDrinksParser
{
    class Program
    {
        private static bool RateLimitExceeded;
        private static string dir = @"C:\Users\k.komarov\dev";

        private static GetDrinksRepository _getDrinksRepository = new GetDrinksRepository();

        private static Untappd untappd = new Untappd(
            Environment.GetEnvironmentVariable("UntappdClientId"),
            Environment.GetEnvironmentVariable("UntappdSecret"));
        
        static void Main(string[] args)
        {
            //RateLimitExceeded = false;
            GetDrinks();
        }

        private static void GetDrinks()
        {
            var htmlDocument = new HtmlDocument();

            string getdrinksFile = dir + "\\getdrinks.html";
            if (File.Exists(getdrinksFile) && ((DateTime.Now - File.GetCreationTime(getdrinksFile)).TotalHours < 24))
            {
                htmlDocument.Load(getdrinksFile);
            }
            else
            {
                Console.WriteLine("Загрузка https://getdrinks.ru/catalog/pivo...");
                try
                {
                    var request = WebRequest.CreateHttp("https://getdrinks.ru/catalog/pivo");
                    var response = request.GetResponse();
                    using (var responseStream = response.GetResponseStream())
                    {
                        htmlDocument.Load(responseStream);
                        htmlDocument.Save(getdrinksFile);
                    }

                    Console.WriteLine("Загрузка завершена.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            
            var contentNode = htmlDocument.DocumentNode;

            var items = contentNode.Descendants("div")
                .Where(p => p.Attributes["class"]?.Value == "ProductCard__CardWrapper-sc-1u74b9b-0 gwutJH").ToList();

            var regex = new Regex(@"Крепость: (\d{1,2}[,\.]?\d{0,2})\s?%");
            var volumeRx = new Regex(@"(0[,\.]\d{1,3})\s?л?");

            var list = new List<GetDrinksBeer>();
            foreach (var item in items)
            {
                var getNode = new Func<string, HtmlNode>(className =>
                    item.Descendants()
                        .Where(p => p.Name == "div" || p.Name == "p")
                        .FirstOrDefault(p => p.Attributes["class"]?.Value == className));

                var beer = new GetDrinksBeer();
                beer.Country = getNode("ProductCard__CardCountry-sc-1u74b9b-14 bhOCax")
                    .InnerText;
                string abv = getNode("ProductCard__CardSubInfo-sc-1u74b9b-15 jnEROK")?.InnerText;
                if (abv != null)
                {
                    var match = regex.Match(abv).Groups[1];
                    beer.Abv = match.Value == "" ? 0 : float.Parse(match.Value.Replace('.', ',')) / 100;
                }

                beer.Style = getNode("ProductCard__InfoSectionRight-sc-1u74b9b-12 hmsHxU")
                    .InnerText;
                var nameNode = getNode("ProductCard__CardTitle-sc-1u74b9b-3 gldMoz");
                beer.Name = WebUtility.HtmlDecode(nameNode.InnerText);
                beer.Name = NormalizeBeerName(beer.Name);
                string href = nameNode.Element("a").Attributes["href"].Value;
                beer.Slug = WebUtility.HtmlDecode(href); //href.Substring(href.LastIndexOf('/') + 1);
                
                var priceNodes = getNode("ProductCard__CardPrice-sc-1u74b9b-6 jFjbGX").ChildNodes
                    .OfType<HtmlTextNode>();
                beer.Price = float.Parse(priceNodes.First().Text.Replace('.', ','));
                var packPrice = getNode("ProductCard__CardPricePerUnit-sc-1u74b9b-9 ghnDcx")
                    .ChildNodes.OfType<HtmlTextNode>();
                beer.PackPrice = float.Parse(packPrice.First().Text.Replace('.', ','));
                
                var beerVolMatch = volumeRx.Match(beer.Name);
                if (beerVolMatch.Success)
                {
                    beer.Volume = (int)(float.Parse(beerVolMatch.Groups[1].Value.Replace('.', ',')) * 1000);
                    beer.Name = beer.Name.Remove(beerVolMatch.Index).Trim();
                }
                
                MapStyle(beer);

                list.Add(beer);
            }

            _getDrinksRepository.Save();

            // CsvSerializer.UseEncoding = Encoding.GetEncoding(12)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string fileName = @"C:\Users\k.komarov\dev\beers.xlsx";
            try
            {
                var excel = new ExcelMapper();
                excel.Save(fileName, list, "Пиво");

                Console.WriteLine($@"Saved to {fileName}");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static string NormalizeBeerName(string name)
        {
            string[] prefixes =
            {
                "пиво", 
                "пивной напиток", 
                "хард-лимонад", 
                "сидр", 
                "Limited edition | Пивной напиток", 
                "Limited edition | Сидр", 
                "медовуха", 
                "напиток игристый"
            };
            foreach (var prefix in prefixes)
            {
                if (name.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                {
                    name = name.Substring(prefix.Length);
                }
            }

            name = name.Replace("Палм", "Palm");
            name = name.Replace("Brewdog Hawkes", "Hawkes");
            name = name.Replace("Paradox Brewery", "Paradox");
            name = Regex.Replace(name, @".*?""(.+?)"".*", "$1");

            return name.Trim();
        }

        private static void MapStyle(GetDrinksBeer beer)
        {
            var dbBeer = _getDrinksRepository.SearchBySlug(beer.Slug);
            if (dbBeer == null)
            {
                // Загружаем с Untappd и сохраняем в репо
                if (LoadBeer(beer))
                {
                    dbBeer = _getDrinksRepository.SearchBySlug(beer.Slug);
                }
            }
            
            if (dbBeer != null)
            {
                beer.Style = dbBeer.UntappdStyle;
                beer.Name = dbBeer.Name;
                beer.Brewery = dbBeer.Brewery;
                beer.UntappdUrl = dbBeer.GetUntappdUrl();
                beer.Abv = dbBeer.Abv;
                beer.Ibu = dbBeer.Ibu;
                beer.Brewery = dbBeer.Brewery;

                // Обновляем данные в репо
                if (beer.Volume > 0)
                {
                    dbBeer.GetDrinksVolume = beer.Volume;
                }

                dbBeer.GetDrinksPrice = beer.Price;
                dbBeer.GetDrinksPackPrice = beer.PackPrice;
            }
        }

        private static bool LoadBeer(GetDrinksBeer getDrinksBeer)
        {
            if (RateLimitExceeded)
            {
                return false;
            }
            
            SearchItem untappdBeer;
            try
            {
                untappdBeer = untappd.Beer.Search(getDrinksBeer.Name).Response.Beers.Items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                RateLimitExceeded = true;
                return false;
            }
            
            if (untappdBeer == null)
            {
                if (!_getDrinksRepository.IsUnmatched(getDrinksBeer.Slug))
                {
                    Console.WriteLine($"Не найдено в Untappd: {getDrinksBeer.Name}");
                    _getDrinksRepository.AddUnmatched(getDrinksBeer);
                }
            }

            if (untappdBeer?.Beer != null)
            {
                _getDrinksRepository.AddBeer(getDrinksBeer, untappdBeer);
                return true;
            }
            
            return false;
        }
    }
}