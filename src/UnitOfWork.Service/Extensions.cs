using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlClient;
using System.Linq;
using UnitOfWork.Interface;
using UnitOfWork.Interface.Providers;
using UnitOfWork.Interface.Query;
using UnitOfWork.Service.Providers;
using UnitOfWork.Service.Query;

namespace UnitOfWork.Service
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TContext">DbContext</typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString">Connection String to connect to the database.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterQueryHandlerPattern<TQueryHandlerAssemblyFromType>(this IServiceCollection services, string connectionString)
        {
            services
                .AddSqlScriptContext(connectionString)
                .AddUnitOfWorkQueryDispatcher<QueryDispatcher>()
                .AddUnitOfWorkAllQueryHandelers<TQueryHandlerAssemblyFromType>();

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TContext">DbContext</typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString">Connection String to connect to the database.</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlScriptContext(this IServiceCollection services, string connectionString)
        {
            SqlScriptContext sqlScriptContext = new SqlScriptContext(() => new SqlConnection(connectionString));
            services
                .AddTransient<ISqlScriptContext>(x => sqlScriptContext);

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TContext">DbContext</typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString">Connection String to connect to the database.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterUnitOfWork<TKey, TContext>(this IServiceCollection services, string connectionString)
            where TContext : DbContext
            where TKey : struct
        {
            services
                .AddDbContext<TContext>(opt => opt.UseSqlServer(connectionString))
                .AddUnitOfWorkEntityAuditHandler<int, DefaultEntityAuditHandler>()
                .AddUnitOfWorkUserInformationProvider<int, DefaultUserInformationProvider>()
                .AddUnitOfWorkTenantProvider<int, DefaultTenantProvider>()
                .AddUnitOfWorkAuditStampProvider<int, DefaultAuditStampProvider>()
                .AddUnitOfWorkSoftDeleteProvider<DefaultSoftDeleteProvider>()
                .AddUnitOfWork<TKey, TContext, UnitOfWork<TKey, TContext>>()
                .AddSqlScriptContext(connectionString);

            return services;
        }

        /// <summary>
        /// Register UnitofWork dependency in the pipleline with default server and providers
        /// </summary>
        /// <typeparam name="TKey">Primary Key Type for IdentityColumn for User and Tenant</typeparam>
        /// <typeparam name="TContext">DbContext</typeparam>
        /// <typeparam name="TQueryAssemblyAssemblyFromType">Assembly in which all the IQueryHandlers defined to register autimatically.</typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection RegisterUnitOfWork<TKey, TContext, TQueryHandlerAssemblyFromType>(this IServiceCollection services, string connectionString)
            where TContext : DbContext
            where TKey : struct

        {
            services
                .RegisterUnitOfWork<TKey, TContext>(connectionString)
                .RegisterQueryHandlerPattern<TQueryHandlerAssemblyFromType>(connectionString);
            return services;
        }

        /// <summary>
        /// Add UserInformation Provider
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUserInformationProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWorkUserInformationProvider<TKey, TUserInformationProvider>(this IServiceCollection services)
            where TKey : struct
            where TUserInformationProvider : IUserInformationProvider<TKey>
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(IUserInformationProvider<TKey>));
            services.Remove(descriptorToRemove);
            var descriptor = new ServiceDescriptor(typeof(IUserInformationProvider<TKey>), typeof(TUserInformationProvider), ServiceLifetime.Transient);
            services.Add(descriptor);
            return services;
        }


        /// <summary>
        /// Add TenantProvider
        /// </summary>
        /// <typeparam name="TKey">Type of primary key</typeparam>
        /// <typeparam name="TTenantProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWorkTenantProvider<TKey, TTenantProvider>(this IServiceCollection services)
            where TKey : struct
            where TTenantProvider : ITenantProvider<TKey>
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(ITenantProvider<TKey>));
            services.Remove(descriptorToRemove);
            var descriptor = new ServiceDescriptor(typeof(ITenantProvider<TKey>), typeof(TTenantProvider), ServiceLifetime.Transient);
            services.Add(descriptor);
            return services;
        }


        /// <summary>
        /// Add AudtiStamp Provider
        /// </summary>
        /// <typeparam name="TKey">Type of Primary Key</typeparam>
        /// <typeparam name="TAuditStampProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWorkAuditStampProvider<TKey, TAuditStampProvider>(this IServiceCollection services)
            where TKey : struct
            where TAuditStampProvider : IAuditStampProvider<TKey>
        {

            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(IAuditStampProvider<TKey>));
            services.Remove(descriptorToRemove);
            var descriptor = new ServiceDescriptor(typeof(IAuditStampProvider<TKey>), typeof(TAuditStampProvider), ServiceLifetime.Transient);
            services.Add(descriptor);
            return services;
        }

        /// <summary>
        /// Add UnitofWork
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUnitOfWork"></typeparam>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWork<TKey, TUnitOfWork>(this IServiceCollection services)
            where TUnitOfWork : IUnitOfWork<TKey>
        {
            services.AddScoped(typeof(IUnitOfWork<TKey>), typeof(TUnitOfWork));
            return services;
        }

        /// <summary>
        /// Add UnitofWork
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUnitOfWork"></typeparam>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWork<TKey, TContext, TUnitOfWork>(this IServiceCollection services)
            where TContext : DbContext
            where TUnitOfWork : IUnitOfWork<TKey, TContext>
        {
            services.AddScoped(typeof(IUnitOfWork<TKey, TContext>), typeof(TUnitOfWork));
            return services;
        }


        /// <summary>
        /// Add QueryDispatcher to the service collection.
        /// </summary>
        /// <typeparam name="TQueryDispatcher"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWorkQueryDispatcher<TQueryDispatcher>(this IServiceCollection services)
            where TQueryDispatcher : IQueryDispatcher
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(IQueryDispatcher));
            services.Remove(descriptorToRemove);
            var descriptor = new ServiceDescriptor(typeof(IQueryDispatcher), typeof(TQueryDispatcher), ServiceLifetime.Transient);
            services.Add(descriptor);
            return services;
        }

        /// <summary>
        /// Individually adding IQueryHandler
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TQueryHandler"></typeparam>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWorkQueryHandler<TParam, TResult, TQueryHandler>(this IServiceCollection services)
            where TQueryHandler : IQueryHandler<TParam, TResult>
        {
            services.AddTransient(typeof(IQueryHandler<TParam, TResult>), typeof(TQueryHandler));
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="repository"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWorkRepository(this IServiceCollection services, Type repository)
        {

            bool isRepository = repository.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>));

            if (isRepository)
            {
                services.AddScoped(typeof(IRepository<>), repository);
            }

            bool isRepositoryTContext = repository.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>));

            if (isRepositoryTContext)
            {
                services.AddScoped(typeof(IRepository<,>), repository);
            }

            if (!isRepository && !isRepositoryTContext) throw new InvalidCastException($"Cannot convert from type { repository } to any of {typeof(IRepository<>)}, {typeof(IRepository<,>)}");

            return services;
        }

        /// <summary>
        /// Registering all IQueryHandler present in a givene namespace
        /// </summary>
        /// <param name="services"></param>
        /// <param name="queryHandlerNamespace"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWorkAllQueryHandelers<TAssemblyFromType>(this IServiceCollection services)
        {
            services.Scan(scan =>
                            scan
                             .FromAssemblyOf<TAssemblyFromType>()
                             .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                             .AsImplementedInterfaces()
                             .WithTransientLifetime()
                    );

            return services;
        }


        /// <summary>
        /// Add AudtiStamp Provider
        /// </summary>
        /// <typeparam name="TKey">Type of Primary Key</typeparam>
        /// <typeparam name="TAuditStampProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWorkEntityAuditHandler<TKey, TEntityAuditHandler>(this IServiceCollection services)
            where TKey : struct
            where TEntityAuditHandler : IEntityAuditHandler<TKey>
        {

            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(IEntityAuditHandler<TKey>));
            services.Remove(descriptorToRemove);
            var descriptor = new ServiceDescriptor(typeof(IEntityAuditHandler<TKey>), typeof(TEntityAuditHandler), ServiceLifetime.Transient);
            services.Add(descriptor);
            return services;

        }


        /// <summary>
        /// Add SoftDelete Provider
        /// </summary>
        /// <typeparam name="TISoftDeleteProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddUnitOfWorkSoftDeleteProvider<TISoftDeleteProvider>(this IServiceCollection services)
            where TISoftDeleteProvider : ISoftDeleteProvider
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(ISoftDeleteProvider));
            services.Remove(descriptorToRemove);
            var descriptor = new ServiceDescriptor(typeof(ISoftDeleteProvider), typeof(TISoftDeleteProvider), ServiceLifetime.Transient);
            services.Add(descriptor);
            return services;
        }
    }
}
