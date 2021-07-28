using System;
using System.Collections.Generic;
using System.Text;
using UnitOfWork.Tests.Db.Entities;

namespace UnitOfWork.Tests.Db
{
    public static class DataSeeder
    {

        public static void SetData()
        {
            TestCountries = new List<Country>();
            TestCities = new List<City>();
            TestTowns = new List<Town>();
            TestRoleMenus = new List<RoleMenu>();
            var countryA = new Country { CountryId = Guid.NewGuid(), Name = "A" };
            var countryB = new Country { CountryId = Guid.NewGuid(), Name = "B" };

            TestCountries.Add(countryA);
            TestCountries.Add(countryB);


            foreach (var country in TestCountries)
            {
                for (int i = 0; i < 3; i++)
                {
                    var city = new City { CityId = Guid.NewGuid(), Name = "A", CountryId = country.CountryId };
                    var town = new Town { TownId = Guid.NewGuid(), Name = "TownA", CityId = city.CityId };
                    TestCities.Add(city);
                    TestTowns.Add(town);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                TestRoleMenus.Add(new RoleMenu { MenuId = i + 1, RoleId = 1, Id = i + 1 });
            }
        }

        public static List<Country> TestCountries { get; set; }

        public static List<City> TestCities { get; set; }

        public static List<Town> TestTowns { get; set; }

        public static List<RoleMenu> TestRoleMenus { get; set; }
    }
}
