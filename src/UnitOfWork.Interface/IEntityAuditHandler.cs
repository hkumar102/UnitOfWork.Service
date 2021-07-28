using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWork.Interface
{
    public interface IEntityAuditHandler<TKey>
         where TKey : struct

    {
        void Handle();
    }
}
