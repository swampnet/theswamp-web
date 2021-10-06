using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TheSwamp.Api.DAL.IOT.Entities;

namespace TheSwamp.Api.DAL.IOT
{
    public class IotContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public IotContext(DbContextOptions<IotContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("iot");
            
            modelBuilder.Entity<Message>()
                .ToTable("Message")
                .HasMany(f => f.Properties)
                .WithOne(f => f.Message)
                .HasForeignKey(f => f.MessageId);
            
            modelBuilder.Entity<MessageProperty>()
                .ToTable("MessageProperty");

            base.OnModelCreating(modelBuilder);
        }
    }
}
