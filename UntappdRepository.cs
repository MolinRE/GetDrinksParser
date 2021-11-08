using System;
using System.Collections.Generic;
using System.Linq;

namespace GetDrinksParser
{
    public class UntappdBeer
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Style { get; set; }

        public UntappdBeer()
        {
            
        }

        public UntappdBeer(int id, string slug, string name, string style)
        {
            Id = id;
            Slug = slug;
            Name = name;
            Style = style;
        }
    }
    
    public class UntappdRepository
    {
        private List<UntappdBeer> data = new List<UntappdBeer>()
        {
            new UntappdBeer(1516764,
                "moscow-brewing-company-moskovskaya-pivovarennaya-kompaniya-zhiguli-barnoe-non-alcoholic-zhiguli-barnoe-bezalkogolnoe",
                "Жигули Барное Безалкогольное", "Non-Alcoholic Beer"),
            new UntappdBeer(26210, "moscow-brewing-company-moskovskaya-pivovarennaya-kompaniya-zhiguli-barnoe-zhiguli-barnoe", "Жигули Барное", "Lager - Pale"),
            new UntappdBeer(242217, "moscow-brewing-company-moskovskaya-pivovarennaya-kompaniya-hamovniki-bavarskoe-pshenichnoe-hamovniki-bavarskoe-pshenichnoe", "Хамовники Баварское Пшеничное", "Wheat Beer - Hefeweizen"),
            new UntappdBeer(229044, "moscow-brewing-company-moskovskaya-pivovarennaya-kompaniya-hamovniki-venskoe-hamovniki-venskoe", "Хамовники Венское", "Lager - Vienna"),
            new UntappdBeer(242215, "moscow-brewing-company-moskovskaya-pivovarennaya-kompaniya-hamovniki-munhenskoye-hamovniki-myunhenskoe", "Хамовники Мюнхенское", "Märzen"),
            new UntappdBeer(1561153, "wolf-s-brewery-volkovskaya-pivovarnya-nepravilnyi-mead-nepravilnyj-myod", "Волковская Пивоварня Неправильный мёд", "Mead - Other"),
            new UntappdBeer(479411, "staropilsen-zlata-praha", "Zlatá Praha", "Pilsner - Czech"),
            new UntappdBeer(2593729, "brewdog-lost-lager", "Brewdog Lost Lager", "Pilsner - Other")
        };

        public UntappdBeer SearchByName(string name)
        {
            return data.FirstOrDefault(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}