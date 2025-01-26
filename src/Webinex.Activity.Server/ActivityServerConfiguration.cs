using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Activity.Server.DataAccess;
using Webinex.Asky;

namespace Webinex.Activity.Server;

public interface IActivityServerConfiguration
{
    Type RowType { get; }
    IServiceCollection Services { get; }
}

public interface IActivityServerConfiguration<TActivityRow> : IActivityServerConfiguration
{
}

internal class ActivityServerConfiguration<TActivityRow> : IActivityServerConfiguration<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    public Type RowType => typeof(TActivityRow);
    public IServiceCollection Services { get; }

    private ActivityServerConfiguration(IServiceCollection services)
    {
        Services = services;
        Services.AddSingleton(this);
        Services.TryAddSingleton<IAskyFieldMap<TActivityRow>, ActivityRowBaseAskyFieldMap<TActivityRow>>();
        Services.TryAddSingleton<IActivityRowFactory<TActivityRow>, DefaultActivityRowFactory<TActivityRow>>();
    }

    internal static ActivityServerConfiguration<TActivityRow> GetOrCreate(IServiceCollection services)
    {
        var instance = (ActivityServerConfiguration<TActivityRow>?)services
            .FirstOrDefault(x => x.ServiceType == typeof(ActivityServerConfiguration<TActivityRow>))
            ?.ImplementationInstance;

        return instance ?? new ActivityServerConfiguration<TActivityRow>(services);
    }
}