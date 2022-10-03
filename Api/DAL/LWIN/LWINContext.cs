using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSwamp.Api.DAL.LWIN.Entities;

namespace TheSwamp.Api.DAL.LWIN
{
    public class LWINContext : DbContext
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

        public async Task<LWINRaw> GetRandomWineAsync()
        {
            await Task.CompletedTask;
            var x = Raw.FromSqlRaw("exec [lwin].[GetRandomWine]").AsEnumerable().Single();
            return x;
        }
    }
}
