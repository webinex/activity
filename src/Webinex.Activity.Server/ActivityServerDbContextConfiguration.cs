using System;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.Server.EfCore;

namespace Webinex.Activity.Server;

public interface IActivityServerDbContextConfiguration
{
    public IActivityServerDbContextConfiguration UseDbInitializer();
    public IServiceCollection Services { get; }
}

internal class ActivityServerDbContextConfiguration : IActivityServerDbContextConfiguration
{
    private readonly Type _dbContextType;
    public IServiceCollection Services { get; }

    public ActivityServerDbContextConfiguration(IServiceCollection services, Type dbContextType)
    {
        _dbContextType = dbContextType;
        Services = services;
    }

    public IActivityServerDbContextConfiguration UseDbInitializer()
    {
        if (_dbContextType != typeof(ActivityDbContext))
        {
            throw new InvalidOperationException(
                $"Database initializer can be used only for default {nameof(ActivityDbContext)}");
        }

        Services.AddHostedService<ActivityDbFactory>();

        return this;
    }
}