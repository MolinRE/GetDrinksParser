using System;
using Ganss.Excel;

namespace GetDrinksParser
{
    internal class GetDrinksItem
    {
        [Column("Страна")]
        public string Country { get; set; }
        
        [Column("Крепость")]
        [DataFormat("0.00%")]
        public float Abv { get; set; }
        
        [Column("Стиль")]
        public string Style { get; set; }
        
        [Column("Название")]
        public string Name { get; set; }

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

        [Ignore]
        public string Slug { get; set; }
    }
}