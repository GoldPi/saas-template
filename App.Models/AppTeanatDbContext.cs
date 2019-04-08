using App.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Models
{
    public class AppTenantDbContext : AppDbContext<AppTenantUser,AppTenantRole>
    {
        TeanantConfiguration _teanantConfiguration;
        ITenant tenant;
        public AppTenantDbContext(DbContextOptions dBContextOptions, TenantContext<ITenant> resolver, TeanantConfiguration confuring):base(dBContextOptions)
        {
            _teanantConfiguration = confuring;
            if (resolver != null)
            {
                this.tenant = resolver.Tenant;
                Database.EnsureCreated();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _teanantConfiguration._confuring.Invoke(optionsBuilder,tenant);
        }
    }

    public sealed class TeanantConfiguration
    {
        public Action<DbContextOptionsBuilder, ITenant> _confuring;
        
    } 
}
