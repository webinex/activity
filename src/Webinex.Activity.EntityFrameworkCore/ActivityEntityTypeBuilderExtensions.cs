using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Webinex.Activity.EntityFrameworkCore;

public static class ActivityEntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> UseActivity<T>(this EntityTypeBuilder<T> model, bool enabled)
        where T : class
    {
        model.HasAnnotation("Activity__Enabled", enabled);
        return model;
    }
}