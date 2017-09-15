using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolinders.Web.Models;

namespace Bolinders.Web.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }
        public DbSet<Vehicle> Vehicles {  get; set; }
        public DbSet<Make> Make { get; set; }
        public DbSet<Facility> Facilites { get; set; }
    }
}
