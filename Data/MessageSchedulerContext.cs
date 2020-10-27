using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageScheduler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace MessageScheduler.Data
{
    public class MessageSchedulerContext : DbContext
    {
        public MessageSchedulerContext(DbContextOptions<MessageSchedulerContext> options) : base(options)
        {

        }

        public DbSet<ScheduledText> ScheduledTexts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduledText>()
                .Property(s => s.PhoneNumber)
                .IsRequired(true);
            modelBuilder.Entity<ScheduledText>()
                .Property(s => s.DateUTC)
                .IsRequired(true);
            modelBuilder.Entity<ScheduledText>()
                .Property(s => s.Email)
                .IsRequired(true);
            modelBuilder.Entity<ScheduledText>()
                .Property(s => s.FirstName)
                .IsRequired(true);
            modelBuilder.Entity<ScheduledText>()
                .Property(s => s.LastName)
                .IsRequired(true);
            modelBuilder.Entity<ScheduledText>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ScheduledText>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<ScheduledText>()
                .ToTable("ScheduledTexts");
        }
    }
}
