using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using GetDrinksParser.Models;
using Saison;

namespace GetDrinksParser
{
    public class DbContainer
    {
        public List<BeerInfo> Beers { get; set; }
        
        /// <summary>
        /// Список пив с GetDrinks, которые не получилось найти в Untappd по названию. Его нужно будет обработать вручную.
        /// </summary>
        public List<BeerInfo> Unmatched { get; set; }

        public DbContainer()
        {
            Beers = new List<BeerInfo>();
            Unmatched = new List<BeerInfo>();
        }
    }
    
    internal class GetDrinksRepository
    {
        private static string fileName = Path.Combine(Settings.DataDirectory, "getdrinks_beer_data.json");
        
        private DbContainer Data;
        
        public GetDrinksRepository()
        {
            if (File.Exists(fileName))
            {
                Data = JsonSerializer.Deserialize<DbContainer>(File.ReadAllText(fileName));
            }
            else
            {
                Data = new DbContainer();
            }
        }

        public async Task ClearMatched()
        {
            Data.Unmatched.RemoveAll(p => p.Name != null && Data.Beers.Any(p2 => p2.Url.Equals(p.Url, StringComparison.CurrentCultureIgnoreCase)));
            await Save();
        }
        
        public BeerInfo SearchBySlug(string slug)
        {
            return Data.Beers.FirstOrDefault(p => p.Url.Equals(slug, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task AddBeer(GetDrinksBeer getDrinksBeer, Saison.Models.Beer.SearchItem beer)
        {
            string slug = getDrinksBeer.Slug;

            // Удаляем предыдущие записи
            Data.Beers.RemoveAll(p => p.Url.Equals(slug, StringComparison.CurrentCultureIgnoreCase));
            
            Data.Beers.Add(new BeerInfo()
            {
                UntappdId = beer.Beer.Bid,
                UntappdSlug = beer.Beer.BeerSlug,
                UntappdLabel = beer.Beer.BeerLabel,
                Name = beer.Beer.BeerName,
                OriginalName = getDrinksBeer.Name,
                Url = getDrinksBeer.Slug,
                UntappdStyle = beer.Beer.BeerStyle,
                Brewery = beer.Brewery.BreweryName,
                BreweryId = beer.Brewery.BreweryId,
                BrewerySlug = beer.Brewery.BrewerySlug,
                Abv = beer.Beer.BeerAbv,
                Ibu = beer.Beer.BeerIbu > 0 ? (int?)beer.Beer.BeerIbu : null,
                GetDrinksPrice = getDrinksBeer.Price,
                GetDrinksPackPrice = getDrinksBeer.PackPrice,
                GetDrinksVolume = getDrinksBeer.Volume
            });

            await Save();
        }

        public async Task AddUnmatched(GetDrinksBeer beer)
        {
            Data.Unmatched.Add(new BeerInfo()
            {
                OriginalName = beer.Name,
                Url = beer.Slug
            });

            await Save();
        }

        public bool IsUnmatched(string slug)
        {
            return Data.Unmatched.Any(p => p.Url.Equals(slug, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task Save()
        {
            await File.WriteAllTextAsync(fileName, 
                JsonSerializer.Serialize(Data, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                }), 
                Encoding.UTF8);
        }
    }
}