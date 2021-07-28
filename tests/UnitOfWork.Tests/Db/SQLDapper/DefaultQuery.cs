using System;
using System.Collections.Generic;
using System.Text;
using UnitOfWork.Interface.Query;

namespace UnitOfWork.Tests.Db.SQLDapper
{
    public class DefaultQuery : IQuery
    {
        public Guid TreeId { get; set; }
    }
}
