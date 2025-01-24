using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Webinex.Activity.Server.DataAccess;

public static class ActivityModelBuilderExtensions
{
    private static readonly Lazy<JsonSerializerOptions> JsonSerializerOptions = new(() => new JsonSerializerOptions());

    public static ModelBuilder AddActivityRowEntity<TActivityRow>(
        this ModelBuilder modelBuilder,
        Action<EntityTypeBuilder<TActivityRow>> configure)
        where TActivityRow : ActivityRowBase
    {
        modelBuilder.Entity<TActivityRow>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Uid).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Kind).HasMaxLength(50).IsRequired();
            entity.Property(x => x.OperationUid).HasMaxLength(50).IsRequired();
            entity.Property(x => x.TenantId).HasMaxLength(50).IsRequired(false);
            entity.Property(x => x.UserId).HasMaxLength(50).IsRequired(false);
            entity.Property(x => x.Success).IsRequired();
            entity.Property(x => x.PerformedAt).IsRequired();
            entity.Property(x => x.ParentUid).HasMaxLength(50).IsRequired(false);
            entity.Property(x => x.System).IsRequired();

            if (typeof(TActivityRow) == typeof(ActivityRow))
            {
                var converter = new ValueConverter<JsonElement, string>(
                    convertFromProviderExpression: @string => JsonSerializer.Deserialize<JsonElement>(@string, JsonSerializerOptions.Value)!,
                    convertToProviderExpression: jElement => jElement.ToString());
            
                entity.Property(nameof(ActivityRow.Values)).HasConversion(converter);
            }

            configure(entity);
        });

        return modelBuilder;
    }
}