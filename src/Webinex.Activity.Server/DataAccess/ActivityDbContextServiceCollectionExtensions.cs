using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Activity.Server.DataAccess;

public static class DbContextActivityWorkerConfigurationExtensions
{
    /// <summary>
    /// You can use this method to configure custom DbContext.
    /// Model definition for <see cref="ActivityRowBase"/> you can find in <see cref="ActivityModelBuilderExtensions.AddActivityRowEntity{TActivity}"/>
    /// </summary>
    public static IActivityServerDbContextConfiguration AddDbContext<TContext>(
        this IActivityServerConfiguration configuration)
        where TContext : class
    {
        if (!typeof(TContext).IsAssignableTo(typeof(IActivityDbContext<>).MakeGenericType(configuration.RowType)))
            throw new InvalidOperationException(
                $"{nameof(TContext)} might be assignable to {nameof(IActivityDbContext<ActivityRowBase>)}<{configuration.RowType.Name}>)");

        configuration.Services.TryAddScoped<TContext>();
        configuration.Services.TryAddScoped(typeof(IActivityDbContext<>).MakeGenericType(configuration.RowType),
            p => p.GetRequiredService<TContext>());

        return new ActivityServerDbContextConfiguration(configuration.Services, configuration.RowType, typeof(TContext));
    }

    public static IActivityServerDbContextConfiguration AddDbContext(
        this IActivityServerConfiguration configuration,
        string schema,
        string activityTableName,
        string activityValueTableName,
        Action<DbContextOptionsBuilder> configureOptions)
    {
        return AddDbContext(
            configuration,
            configureOptions,
            schema,
            activityTableName,
            activityValueTableName);
    }

    public static IActivityServerDbContextConfiguration AddDbContext(
        this IActivityServerConfiguration configuration,
        Action<DbContextOptionsBuilder> configureOptions)
    {
        return AddDbContext(
            configuration,
            configureOptions,
            ActivityDbContextDefaults.SCHEMA,
            ActivityDbContextDefaults.ACTIVITY_TABLE_NAME,
            ActivityDbContextDefaults.ACTIVITY_VALUE_TABLE_NAME);
    }

    private static IActivityServerDbContextConfiguration AddDbContext(
        this IActivityServerConfiguration configuration,
        Action<DbContextOptionsBuilder> configureOptions,
        string activitySchema,
        string activityTableName,
        string activityValueTableName)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        activitySchema = activitySchema ?? throw new ArgumentNullException(nameof(activitySchema));
        activityTableName = activityTableName ?? throw new ArgumentNullException(nameof(activityTableName));
        activityValueTableName =
            activityValueTableName ?? throw new ArgumentNullException(nameof(activityValueTableName));

        var optionsBuilder = new DbContextOptionsBuilder();
        configureOptions(optionsBuilder);

        var options = optionsBuilder.Options;
        var dbContextSettings =
            new ActivityDbContextSettings(options, activitySchema, activityTableName, activityValueTableName);
        configuration.Services.TryAddSingleton(dbContextSettings);

        configuration.Services.TryAddScoped(typeof(ActivityDbContext<>).MakeGenericType(configuration.RowType));
        configuration.Services.TryAddScoped(typeof(IActivityDbContext<>).MakeGenericType(configuration.RowType),
            p => p.GetRequiredService(typeof(ActivityDbContext<>).MakeGenericType(configuration.RowType)));

        return new ActivityServerDbContextConfiguration(configuration.Services,
            configuration.RowType,
            typeof(ActivityDbContext<>).MakeGenericType(configuration.RowType));
    }
}