using Microsoft.EntityFrameworkCore;
using UnitOfWork.Interface;

namespace UnitOfWork.Service
{
    internal class Repository<TEntity, TContext> : Repository<TEntity>, IRepository<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        public Repository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
