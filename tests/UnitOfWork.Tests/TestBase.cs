using Microsoft.Extensions.DependencyInjection;
using System;
using UnitOfWork.Tests.Db;
using UnitOfWork.Service;
using UnitOfWork.Tests.Db.SQLDapper;

namespace UnitOfWork.Tests
{

    public abstract class TestBase
    {
        protected readonly IServiceProvider ServiceProvider;
        public TestBase()
        {
            var services = new ServiceCollection();

            var connectionString = "Data Source=biglynx.database.windows.net;Initial Catalog=MLTrees_Core_Dev;Persist Security Info=True;User ID=delhidev;Password=Suqu8169";

            if (string.IsNullOrEmpty(connectionString)) throw new ApplicationException("ConnectionString cannnot be nulll");

            services.RegisterUnitOfWork<int, InMemoryContext, TestDefaultRawQueryHandler>(connectionString);
            ServiceProvider = services.BuildServiceProvider();
        }

        protected T GetService<T>() where T : class
        {
            return ServiceProvider.GetService(typeof(T)) as T;
        }
    }
}
