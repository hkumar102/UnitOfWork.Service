using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWork.Tests.Db.Entities
{
    public class Town : BaseEntity
    {
        public Guid TownId { get; set; }
        public Guid CityId { get; set; }
        public City City { get; set; }
        public string Name { get; set; }
    }
}
