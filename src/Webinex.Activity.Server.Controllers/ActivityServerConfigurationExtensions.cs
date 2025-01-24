using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server.Controllers;

public static class ActivityServerConfigurationExtensions
{
    public static IActivityServerConfiguration<TActivityRow> AddEndpoints<TActivityRow>(
        this IActivityServerConfiguration<TActivityRow> configuration)
        where TActivityRow : ActivityRow
    {
        configuration.Services.TryAddScoped<IActivityReadService<TActivityRow>, ActivityReadService<TActivityRow>>();
        configuration.Services.TryAddTransient<IActivityDTOMapper<TActivityRow>, DefaultActivityDTOMapper<TActivityRow>>();
        DefaultActivityDTOMapperConfiguration<TActivityRow>.GetOrCreate(configuration.Services);
        return configuration;
    }

    public static IActivityServerConfiguration<TActivityRow> ConfigureDefaultMapper<TActivityRow>(
        this IActivityServerConfiguration<TActivityRow> configuration,
        Action<DefaultActivityDTOMapperConfiguration<TActivityRow>> configure)
    {
        var mapperConfiguration =
            DefaultActivityDTOMapperConfiguration<TActivityRow>.GetOrCreate(configuration.Services);
        configure(mapperConfiguration);
        return configuration;
    }
}