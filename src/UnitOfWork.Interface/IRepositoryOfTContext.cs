using Microsoft.EntityFrameworkCore;

namespace UnitOfWork.Interface
{
    public interface IRepository<TEntity, TContext> : IRepository<TEntity>
         where TEntity : class
         where TContext : DbContext
    {
    }
}
