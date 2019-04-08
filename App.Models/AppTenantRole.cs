using App.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Models
{
    public class AppTenantRole :AppRole
    {
        public AppTenantRole()
        {

        }

        public AppTenantRole(string Role,string Description):base(Role,Description)
        {

        }
    }
}
