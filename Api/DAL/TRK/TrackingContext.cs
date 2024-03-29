﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using TheSwamp.Api.DAL.TRK.Entities;

namespace TheSwamp.Api.DAL.TRK
{
    public class TrackingContext : DbContext
    {
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<DataPoint> DataPoints { get; set; }
        public DbSet<DataSourceEvent> Events { get; set; }


        public TrackingContext(DbContextOptions<TrackingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("trk");

            modelBuilder.Entity<DataSource>().ToTable("DataSource");
            modelBuilder.Entity<DataSourceEvent>().ToTable("DataSourceEvent");
            modelBuilder.Entity<DataPoint>().ToTable("DataPoint");
            modelBuilder.Entity<DataSourceProcessor>().ToTable("DataSourceProcessor");
            modelBuilder.Entity<DataSourceProcessorParameter>().ToTable("DataSourceProcessorParameter");
            base.OnModelCreating(modelBuilder);
        }
    }
}
