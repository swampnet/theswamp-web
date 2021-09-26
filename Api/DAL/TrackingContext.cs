using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using TheSwamp.Api.DAL.Entities;

namespace TheSwamp.Api.DAL
{
    public class TrackingContext : DbContext
    {
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<DataPoint> DataPoints { get; set; }

        public TrackingContext(DbContextOptions<TrackingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("trk");

            modelBuilder.Entity<DataSource>().ToTable("DataSource");
            modelBuilder.Entity<DataPoint>().ToTable("DataPoint");
            base.OnModelCreating(modelBuilder);
        }
    }
}
