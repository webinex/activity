using System;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.Server.EfCore;

namespace Webinex.Activity.Server;

public interface IActivityServerDbContextConfiguration
{
    /// <summary>
    /// Automatically creates all required tables
    /// </summary>
    /// <remarks>WARNING: Use only with default implementation of <see cref="IActivityDbContext"/></remarks>
    public IActivityServerDbContextConfiguration UseTablesAutoCreation();

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

    public IActivityServerDbContextConfiguration UseTablesAutoCreation()
    {
        if (_dbContextType != typeof(ActivityDbContext))
        {
            throw new InvalidOperationException(
                $"Tables auto creation can be used only with default {nameof(IActivityDbContext)} implementation");
        }

        Services.AddHostedService<ActivityDbFactory>();

        return this;
    }
}