using GOTHelperEng.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GOTHelperEng.Data
{
    public class GOTDatabaseContext : IdentityDbContext<User>
    {
        public GOTDatabaseContext(DbContextOptions<GOTDatabaseContext> options) : base(options)
        {
        }
        public DbSet<Administrator>? Administrators { get; set; }
        public DbSet<Leader>? Leaders { get; set; }
        public DbSet<Tourist>? Tourists { get; set; }
        public DbSet<Booklet>? Booklets { get; set; }
        public DbSet<Gender>? Genders { get; set; }
        public DbSet<Point>? Points { get; set; }
        public DbSet<Level>? Levels { get; set; }
        public DbSet<MountainArea>? MountainAreas { get; set; }
        public DbSet<TripApplication>? TripApplications { get; set; }
        public DbSet<StageShutdown>? StageShutdowns { get; set; }
        public DbSet<Stage>? Stage { get; set; }
        public DbSet<Position>? Position { get; set; }
        public DbSet<Trip>? Trips { get; set; }
        public DbSet<MountainRange>? MountainRanges { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
                entity.HasNoDiscriminator());
            modelBuilder.Entity<Stage>(entity =>
                entity.HasCheckConstraint("CK_ST_DI", "[StartPointId] <> [EndPointId]"));
            modelBuilder.Entity<Stage>()
                                 .HasOne(o => o.StartPoint)
                                 .WithMany(p => p.StagesOpening)
                                 .HasForeignKey(m => m.StartPointId)
                                 .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Stage>()
                                 .HasOne(o => o.EndPoint)
                                 .WithMany(p => p.StagesClosing)
                                 .HasForeignKey(m => m.EndPointId)
                                 .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
