using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Webinex.Activity.EntityFrameworkCore.Utils;

internal static class CurrentValuesUtil
{
    public static IDictionary<string, object?> Get(EntityEntry entry, bool includeReferences)
    {
        var values = new Dictionary<string, object?>();

        foreach (var property in entry.Properties.Where(x => !x.Metadata.IsShadowProperty()))
        {
            values[property.Metadata.Name] = property.CurrentValue;
        }

        if (includeReferences)
        {
            foreach (var reference in entry.References.Where(x => !x.Metadata.IsShadowProperty()).ToArray())
            {
                values[reference.Metadata.Name] = reference.TargetEntry != null
                    ? reference.TargetEntry!.Values(includeReferences)
                    : null;
            }

            foreach (var collectionEntry in entry.Collections.Where(x =>
                         !x.Metadata.IsShadowProperty() && x.Metadata.TargetEntityType.IsOwned()))
            {
                var entries = new List<EntityEntry>();

                foreach (var value in collectionEntry.CurrentValue ?? Array.Empty<object>())
                {
                    entries.Add(entry.Context.Entry(value));
                }

                values[collectionEntry.Metadata.Name] = entries.Select(x => x.Values(includeReferences)).ToArray();
            }
        }

        return values;
    }
}