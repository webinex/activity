using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Webinex.Activity.EntityFrameworkCore.Utils;

internal static class FindOwnerUtil
{
    public static EntityEntry? FindEntity(EntityEntry entry, EntityState state)
    {
        if (!entry.Metadata.IsOwned())
        {
            return null;
        }

        var ownership = entry.Metadata.FindOwnership();
        if (ownership == null)
        {
            return null;
        }

        var foreignKeyValues = ownership.Properties.Select(entry.Property).Select(x => x.CurrentValue).ToArray();

        IEnumerable<object?> PrincipalKeyValuesSelector(EntityEntry x) =>
            ownership.PrincipalKey.Properties.Select(prop => x.Property(prop).CurrentValue);

        bool IsPrincipalKeyMatch(EntityEntry x) => PrincipalKeyValuesSelector(x).SequenceEqual(foreignKeyValues);

        return entry.Context.ChangeTracker
            .Entries()
            .FirstOrDefault(x => x.Metadata == ownership.PrincipalEntityType && IsPrincipalKeyMatch(x) && x.State == state);
    }
}