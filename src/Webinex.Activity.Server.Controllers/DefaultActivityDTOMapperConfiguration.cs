using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.Server.Controllers;

public interface IDefaultActivityDTOMapperSettings<TActivityRow>
{
    Func<IServiceProvider, IReadOnlyCollection<TActivityRow>,
        Task<IReadOnlyDictionary<string, IDictionary<string, object?>>>>? Extra { get; }
}

public class DefaultActivityDTOMapperConfiguration<TActivityRow> : IDefaultActivityDTOMapperSettings<TActivityRow>
{
    public IServiceCollection Services { get; }

    private DefaultActivityDTOMapperConfiguration(IServiceCollection services)
    {
        Services = services;

        services.AddSingleton(this);
        services.AddSingleton<IDefaultActivityDTOMapperSettings<TActivityRow>>(this);
    }

    public Func<IServiceProvider, IReadOnlyCollection<TActivityRow>,
        Task<IReadOnlyDictionary<string, IDictionary<string, object?>>>>? Extra { get; private set; }

    public DefaultActivityDTOMapperConfiguration<TActivityRow> UseExtra(
        Func<IServiceProvider, IReadOnlyCollection<TActivityRow>,
            Task<IReadOnlyDictionary<string, IDictionary<string, object?>>>> extra)
    {
        Extra = extra;
        return this;
    }

    internal static DefaultActivityDTOMapperConfiguration<TActivityRow> GetOrCreate(IServiceCollection services)
    {
        var instance = (DefaultActivityDTOMapperConfiguration<TActivityRow>?)services
            .FirstOrDefault(x => x.ServiceType == typeof(DefaultActivityDTOMapperConfiguration<TActivityRow>))
            ?.ImplementationInstance;

        return instance ?? new DefaultActivityDTOMapperConfiguration<TActivityRow>(services);
    }
}