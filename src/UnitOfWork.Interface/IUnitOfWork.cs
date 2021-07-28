using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork.Interface.Query;
using UnitOfWork.Model.Query;

namespace UnitOfWork.Interface
{
    public interface IUnitOfWork : IRepositoryBuilder
    {

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Execute SQL script directly to the database using dapper
        /// </summary>
        /// <param name="sql">SQL script or SP, IE EXEC SP_NAME @PARAM = @PARAM</param>
        /// <param name="parameter">new  { PARAM = 1}</param>
        /// <returns></returns>
        Task<int> ExecuteSqlAsync(string sql, object parameter);


        /// <summary>
        /// Execute SQL script directly to the database using dapper
        /// </summary>
        /// <param name="sql">SQL script or SP, IE EXEC SP_NAME @PARAM = @PARAM</param>
        /// <param name="parameter">new  { PARAM = 1}</param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> ExecuteSqlAsync<TResult>(string sql, object parameter);

        /// <summary>
        /// Execute SQL adn return multiple DataSet
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="queryMapItems"></param>
        /// <returns></returns>
        Task<IDictionary<string, object>> ExecuteSqlAsync(string sql, object parameter, IEnumerable<QueryMapItem> queryMapItems);

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        DbContext DbContext { get; }

    }
}
