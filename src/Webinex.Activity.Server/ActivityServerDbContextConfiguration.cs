using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server;

public interface IActivityServerDbContextConfiguration
{
    /// <summary>
    /// Automatically creates all required tables
    /// </summary>
    /// <remarks>WARNING: Use only with default implementation of <see cref="IActivityDbContext{T}"/></remarks>
    public IActivityServerDbContextConfiguration UseTablesAutoCreation();

    public IServiceCollection Services { get; }
}

internal class ActivityServerDbContextConfiguration : IActivityServerDbContextConfiguration
{
    private readonly Type _rowType;
    private readonly Type _dbContextType;
    public IServiceCollection Services { get; }

    public ActivityServerDbContextConfiguration(IServiceCollection services, Type rowType, Type dbContextType)
    {
        _dbContextType = dbContextType;
        _rowType = rowType;
        Services = services;
    }

    public IActivityServerDbContextConfiguration UseTablesAutoCreation()
    {
        if (_dbContextType != typeof(ActivityDbContext<>).MakeGenericType(_rowType))
        {
            throw new InvalidOperationException(
                $"Tables auto creation can be used only with default {nameof(IActivityDbContext<ActivityRowBase>)} implementation");
        }

        Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService),
            typeof(ActivityDbFactory<>).MakeGenericType(_rowType)));

        return this;
    }
}