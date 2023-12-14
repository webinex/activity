using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Activity.Server.EfCore;

public static class DbContextActivityWorkerConfigurationExtensions
{
    /// <summary>
    /// You can use this method to configure custom DbContext.
    /// Model definition for <see cref="ActivityRow"/> you can find in <see cref="ActivityModelBuilderExtensions.AddActivityRowEntity"/>
    /// Model definition for <see cref="ActivityValueRow"/> you can find in <see cref="ActivityModelBuilderExtensions.AddActivityValueRowEntity"/>
    /// </summary>
    public static IActivityServerDbContextConfiguration AddDbContext<TContext>(
        this IActivityServerConfiguration configuration)
        where TContext : class, IActivityDbContext
    {
        configuration.Services.TryAddScoped<TContext>();
        configuration.Services.TryAddScoped<IActivityDbContext>(e => e.GetRequiredService<TContext>());

        return new ActivityServerDbContextConfiguration(configuration.Services, typeof(TContext));
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

        configuration.Services.TryAddScoped<ActivityDbContext>();
        configuration.Services.TryAddScoped<IActivityDbContext>(sp => sp.GetRequiredService<ActivityDbContext>());

        return new ActivityServerDbContextConfiguration(configuration.Services, typeof(ActivityDbContext));
    }
}