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

        public DbSet<TrafficRoute> TrafficRoutes { get; set; }
        public DbSet<RouteEvent> RouteEvents { get; set; }
        public DbSet<TrafficRouteRouteEvent> TrafficRouteRouteEvents { get; set; }


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
                entity.Property(e => e.Id).HasMaxLength(36);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });
            modelBuilder.Entity<TrafficRouteRouteEvent>(entity =>
            {
                entity.HasKey(e => new { e.TrafficRouteId, e.RouteEventId });
                entity.HasOne(e => e.TrafficRoute)
                    .WithMany()
                    .HasForeignKey(e => e.TrafficRouteId);
                entity.HasOne(e => e.RouteEvent)
                    .WithMany()
                    .HasForeignKey(e => e.RouteEventId);
                entity.Property(e => e.Name).HasMaxLength(200);
            });
            //modelBuilder.Ignore<TrafficRoute>();
            //modelBuilder.Ignore<RouteEvent>();
        }
    }
}
