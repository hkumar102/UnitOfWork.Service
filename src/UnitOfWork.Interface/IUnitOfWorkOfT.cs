using Microsoft.EntityFrameworkCore;

namespace UnitOfWork.Interface
{
    public interface IUnitOfWork<TKey> : IUnitOfWork
    {

    }

    public interface IUnitOfWork<TKey, TContext> : IUnitOfWork
        where TContext : DbContext
    {
    }

}
