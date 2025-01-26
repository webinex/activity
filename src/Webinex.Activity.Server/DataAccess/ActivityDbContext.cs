using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.DataAccess;

internal class ActivityDbContext<TActivityRow> : DbContext, IActivityDbContext<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    private readonly ActivityDbContextSettings _dbContextSettings;

    public DbSet<TActivityRow> Activities { get; init; } = null!;

    public ActivityDbContext(ActivityDbContextSettings dbContextSettings)
        : base(dbContextSettings.Options)
    {
        _dbContextSettings = dbContextSettings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .AddActivityRowEntity<TActivityRow>(entity => entity
                .ToTable(_dbContextSettings.ActivityTableName, _dbContextSettings.Schema));

        base.OnModelCreating(modelBuilder);
    }

    Task IActivityDbContext<TActivityRow>.SaveChangesAsync(CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);
}