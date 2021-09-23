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
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceValue> DeviceValues { get; set; }

        public TrackingContext(DbContextOptions<TrackingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("trk");

            modelBuilder.Entity<Device>().ToTable("Device");
            modelBuilder.Entity<DeviceValue>().ToTable("DeviceValue");
            base.OnModelCreating(modelBuilder);
        }
    }
}
