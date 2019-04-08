using App.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Master.Models
{
    public class MasterTenent : ITenant
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Hostname { get; set; }

        public string ConnectionString { get; set; }


        public string[] Hostnames
        {
            get
            {
                return Hostname?.Split('|');
            }
        }

        
    }
}
