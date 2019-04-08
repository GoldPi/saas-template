using App.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Master.Models
{
    public class TenantRole: AppRole
    {
        public TenantRole()
        {

        }
        public TenantRole(string rolename,string description):base(rolename,description)
        {

        }
    }
}
