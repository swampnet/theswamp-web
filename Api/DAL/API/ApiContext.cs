using Microsoft.EntityFrameworkCore;
using TheSwamp.Api.DAL.API.Entities;

namespace TheSwamp.Api.DAL.API
{
    public class ApiContext : DbContext
    {
        public DbSet<KeyEntity> Keys { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("api");
            modelBuilder.Entity<KeyEntity>().ToTable("key");

            base.OnModelCreating(modelBuilder);
        }
    }
}
