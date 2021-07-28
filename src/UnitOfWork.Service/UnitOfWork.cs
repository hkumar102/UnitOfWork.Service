using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnitOfWork.Interface;
using UnitOfWork.Interface.Query;
using UnitOfWork.Model.Query;

namespace UnitOfWork.Service
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IUnitOfWork"/> and <see cref="IUnitOfWork{TContext}"/> interface.
    /// </summary>
    /// <typeparam name="TContext">The type of the db context.</typeparam>
    /// <typeparam name="TKey">The type of the primary key of IdentityTable like User and Tenant</typeparam>
    internal class UnitOfWork<TKey> : IUnitOfWork<TKey>, IUnitOfWork
         where TKey : struct
    {
        private readonly DbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEntityAuditHandler<TKey> _entityAuditHandler;
        private readonly ISqlScriptContext _sqlContext;
        private bool disposed = false;


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serviceProvider">Dependency Resolver to resolve IRepository Services on runtime</param>
        /// <param name="userInformationProvider">Provider to get UserId</param>
        /// <param name="tenantProvider">Provider to user TenantId, If not null then application will be marked as Multi-Tenant</param>
        /// <param name="auditStampProvider">Provider Set Default Audit proerties when a entity being created/modfified/deleted</param>
        /// <param name="softDeleteProvider">Proivder to enable soft delete for the application meaning records from the database will not be a hard delete</param>
        public UnitOfWork(
                DbContext context,
                IServiceProvider serviceProvider,
                ISqlScriptContext sqlContext,
                IEntityAuditHandler<TKey> entityAuditHandler = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _entityAuditHandler = entityAuditHandler;
            _sqlContext = sqlContext;
        }
        #endregion

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        public DbContext DbContext => _context;

        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            Repository<TEntity> repos = new Repository<TEntity>(_context);
            return repos;
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges()
        {
            if (_entityAuditHandler != null) _entityAuditHandler.Handle();
            return _context.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// IEntityAuditHandler is not null then call its Handle Method for audit properties
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync()
        {
            if (_entityAuditHandler != null) _entityAuditHandler.Handle();
            return await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // dispose the db context.
                    _context.Dispose();
                }
            }

            disposed = true;
        }

        /// <summary>
        /// Return QueryDispatcher to execute query directly on the SQL using dapper and resolve the QueryHandler on runtime
        /// </summary>
        /// <returns>IQueryDispatcher</returns>
        public IQueryDispatcher GetQueryDispatcher()
        {
            var queryDispatcher = _serviceProvider.GetService(typeof(IQueryDispatcher)) as IQueryDispatcher;
            return queryDispatcher;
        }

        /// <summary>
        /// Return the Query handler to execute query directly on the SQL using dapper
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IQueryHandler<TParameter, TResult></returns>
        public IQueryHandler<TParameter, TResult> GetQueryHandler<TParameter, TResult>() 
        {
            var handler = _serviceProvider.GetService(typeof(IQueryHandler<TParameter, TResult>)) as IQueryHandler<TParameter, TResult>;
            return handler;
        }

        /// <summary>
        /// Execute SQL script directly to the database using dapper
        /// </summary>
        /// <param name="sql">SQL script or SP, IE EXEC SP_NAME @PARAM = @PARAM_IN</param>
        /// <param name="parameter">new  { PARAM_IN = 1}</param>
        /// <returns></returns>
        public Task<int> ExecuteSqlAsync(string sql, object parameter)
        {
            return _sqlContext.ExecuteAsync(sql, parameter);
        }

        /// <summary>
        /// Execute SQL script directly to the database using dapper
        /// </summary>
        /// <param name="sql">SQL script or SP, IE EXEC SP_NAME @PARAM = @PARAM_IN</param>
        /// <param name="parameter">new  { PARAM_IN = 1}</param>
        /// <returns></returns>
        public Task<IEnumerable<TResult>> ExecuteSqlAsync<TResult>(string sql, object parameter)
        {
            return _sqlContext.SelectAsync<TResult>(sql, parameter);
        }

        /// <summary>
        /// Execute SQL adn return multiple DataSet
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql">SQL script or SP, IE EXEC SP_NAME @PARAM = @PARAM_IN</param>
        /// <param name="parameter">new  { PARAM_IN = 1}</param>
        /// <param name="queryMapItems"></param>
        /// <returns></returns>
        public Task<IDictionary<string, object>> ExecuteSqlAsync(string sql, object parameter, IEnumerable<QueryMapItem> queryMapItems)
        {
            return _sqlContext.ExecuteQueryMultipleAsync(sql, parameter, queryMapItems);
        }
    }
}
