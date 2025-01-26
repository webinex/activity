using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.Server.Worker.DataAccess;
using Webinex.Activity.Server.Worker.DataAccess.EntityFrameworkCore;

namespace Webinex.Activity.Server.Worker;

public interface IActivityWorkerConfiguration
{
    IActivityServerConfiguration ServerConfiguration { get; }
    IDictionary<string, object> Values { get; }
}

internal class ActivityWorkerConfiguration : IActivityWorkerConfiguration
{
    public ActivityWorkerConfiguration(IActivityServerConfiguration serverConfiguration)
    {
        ServerConfiguration = serverConfiguration ?? throw new ArgumentNullException(nameof(serverConfiguration));
        ServerConfiguration.Services.AddScoped<IActivityWorkerService, ActivityWorkerService>();
    }

    public IActivityServerConfiguration ServerConfiguration { get; }

    public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();
}

public static class ActivityWorkerConfigurationExtensions
{
    public static IActivityWorkerConfiguration UseDbContext(
        this IActivityWorkerConfiguration configuration)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        configuration.ServerConfiguration.Services.AddScoped(typeof(IActivityStore),
            typeof(DbContextActivityStore<>).MakeGenericType(configuration.ServerConfiguration.RowType));
        return configuration;
    }
}