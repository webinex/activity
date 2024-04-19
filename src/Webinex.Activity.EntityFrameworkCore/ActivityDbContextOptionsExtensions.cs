using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.EntityFrameworkCore;

public static class ActivityDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseActivitySaveChangesInterceptor(this DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new ActivitySaveChangesInterceptor<IActivitySaveChangesSubscriber>());
        return optionsBuilder;
    }

    public static DbContextOptionsBuilder UseActivitySaveChangesInterceptor<TSubscriber>(this DbContextOptionsBuilder optionsBuilder)
        where TSubscriber : IActivitySaveChangesSubscriber
    {
        optionsBuilder.AddInterceptors(new ActivitySaveChangesInterceptor<TSubscriber>());
        return optionsBuilder;
    }
}