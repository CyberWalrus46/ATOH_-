using AtonTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AtonTask.Infrastucture.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Login).IsUnique();
                entity.Property(u => u.Gender).HasConversion<int>();
            });
            
        }

        public DbSet<User> Users { get; set; }
    }
}
