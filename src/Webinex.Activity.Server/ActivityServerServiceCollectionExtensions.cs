using System;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server;

public static class ActivityServerServiceCollectionExtensions
{
    public static IServiceCollection AddActivityServer(
        this IServiceCollection services,
        Action<IActivityServerConfiguration<ActivityRow>> configure)
    {
        return services.AddActivityServer<ActivityRow>(configure);
    }

    public static IServiceCollection AddActivityServer<TActivityRow>(
        this IServiceCollection services,
        Action<IActivityServerConfiguration<TActivityRow>> configure)
        where TActivityRow : ActivityRow
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        configure = configure ?? throw new ArgumentNullException(nameof(configure));

        var configuration = ActivityServerConfiguration<TActivityRow>.GetOrCreate(services);
        configure(configuration);

        return services;
    }
}