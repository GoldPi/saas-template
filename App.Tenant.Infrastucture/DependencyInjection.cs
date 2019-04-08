using System;
using App.Master.Models;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Tenant.Infrastucture;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMultiTanancyUsingDataBase(this IServiceCollection service)
        {
            service.AddMultitenancy<MasterTenent, TenantResolverUsingDataBase>();
            return service;
        }

        public static IServiceCollection AddTenantMasterSqlDataBase(this IServiceCollection service,Action<DbContextOptionsBuilder> options = null)
        {
            service.AddDbContext<TenantMasterDbContext>(options);
            return service;
        }
        public static IServiceCollection AddTenantSqlDataBase(this IServiceCollection service, Action<DbContextOptionsBuilder> options = null)
        {
            service.AddDbContext<TenantMasterDbContext>(options);
            return service;
        }

        public static IServiceCollection AddDataSqlDataBase(this IServiceCollection service, Action<DbContextOptionsBuilder> options = null)
        {
            service.AddDbContext<AppTenantDbContext>(options);
            return service;
        }

        public static IServiceCollection AddTenantConfiguration(this IServiceCollection service, TeanantConfiguration teanantConfiguration)
        {
            service.AddScoped(typeof(TeanantConfiguration),o=>teanantConfiguration );
            return service;
        }
    }
}
