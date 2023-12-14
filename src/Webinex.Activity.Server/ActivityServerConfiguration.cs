using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.Server;

public interface IActivityServerConfiguration
{
    IServiceCollection Services { get; }
}

internal class ActivityServerConfiguration : IActivityServerConfiguration
{
    internal ActivityServerConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}