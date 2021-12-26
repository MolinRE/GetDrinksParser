namespace GetDrinksParser.Models
{
	public class BeerInfo
	{
		public int? UntappdId { get; set; }
		public string UntappdSlug { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// Название пива на сайте GetDrinks
		/// </summary>
		public string OriginalName { get; set; }
		public string Url { get; set; }
		public string UntappdStyle { get; set; }
		public float Abv { get; set; }
		public string Brewery { get; set; }
		public int? Ibu { get; set; }
		public float? GetDrinksPrice { get; set; }
		public float? GetDrinksPackPrice { get; set; }
		public int? GetDrinksVolume { get; set; }
		/// <summary>
		/// Картинка пива
		/// </summary>
		public string UntappdLabel { get; set; }

		public int BreweryId { get; set; }
		public string BrewerySlug { get; set; }

		public BeerInfo()
		{
            
		}

		public BeerInfo(string name)
		{
			Name = name;
		}

		public string GetUntappdUrl()
		{
			return $"https://untappd.com/b/{UntappdSlug}/{UntappdId}";
		}
	}
}