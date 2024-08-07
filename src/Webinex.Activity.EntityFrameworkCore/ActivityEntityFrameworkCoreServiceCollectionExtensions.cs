using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.EntityFrameworkCore;

public static class ActivityEntityFrameworkCoreServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultActivitySaveChangesSubscriber(
        this IServiceCollection services,
        Action<IDefaultActivitySaveChangesConfiguration>? configure = null)
    {
        var settings = new DefaultActivitySaveChangesSubscriberSettings();
        configure?.Invoke(settings);
        services.AddSingleton(settings);
        return services.AddScoped<IActivitySaveChangesSubscriber, DefaultActivitySaveChangesSubscriber>();
    }
}