using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitOfWork.Interface.Query
{

    /// <summary>
    /// Implement this interface for raw queries
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IQueryHandler<in TParam, TResult>
    {
        // TODO: Read scripts from file
        /// <summary>
        /// Full file path to reach script and execute instead of writing queries directly in the code
        /// </summary>
        //string ScriptFile { get; set; }

        /// <summary>
        /// Execute the Handler for the Type Passsed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<TResult> ExecuteAsync(TParam parameter);
    }

}
