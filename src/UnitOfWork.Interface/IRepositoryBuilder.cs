using UnitOfWork.Interface.Query;

namespace UnitOfWork.Interface
{
    public interface IRepositoryBuilder
    {
        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/> this is related to the entityframewok.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// Return QueryDispatcher to execute query directly on the SQL using dapper and resolve the QueryHandler on runtime
        /// </summary>
        /// <returns></returns>
        IQueryDispatcher GetQueryDispatcher();


        /// <summary>
        /// Return the Query handler to execute query directly on the SQL using dapper
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        IQueryHandler<TParameter, TResult> GetQueryHandler<TParameter, TResult>();
    }
}
