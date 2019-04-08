using App.Master.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Tenant.Infrastucture
{
    public class TenantResolverUsingDataBase : ITenantResolver<MasterTenent>
    {
        private TenantMasterDbContext db;
        private MasterTenent tenant;

        // private AppTenant Current;
        public  TenantResolverUsingDataBase(TenantMasterDbContext backEndContext)
        {
            db = backEndContext;
        }

        public async Task<TenantContext<MasterTenent>> ResolveAsync(HttpContext context)
        {
            TenantContext<MasterTenent> tenantContext = null;
#if DEBUG
            Console.WriteLine("-->" + context.Request.Host.ToString());
#endif
            tenant = await db.Tenants.FirstOrDefaultAsync(t =>
                 t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));

            if (tenant != null)
            {
                Console.WriteLine(tenant.Name + ":" + tenant.Hostnames[0]);
                tenantContext = new TenantContext<MasterTenent>(tenant);
            }
            else
            {
                tenant = new MasterTenent { Id = "00", Hostname = "34.73.160.251", Name = "ADMIN TENANT" };
#if DEBUG
                Console.WriteLine(tenant.Name + ":" + tenant.Hostnames[0]);
#endif

                tenantContext = new TenantContext<MasterTenent>(tenant);
            }

            Console.WriteLine(context.Request.Path);
            return tenantContext;
        }
    }
}
