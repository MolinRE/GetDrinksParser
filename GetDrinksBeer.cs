using System;
using Ganss.Excel;

namespace GetDrinksParser
{
    internal class GetDrinksBeer
    {
        private float _abv;
        
        [Column("Страна")]
        public string Country { get; set; }

        [Column("Крепость")]
        [DataFormat("0.00%")]
        public float Abv
        {
            get => _abv < 1 ? _abv : _abv / 100;
            set => _abv = value;
        }
        
        [Column("IBU")]
        public int? Ibu { get; set; }

        [Column("Стиль")]
        public string Style { get; set; }
        
        /// <summary>
        /// Название пива на сайте GetDrinks.
        /// </summary>
        [Column("Название")]
        public string Name { get; set; }
        
        [Column("Пивоварня")]
        public string Brewery { get; set; }

        [Column("Объём (мл.)")]
        public int Volume { get; set; }
        
        [Column("Цена за шт.")]
        [DataFormat("0.00")]
        public float Price { get; set; }
        
        [Column("Цена за упак.")]
        [DataFormat("0.00")]
        public float PackPrice { get; set; }
        
        [Column("Количество в упак.")]
        public int PackQuantity => (int)Math.Round(PackPrice / Price);

        // [Ignore]
        [Column("GetDrinksUrl")]
        public string Slug { get; set; }

        [Column("Untappd")]
        public string UntappdUrl { get; set; }
    }
}