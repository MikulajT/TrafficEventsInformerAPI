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
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }

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
                    .WithMany(e => e.TrafficRouteRouteEvents)
                    .HasForeignKey(e => e.TrafficRouteId);
                entity.HasOne(e => e.RouteEvent)
                    .WithMany(e => e.TrafficRouteRouteEvents)
                    .HasForeignKey(e => e.RouteEventId);
                entity.Property(e => e.Name).HasMaxLength(200);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });
            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.FcmToken);
            });
            //modelBuilder.Ignore<TrafficRoute>();
            //modelBuilder.Ignore<RouteEvent>();
            //modelBuilder.Ignore<TrafficRouteRouteEvents>();
            //modelBuilder.Ignore<Users>();
            //modelBuilder.Ignore<Devices>();
        }
    }
}
