using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWork.Tests.Db.Entities
{
    public class City : BaseEntity
    {
        public Guid CityId { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        ICollection<Town> Towns { get; set; }
    }
}
