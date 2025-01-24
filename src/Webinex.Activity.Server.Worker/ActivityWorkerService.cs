using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Webinex.Activity.Server.Worker.DataAccess;

namespace Webinex.Activity.Server.Worker;

public interface IActivityWorkerService
{
    Task ProcessAsync(IActivityBatchValue batch, CancellationToken cancellationToken);
}

internal class ActivityWorkerService : IActivityWorkerService
{
    private readonly IActivityStore _activityStore;

    public ActivityWorkerService(IActivityStore activityStore)
    {
        _activityStore = activityStore;
    }

    public async Task ProcessAsync(IActivityBatchValue batch, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var storeArgs = NewStoreArgs(batch);
        await _activityStore.AddAsync(storeArgs);
    }

    private ActivityStoreArgs[] NewStoreArgs(IActivityBatchValue batch)
    {
        var result = new LinkedList<ActivityStoreArgs>();
        foreach (var activity in batch.Activities)
            result.AddLast(new ActivityStoreArgs(activity));

        return result.ToArray();
    }
}