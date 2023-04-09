using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.EfCore
{
    public class ActivityDbContext : DbContext
    {
        private readonly ActivityDbContextSettings _dbContextSettings;

        public DbSet<ActivityRow> Activities { get; set; }

        public DbSet<ActivityValueRow> ActivityValues { get; set; }

        public ActivityDbContext(ActivityDbContextSettings dbContextSettings)
            : base(dbContextSettings.Options)
        {
            _dbContextSettings = dbContextSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityRow>(entity =>
            {
                entity.ToTable(_dbContextSettings.ActivityTableName, _dbContextSettings.Schema);
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Uid).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Kind).HasMaxLength(50).IsRequired();
                entity.Property(x => x.OperationUid).HasMaxLength(50).IsRequired();
                entity.Property(x => x.ParentUid).HasMaxLength(50).IsRequired(false);
                entity.HasMany(x => x.Values).WithOne(x => x.Activity).HasForeignKey(x => x.ActivityId);
            });

            modelBuilder.Entity<ActivityValueRow>(entity =>
            {
                entity.ToTable(_dbContextSettings.ActivityValueTableName, _dbContextSettings.Schema);
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Path).HasMaxLength(4000).IsRequired();
                entity.Property(x => x.SearchPath).HasMaxLength(4000).IsRequired();
                entity.Property(x => x.Kind).HasMaxLength(250).IsRequired();
                entity.Property(x => x.Value).IsRequired();

                entity
                    .HasOne(x => x.Activity)
                    .WithMany(x => x.Values)
                    .HasForeignKey(x => x.ActivityId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}