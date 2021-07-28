using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitOfWork.Interface.Query
{
    public interface IQueryContext
    {
        Task<T> GetAsync<T>(string sql, object parameters);
        Task<IList<T>> SelectAsync<T>(string sql, object parameters);
    }
}
