using App.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Master.Models
{
    public class TenantMasterDbContext : AppDbContext<TenantUser,TenantRole>
    {
        public TenantMasterDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<MasterTenent> Tenants { get; set; }
    }
}
