using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Webinex.Activity.EntityFrameworkCore.Utils;

internal static class PrimaryKeyUtil
{
    public static IDictionary<string, object?>? Get(EntityEntry entry)
    {
        var pk = entry.Metadata.FindPrimaryKey();
        if (pk == null) return null;

        var values = new Dictionary<string, object?>();
        foreach (var property in pk.Properties)
        {
            values[property.Name] = entry.Property(property.Name).CurrentValue;
        }

        return values;
    }
}