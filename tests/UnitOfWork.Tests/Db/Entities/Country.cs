using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWork.Tests.Db.Entities
{
    public class Country : BaseEntity
    {
        public Guid CountryId { get; set; }
        public string Name { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
