using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Infrastructure
{
    public class AppDbContext<Tu,Tr> : IdentityDbContext<Tu, Tr, string> where Tu : AppUser where Tr : AppRole
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }
    }
}
