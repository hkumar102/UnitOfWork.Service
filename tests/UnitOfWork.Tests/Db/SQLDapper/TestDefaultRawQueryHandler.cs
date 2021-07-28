using SqlKata;
using SqlKata.Compilers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork.Interface;
using UnitOfWork.Interface.Query;
using UnitOfWork.Model;
using UnitOfWork.Model.Query;
using UnitOfWork.Service.Collections;
using UnitOfWork.Tests.Db.Entities;

namespace UnitOfWork.Tests.Db.SQLDapper
{
    public class TestDefaultRawQueryHandler :
        IQueryHandler<DefaultQuery, IEnumerable<Menu>>,
        IQueryHandler<DefaultQuery, int>,
        IQueryHandler<DefaultQuery, IEnumerable<MenuType>>,
        IQueryHandler<DefaultQuery, IList<Menu>>
    {
        private readonly ISqlScriptContext _sqlContext;

        public TestDefaultRawQueryHandler(ISqlScriptContext sqlContext)
        {
            _sqlContext = sqlContext;
        }

        async Task<IEnumerable<Menu>> IQueryHandler<DefaultQuery, IEnumerable<Menu>>.ExecuteAsync(DefaultQuery parameter)
        {
            var sqlQuery = @"SELECT * FROM Config.Menu";
            var result = (await _sqlContext.SelectAsync<Menu>(sqlQuery, parameter));
            return result;
        }

        async Task<int> IQueryHandler<DefaultQuery, int>.ExecuteAsync(DefaultQuery parameter)
        {
            var sqlQuery = @"EXEC [TreeSetup].[usp_RemoveTree] @TreeId = @TreeId";
            var result = (await _sqlContext.ExecuteAsync(sqlQuery, parameter));
            return result;
        }

        async Task<IEnumerable<MenuType>> IQueryHandler<DefaultQuery, IEnumerable<MenuType>>.ExecuteAsync(DefaultQuery parameter)
        {
            var query = @"SELECT TOP (1000) [Id]
                          ,[CreatedOn]
                          ,[UpdatedOn]
                          ,[IsDeleted]
                          ,[Title]
                          ,[Icon]
                          ,[IsRoot]
                          ,[PageUrl]
                          ,[ParentMenuId]
                          ,[MenuTypeId]
                          ,[OrderNo]
                      FROM [Config].[Menu];


                          SELECT TOP (1000) [Id]
                          ,[CreatedOn]
                          ,[UpdatedOn]
                          ,[IsDeleted]
                          ,[Name]
                          ,[Code]
                      FROM [Config].[MenuType]";

            var querymapItems = new List<QueryMapItem>()
            {
                new QueryMapItem(typeof(Menu), DataRetriveTypeEnum.List, "Menu"),
                new QueryMapItem(typeof(MenuType), DataRetriveTypeEnum.List, "RoleMenu")
            };

            var result = await _sqlContext.ExecuteQueryMultipleAsync(query, parameter, querymapItems);
            var menuTypes = ((IEnumerable<object>)result["RoleMenu"]).Convert<IEnumerable<MenuType>>();
            var menus = ((IEnumerable<object>)result["Menu"]).Convert<IEnumerable<Menu>>();
            foreach (var menuType in menuTypes)
            {
                menuType.Menus.AddRange(menus.Where(x => x.MenuTypeId == menuType.Id));
            }

            return menuTypes;
        }

        async Task<IList<Menu>> IQueryHandler<DefaultQuery, IList<Menu>>.ExecuteAsync(DefaultQuery parameter)
        {
            var compiler = new SqlServerCompiler();
            var query = new Query("Config.Menu");
            SqlResult sqlResult = compiler.Compile(query);
            string sql = sqlResult.Sql;

            var result = (await _sqlContext.SelectAsync<Menu>(sql, parameter)).ToList();
            return result;
        }
    }
}
