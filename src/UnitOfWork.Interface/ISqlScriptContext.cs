using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnitOfWork.Interface.Query;
using UnitOfWork.Model.Query;

namespace UnitOfWork.Interface
{
    public interface ISqlScriptContext
    {
        Task<T> GetAsync<T>(string sql, object parameter, CommandType commandType = CommandType.Text);
        Task<IEnumerable<T>> SelectAsync<T>(string sql, object parameter, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object parameter, CommandType commandType = CommandType.Text );
        Task<IDictionary<string, object>> ExecuteQueryMultipleAsync(string sql, object parameter = null, IEnumerable<QueryMapItem> queryMapItems = null, CommandType commandType = CommandType.Text);
    }
}
