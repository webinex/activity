using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Webinex.Activity.EntityFrameworkCore.Utils;

namespace Webinex.Activity.EntityFrameworkCore;

internal static class EntityEntryExtensions
{
    public static bool IsModified(this EntityEntry entry)
    {
        return entry.State != EntityState.Detached && entry.State != EntityState.Unchanged;
    }

    public static IDictionary<string, object?>? PrimaryKey(this EntityEntry entry)
    {
        return PrimaryKeyUtil.Get(entry);
    }

    public static IDictionary<string, object?> Values(this EntityEntry entry, bool includeReferences = true)
    {
        return CurrentValuesUtil.Get(entry, includeReferences);
    }

    public static IDictionary<string, object?>? OriginalValues(this EntityEntry entry, bool includeReferences = true)
    {
        return OriginalValuesUtil.Get(entry, includeReferences);
    }

    public static EntityEntry? FindUnchangedOwner(this EntityEntry entry)
    {
        return FindOwnerUtil.FindEntity(entry, EntityState.Unchanged);
    }
}