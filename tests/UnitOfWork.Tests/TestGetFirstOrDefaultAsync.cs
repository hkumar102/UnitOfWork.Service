using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitOfWork.Interface;
using UnitOfWork.Interface.Providers;
using UnitOfWork.Interface.Query;
using UnitOfWork.Model;
using UnitOfWork.Model.Query;
using UnitOfWork.Service.Collections;
using UnitOfWork.Tests.Db.Entities;
using UnitOfWork.Tests.Db.SQLDapper;
using Xunit;

namespace UnitOfWork.Tests
{
    public class TestGetFirstOrDefaultAsync : TestBase
    {

        [Fact]
        public async void TestRawSqlQuery()
        {
            var unitOfWork = GetService<IUnitOfWork>();
            var dispatcher = unitOfWork.GetQueryDispatcher();
            var handler1 = GetService<IQueryHandler<DefaultQuery, IEnumerable<Menu>>>();
            var result = await dispatcher.DispatchAsync<DefaultQuery, IEnumerable<Menu>>(new DefaultQuery());
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void TestRawSqlQueryUsingSQLKata()
        {
            var dispatcher = GetService<IQueryDispatcher>();
            var result = await dispatcher.DispatchAsync<DefaultQuery, IList<Menu>>(new DefaultQuery());
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void TestRawSqlSP()
        {
            var dispatcher = GetService<IQueryDispatcher>();
            var result = await dispatcher.DispatchAsync(new DefaultQuery { TreeId = Guid.NewGuid() });
            Assert.Equal(1, result);
        }


        [Fact]
        public async void TestRawSqlQueryMultipleDataSet()
        {
            var dispatcher = GetService<IQueryDispatcher>();
            var result = await dispatcher.DispatchAsync<DefaultQuery, IEnumerable<MenuType>>(new DefaultQuery());
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async void TestPagination(int pageIndex)
        {
            var unitOfWork = GetService<IUnitOfWork>();
            var repository = unitOfWork.GetRepository<RoleMenu>();

            repository.Where(x => x.Id == 2);

            var result = await repository.GetPagedListAsync((s) => s, pageIndex: pageIndex, pageSize: 2, include: inc => inc.Include(x => x.Menu).ThenInclude(m => m.MenuType));
            Assert.NotNull(result);

        }

        [Fact]
        public void TestDependency()
        {
            var userProvider = GetService<IUserInformationProvider<int>>();
            var tenantProvider = GetService<ITenantProvider<int>>();
            var auditProvider = GetService<IAuditStampProvider<int>>();
            var handler1 = GetService<IQueryHandler<DefaultQuery, IEnumerable<Menu>>>();
            var handler2 = GetService<IQueryHandler<DefaultQuery, IEnumerable<int>>>();
            var handler3 = GetService<IQueryHandler<DefaultQuery, IEnumerable<MenuType>>>();
            var handler4 = GetService<IQueryHandler<DefaultQuery, IList<Menu>>>();
        }

        [Fact]
        public async void ExecuteSPUsingUOW()
        {
            var unitOfWork = GetService<IUnitOfWork>();
            var sql = "EXEC [TreeSetup].[usp_RemoveTree] @TreeId = @TreeId";
            var result1 = await unitOfWork.ExecuteSqlAsync(sql, new { TreeId = Guid.NewGuid() });

            sql = "SELECT * FROM Config.Menu";
            var result2 = await unitOfWork.ExecuteSqlAsync<Menu>(sql, new { TreeId = Guid.NewGuid() });

            sql = @"SELECT TOP (1000) [Id]
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
            FROM[Config].[Menu];


            SELECT TOP(1000) [Id]
                          ,[CreatedOn]
                          ,[UpdatedOn]
                          ,[IsDeleted]
                          ,[Name]
                          ,[Code]
            FROM[Config].[MenuType]";

            var querymapItems = new List<QueryMapItem>()
            {
                new QueryMapItem(typeof(Menu), DataRetriveTypeEnum.List, "Menu"),
                new QueryMapItem(typeof(MenuType), DataRetriveTypeEnum.List, "RoleMenu")
            };

            var result3 = await unitOfWork.ExecuteSqlAsync(sql, null, querymapItems);
            var menuTypes = result3["RoleMenu"].Convert<IEnumerable<MenuType>>();
            var menus = result3["Menu"].Convert<IEnumerable<Menu>>();
            foreach (var menuType in menuTypes)
            {
                menuType.Menus.AddRange(menus.Where(x => x.MenuTypeId == menuType.Id));
            }

        }
    }
}
