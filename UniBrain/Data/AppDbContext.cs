using Microsoft.EntityFrameworkCore;
using UniBrain.Models;

namespace UniBrain.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ClassSession> Sessions { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Egyedi index a tárgykódra, hogy ne duplikáljuk
            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.Code)
                .IsUnique();
        }
    }
}
