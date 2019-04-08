using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Infrastructure
{
    public  class AppRole : IdentityRole
    {
        public AppRole()
        {

        }
        public AppRole(string roleName,string description):base(roleName)
        {
            Description = description;
        }

        public string Description { get; set; }
        public virtual ICollection<IdentityUserRole<string>> Users { get; set; }


        public virtual ICollection<IdentityRoleClaim<string>> Claims { get; set; }
    }
}
