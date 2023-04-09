using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Activity.Server.EfCore
{
    public static class DbContextActivityWorkerConfigurationExtensions
    {
        public static IActivityServerConfiguration AddDbContext(
            [NotNull] this IActivityServerConfiguration configuration,
            [NotNull] string schema,
            [NotNull] string activityTableName,
            [NotNull] string activityValueTableName,
            [NotNull] Action<DbContextOptionsBuilder> configureOptions)
        {
            return AddDbContext(
                configuration,
                configureOptions,
                schema,
                activityTableName,
                activityValueTableName);
        }

        public static IActivityServerConfiguration AddDbContext(
            [NotNull] this IActivityServerConfiguration configuration,
            [NotNull] Action<DbContextOptionsBuilder> configureOptions)
        {
            return AddDbContext(
                configuration,
                configureOptions,
                ActivityDbContextDefaults.SCHEMA,
                ActivityDbContextDefaults.ACTIVITY_TABLE_NAME,
                ActivityDbContextDefaults.ACTIVITY_VALUE_TABLE_NAME);
        }

        private static IActivityServerConfiguration AddDbContext(
            [NotNull] this IActivityServerConfiguration configuration,
            [NotNull] Action<DbContextOptionsBuilder> configureOptions,
            [NotNull] string activitySchema,
            [NotNull] string activityTableName,
            [NotNull] string activityValueTableName)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            activitySchema = activitySchema ?? throw new ArgumentNullException(nameof(activitySchema));
            activityTableName = activityTableName ?? throw new ArgumentNullException(nameof(activityTableName));
            activityValueTableName = activityValueTableName ?? throw new ArgumentNullException(nameof(activityValueTableName));

            var optionsBuilder = new DbContextOptionsBuilder();
            configureOptions(optionsBuilder);

            var options = optionsBuilder.Options;
            var dbContextSettings = new ActivityDbContextSettings(options, activitySchema, activityTableName, activityValueTableName);
            configuration.Services.TryAddSingleton(dbContextSettings);
            configuration.Services.TryAddScoped<ActivityDbContext>();
            configuration.Services.AddHostedService<ActivityDbFactory>();
            return configuration;
        }
    }
}