using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.Interface.Query
{
    /// <summary>
    /// Interface Used for direct query to the database, should be mainly used for performanced based queries or SP execution
    /// Internally Used Dapper to execute queries into database
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Will try to execute handler method from the parameter.
        /// Resolve dependecy on runtime for IQueryHandler<TParameter,TResult>
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TResult> DispatchAsync<TParameter, TResult>(TParameter query)
           where TResult : class;

        /// <summary>
        /// Will try to execute handler method from the parameter.
        /// Resolve dependecy on runtime for IQueryHandler<TParameter,TResult>
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<int> DispatchAsync<TParameter>(TParameter query);
    }
}
