using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Webinex.Activity.EntityFrameworkCore;

public class ActivitySaveChangesInterceptor<TSubscriber> : SaveChangesInterceptor
    where TSubscriber : IActivitySaveChangesSubscriber
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        var context = eventData.Context;
        if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var logger = context.GetService<ILogger<ActivitySaveChangesInterceptor<TSubscriber>>>();
        IActivitySaveChangesSubscriber subscriber;

        try
        {
            subscriber = (IActivitySaveChangesSubscriber)context.GetService(typeof(TSubscriber)) ??
                         throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to resolve subscriber");
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        try
        {
            var changes = new EntityChangeSet(context).Changes;
            if (changes.Any())
                await subscriber.ProcessAsync(context, new EntityChangeSet(context).Changes, cancellationToken);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to process changes");
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private class EntityChangeSet
    {
        private readonly List<EntityChange> _changes = new();
        private readonly Lazy<EntityEntry[]> _entriesLazy;

        public IEnumerable<EntityChange> Changes => _changes.ToArray();

        public EntityChangeSet(DbContext dbContext)
        {
            _entriesLazy = new Lazy<EntityEntry[]>(() => dbContext.ChangeTracker.Entries().ToArray());
            Collect();
        }

        private void Collect()
        {
            var added = NonOwnedAddedEntries.Select(x => Map(x, EntityChangeType.Added)).ToArray();
            var modified = NonOwnedModifiedEntries.Select(x => Map(x, EntityChangeType.Updated)).ToArray();
            var deleted = NonOwnedDeletedEntries.Select(x => Map(x, EntityChangeType.Deleted)).ToArray();
            var modifiedOwners =
                UnchangedOwnerWithModifiedOwned.Select(x => Map(x, EntityChangeType.Updated)).ToArray();
            _changes.AddRange(added.Concat(modified).Concat(modifiedOwners).Concat(deleted));
        }

        private EntityChange Map(EntityEntry entry, EntityChangeType type) =>
            new(type, entry.Metadata.ClrType.Name, entry.PrimaryKey(),
                type == EntityChangeType.Deleted ? entry.OriginalValues()! : entry.Values(),
                type == EntityChangeType.Updated ? entry.OriginalValues() : null);

        private EntityEntry[] NonOwnedAddedEntries =>
            NonOwnedEnabled.Where(x => x.State == EntityState.Added).ToArray();

        private EntityEntry[] NonOwnedModifiedEntries => NonOwnedEnabled
            .Where(x => x.State == EntityState.Modified).ToArray();

        private EntityEntry[] NonOwnedDeletedEntries =>
            NonOwnedEnabled.Where(x => x.State == EntityState.Deleted).ToArray();

        private EntityEntry[] NonOwned => _entriesLazy.Value.Where(x => !x.Metadata.IsOwned()).ToArray();
        private EntityEntry[] NonOwnedEnabled => NonOwned.Where(x => x.Metadata.IsActivityEnabled()).ToArray();

        private EntityEntry[] Owned => _entriesLazy.Value.Where(x => x.Metadata.IsOwned()).ToArray();

        private EntityEntry[] ChangedOwned => Owned
            .Where(x => x.State is EntityState.Modified or EntityState.Deleted or EntityState.Added)
            .ToArray();

        private EntityEntry[] NonOwnedWithChangedOwned => ChangedOwned
            .Select(x => x.FindUnchangedOwner())
            .Where(x => x != null)
            .Cast<EntityEntry>()
            .Distinct()
            .ToArray();

        private EntityEntry[] UnchangedOwnerWithModifiedOwned => NonOwnedWithChangedOwned
            .Where(x => x.Metadata.IsActivityEnabled())
            .DistinctBy(x => x.Entity)
            .ToArray();
    }
}