using System;

namespace UnitOfWork.Tests.Db.Entities
{
    public class Customer : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }

        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid TownId { get; set; }

        public Country Country { get; set; }
        public City City { get; set; }
        public Town Town { get; set; }

    }
}
