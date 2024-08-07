using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.EntityFrameworkCore;

internal class DefaultActivitySaveChangesSubscriber : IActivitySaveChangesSubscriber
{
    private readonly IActivityScope _activityScope;
    private readonly DefaultActivitySaveChangesSubscriberSettings _settings;

    public DefaultActivitySaveChangesSubscriber(IActivityScope activityScope,
        DefaultActivitySaveChangesSubscriberSettings settings)
    {
        _activityScope = activityScope;
        _settings = settings;
    }

    public Task ProcessAsync(DbContext dbContext, IEnumerable<EntityChange> entityChanges,
        CancellationToken cancellationToken)
    {
        if (!_activityScope.Initialized)
            return Task.CompletedTask;

        if (_settings.SkipEqualValues)
            entityChanges = entityChanges.Where(x => x.Type != EntityChangeType.Updated || !x.IsValuesEqual())
                .ToArray();

        if (!entityChanges.Any())
            return Task.CompletedTask;

        using var _ = _activityScope.Push(_settings.Kind);
        _activityScope.Enrich(_settings.ValuePath, entityChanges);
        return Task.CompletedTask;
    }
}