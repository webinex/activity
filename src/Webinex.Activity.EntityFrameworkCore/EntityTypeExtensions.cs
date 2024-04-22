using Microsoft.EntityFrameworkCore.Metadata;

namespace Webinex.Activity.EntityFrameworkCore;

internal static class EntityTypeExtensions
{
    public static bool IsActivityEnabled(this IEntityType entityType)
    {
        return entityType.FindAnnotation("Activity__Enabled")?.Value as bool? == true;
    }
}