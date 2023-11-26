using Microsoft.EntityFrameworkCore;
using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Ef
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrafficRoute> TrafficRoute { get; set; }
        public DbSet<RouteEvent> RouteEvent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrafficRoute>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Coordinates).HasColumnType("xml");
            });
            modelBuilder.Entity<RouteEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasOne(b => b.TrafficRoute)
                    .WithMany(a => a.Events)
                    .HasForeignKey(b => b.RouteId)
                    .IsRequired();
            });
        }
    }
}
