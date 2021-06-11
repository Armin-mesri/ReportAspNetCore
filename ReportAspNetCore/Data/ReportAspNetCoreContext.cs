using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportAspNetCore.Models;

namespace ReportAspNetCore.Data
{
    public class ReportAspNetCoreContext : DbContext
    {
        public ReportAspNetCoreContext (DbContextOptions<ReportAspNetCoreContext> options)
            : base(options)
        {
        }

        public DbSet<ReportAspNetCore.Models.Person> Person { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
