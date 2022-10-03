using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLWIN.DAL
{
    internal class LWINContext : DbContext
    {
        public DbSet<LWINRaw> Raw { get; set; }

        public LWINContext(DbContextOptions<LWINContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("lwin");
            modelBuilder.Entity<LWINRaw>().ToTable("Raw").HasKey("LWIN");

            base.OnModelCreating(modelBuilder);
        }
    }
}
