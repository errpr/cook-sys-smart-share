using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server
{
    public class SmartShareContext : DbContext
    {
        public DbSet<UploadedFile> UploadedFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("server=127.0.0.1;port=5432;database=smartshare;userid=postgres;password=bondstone");
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<UploadedFile>(entity => { entity.HasIndex(e => e.Name).IsUnique(); });
        }
    }
}