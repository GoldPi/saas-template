using System;
using System.Collections.Generic;
using System.Text;

namespace App.Infrastructure
{
    public interface ITenant
    {
        string Id { get; set; }
       
        string Name { get; set; }

        string Hostname { get; set; }

        string ConnectionString { get; set; }

        string[] Hostnames { get;  }
    }
}
