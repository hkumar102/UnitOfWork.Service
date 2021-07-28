using System;
using System.Threading.Tasks;
using UnitOfWork.Interface.Query;

namespace UnitOfWork.Service.Query
{
    /// <summary>
    /// SQL Raw query dispatcher. should be mainly used for performanced based queries or SP execution
    /// Using Dapper to map parameters to query
    /// </summary>
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _dependencyResolver;
        public QueryDispatcher(IServiceProvider dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        private IQueryHandler<TParameter, TResult> ResolveIQueryHandler<TParameter, TResult>()
        {
            var handler = _dependencyResolver.GetService(typeof(IQueryHandler<TParameter, TResult>)) as IQueryHandler<TParameter, TResult>;

            if (handler == null)
            {
                throw new InvalidOperationException($"Could not resolve service from IOC Container, Please check if you have registered type { typeof(IQueryHandler<TParameter, TResult>) }");
            }

            return handler;
        }

        /// <summary>
        /// Will try to execute handler method from the parameter.
        /// Resolve dependecy on runtime for IQueryHandler<TParameter,TResult>
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TResult> DispatchAsync<TParameter, TResult>(TParameter query)
            where TResult : class
        {
            var handler = ResolveIQueryHandler<TParameter, TResult>();
            return await handler.ExecuteAsync(query);
        }

        /// <summary>
        /// Will try to execute handler method from the parameter.
        /// Resolve dependecy on runtime for IQueryHandler<TParameter,TResult>
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<int> DispatchAsync<TParameter>(TParameter query)
        {
            var handler = ResolveIQueryHandler<TParameter, int>();
            return await handler.ExecuteAsync(query);
        }

    }
}
