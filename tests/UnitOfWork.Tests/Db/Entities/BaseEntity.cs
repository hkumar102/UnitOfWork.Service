using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWork.Tests.Db.Entities
{
    public interface IBaseEntity { }
    public class BaseEntity : IBaseEntity
    {
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
