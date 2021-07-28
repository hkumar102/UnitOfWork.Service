using Microsoft.EntityFrameworkCore;
using System;
using UnitOfWork.Interface;

namespace UnitOfWork.Service
{
    internal class UnitOfWork<TKey, TContext> : UnitOfWork<TKey>, IUnitOfWork<TKey, TContext>
        where TKey : struct
        where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public UnitOfWork(
                TContext context,
                IServiceProvider serviceProvider,
                ISqlScriptContext sqlContext,
                IEntityAuditHandler<TKey> entityAuditHandler = null) : base(context, serviceProvider, sqlContext, entityAuditHandler)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        public override IRepository<TEntity> GetRepository<TEntity>()
        {
            Repository<TEntity> repos = new Repository<TEntity>(_dbContext);
            return repos;
        }
    }
}
