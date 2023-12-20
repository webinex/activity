using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.EfCore;

internal class ActivityDbContext : DbContext, IActivityDbContext
{
    private readonly ActivityDbContextSettings _dbContextSettings;

    public DbSet<ActivityRow> Activities { get; init; } = null!;
    public DbSet<ActivityValueRow> ActivityValues { get; init; } = null!;

    public ActivityDbContext(ActivityDbContextSettings dbContextSettings)
        : base(dbContextSettings.Options)
    {
        _dbContextSettings = dbContextSettings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .AddActivityRowEntity(entity => entity
                .ToTable(_dbContextSettings.ActivityTableName, _dbContextSettings.Schema))
            .AddActivityValueRowEntity(entity => entity
                .ToTable(_dbContextSettings.ActivityValueTableName, _dbContextSettings.Schema));

        base.OnModelCreating(modelBuilder);
    }

    Task IActivityDbContext.SaveChangesAsync(CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);
}