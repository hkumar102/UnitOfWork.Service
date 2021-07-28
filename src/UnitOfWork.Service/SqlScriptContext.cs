using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using UnitOfWork.Interface;
using UnitOfWork.Model.Query;
using System.Linq;
using UnitOfWork.Model;
using UnitOfWork.Interface.Query;
using UnitOfWork.Service.Collections;
using System.Data;

namespace UnitOfWork.Service
{
    public class SqlScriptContext : ISqlScriptContext
    {
        private readonly Func<SqlConnection> _dbConnectionFactory;
        private readonly int CommandTimeout = 30;

        public SqlScriptContext(Func<SqlConnection> dbConnectionFactory, int commandTimeout = 30)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            CommandTimeout = commandTimeout;
        }

        #region Private Methods
        private async Task<T> CommandAsync<T>(Func<SqlConnection, SqlTransaction, int, Task<T>> command)
        {
            using (var connection = _dbConnectionFactory.Invoke())
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await command(connection, transaction, CommandTimeout);

                        transaction.Commit();

                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private async Task<IDictionary<string, object>> ExecuteScriptAsync(string sql, object parameter = null, CommandType commandType = CommandType.Text, params QueryMapItem[] queryMapItems)
        {
            return await CommandAsync<IDictionary<string, object>>(async (conn, transcation, timeout) =>
            {
                try
                {
                    var data = new Dictionary<string, object>();

                    using (var dbResult = await conn.QueryMultipleAsync(sql, parameter, transcation, timeout, commandType))
                    {
                        if (queryMapItems == null) return data;

                        foreach (var queryMapItem in queryMapItems)
                        {
                            switch (queryMapItem.DataRetriveType)
                            {
                                case DataRetriveTypeEnum.FirstOrDefault:
                                    var singleItem = dbResult.Read(queryMapItem.Type).FirstOrDefault();
                                    data.Add(queryMapItem.PropertyName, singleItem);
                                    break;
                                case DataRetriveTypeEnum.List:
                                    var listItem = dbResult.Read(queryMapItem.Type).ToList();
                                    data.Add(queryMapItem.PropertyName, listItem);
                                    break;
                                case DataRetriveTypeEnum.Empty:
                                    // Passing one becasue script executed successfully and do not return data
                                    // Probably be SP execution returning nothing
                                    data.Add(queryMapItem.PropertyName, 1);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    return data;

                }
                catch (Exception)
                {

                    throw;
                }

            });
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Execute sql queries which will not return any result, Mainly SP which return int as success
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, object parameter, CommandType commandType = CommandType.Text)
        {

            try
            {
                var dbResult = await ExecuteScriptAsync(sql, parameter, commandType, new QueryMapItem(typeof(int), DataRetriveTypeEnum.Empty, "Return"));
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute Sql Query and return First Record from the dataset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string sql, object parameter, CommandType commandType = CommandType.Text)
        {
            var dbResult = await ExecuteScriptAsync(sql, parameter, commandType, new QueryMapItem(typeof(T), DataRetriveTypeEnum.FirstOrDefault, "Object1"));
            var result = dbResult["Object1"].Convert<T>();
            return result;
        }


        /// <summary>
        /// Execute Sql Query and return IEnumerable<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SelectAsync<T>(string sql, object parameter, CommandType commandType = CommandType.Text)
        {
            var dbResult = await ExecuteScriptAsync(sql, parameter, commandType, new QueryMapItem(typeof(T), DataRetriveTypeEnum.List, "Object1"));
            var result = dbResult["Object1"].Convert<IEnumerable<T>>();
            return result;
        }


        /// <summary>
        /// Execute Sql Query which return multiple dataset. Use extension method Convert of IDictionary to get the desired IEnumerable Datasets
        /// var result = await _sqlContext.ExecuteQueryMultipleAsync(query, parameter, querymapItems);
        /// var type1 = result["type1").Convert<T1>();
        /// var type2 = result["type2").Convert<T2>();
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="queryMapItems"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> ExecuteQueryMultipleAsync(string sql, object parameter = null, IEnumerable<QueryMapItem> queryMapItems = null, CommandType commandType = CommandType.Text)
        {

            if (queryMapItems == null || queryMapItems.Count() == 0)
                throw new ArgumentNullException("queryMapItems cannot be null or empty");

            return await ExecuteScriptAsync(sql, parameter, commandType, queryMapItems.ToArray());


        }


        /// <summary>
        /// Execute Sql Query which return multiple dataset. Use extension method Convert of IDictionary to get the desired IEnumerable Datasets
        /// var result = await _sqlContext.ExecuteQueryMultipleAsync(query, parameter, querymapItems);
        /// var type1 = result["type1").Convert<T1>();
        /// var type2 = result["type2").Convert<T2>();
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="queryMapItems"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> ExecuteQueryMultipleAsync(string sql, IQuery parameter = null, CommandType commandType = CommandType.Text, params QueryMapItem[] queryMapItems)
        {
            return await ExecuteScriptAsync(sql, parameter, commandType, queryMapItems);
        }

        #endregion
    }
}
