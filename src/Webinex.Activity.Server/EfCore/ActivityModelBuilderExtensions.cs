using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Webinex.Activity.Server.EfCore;

public static class ActivityModelBuilderExtensions
{
    public static ModelBuilder AddActivityRowEntity(
        this ModelBuilder modelBuilder,
        Action<EntityTypeBuilder<ActivityRow>> configure)
    {
        modelBuilder.Entity<ActivityRow>(entity =>
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
            entity.HasMany(x => x.Values).WithOne(x => x.Activity).HasForeignKey(x => x.ActivityId);

            configure(entity);
        });

        return modelBuilder;
    }

    public static ModelBuilder AddActivityValueRowEntity(
        this ModelBuilder modelBuilder,
        Action<EntityTypeBuilder<ActivityValueRow>> configure)
    {
        modelBuilder.Entity<ActivityValueRow>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(e => e.ActivityId).IsRequired();
            entity.Property(x => x.Path).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.SearchPath).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.Kind).IsRequired();
            entity.Property(x => x.Value).IsRequired();

            entity
                .HasOne(x => x.Activity)
                .WithMany(x => x.Values)
                .HasForeignKey(x => x.ActivityId);

            configure(entity);
        });

        return modelBuilder;
    }
}