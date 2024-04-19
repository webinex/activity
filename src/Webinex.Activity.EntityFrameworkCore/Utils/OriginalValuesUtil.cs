using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Webinex.Activity.EntityFrameworkCore.Utils;

internal static class OriginalValuesUtil
{
    public static IDictionary<string, object?>? Get(EntityEntry entry, bool includeReferences)
    {
        if (!entry.Metadata.IsOwned() && entry.State == EntityState.Added)
            return null;

        var byProperties = ByProperties(entry);
        var result = new List<KeyValuePair<string, object?>>(byProperties);

        if (includeReferences)
        {
            result.AddRange(ByReferences(entry, includeReferences));
            result.AddRange(ByCollections(entry, includeReferences));
        }

        return new Dictionary<string, object?>(result);
    }

    private static KeyValuePair<string, object?>[] ByProperties(EntityEntry entry)
    {
        return entry.Properties.Where(x => !x.Metadata.IsShadowProperty())
            .Select(x => KeyValuePair.Create(x.Metadata.Name, x.OriginalValue))
            .ToArray();
    }

    private static KeyValuePair<string, object?>[] ByReferences(EntityEntry entry, bool includeReferences)
    {
        var result = new List<KeyValuePair<string, object?>>();

        foreach (var reference in entry.References.Where(x => !x.Metadata.IsShadowProperty()).ToArray())
        {
            bool IsKeyMatch(EntityEntry dependent)
            {
                var ownership = reference.Metadata.TargetEntityType.FindOwnership();
                if (ownership == null) return false;

                return ownership.Properties.Select(prop => dependent.Property(prop).CurrentValue).SequenceEqual(
                    ownership.PrincipalKey.Properties.Select(prop => entry.Property(prop).CurrentValue));
            }

            EntityEntry? FindEntry(EntityState state)
            {
                return entry.Context.ChangeTracker.Entries().FirstOrDefault(x =>
                    x.State == state
                    && x.Metadata == reference!.Metadata.TargetEntityType
                    && IsKeyMatch(x));
            }

            var originalEntry = FindEntry(EntityState.Deleted) ??
                                FindEntry(EntityState.Modified) ?? FindEntry(EntityState.Unchanged);

            result.Add(KeyValuePair.Create<string, object?>(reference.Metadata.Name,
                originalEntry != null ? Get(originalEntry, includeReferences) : null));
        }

        return result.ToArray();
    }

    private static KeyValuePair<string, object?>[] ByCollections(EntityEntry entry, bool includeReferences)
    {
        var result = new List<KeyValuePair<string, object?>>();

        foreach (var collectionEntry in entry.Collections.Where(x =>
                     !x.Metadata.IsShadowProperty() && x.Metadata.TargetEntityType.IsOwned()))
        {
            bool IsKeyMatch(EntityEntry dependent)
            {
                var ownership = collectionEntry!.Metadata.TargetEntityType.FindOwnership();
                if (ownership == null) return false;

                var principalKeyValues = ownership.PrincipalKey.Properties
                    .Select(prop => entry.Property(prop).CurrentValue).ToArray();
                var foreignKeyValues = ownership.Properties.Select(prop => dependent.Property(prop).CurrentValue)
                    .ToArray();
                return principalKeyValues.SequenceEqual(foreignKeyValues);
            }

            var entries = entry.Context.ChangeTracker.Entries()
                .Where(x => x.Metadata == collectionEntry.Metadata.TargetEntityType)
                .Where(IsKeyMatch)
                .Where(x => x.State is EntityState.Deleted or EntityState.Modified or EntityState.Unchanged)
                .ToArray();

            result.Add(KeyValuePair.Create<string, object?>(collectionEntry.Metadata.Name,
                entries.Select(x => Get(x, includeReferences)).ToArray()));
        }

        return result.ToArray();
    }
}