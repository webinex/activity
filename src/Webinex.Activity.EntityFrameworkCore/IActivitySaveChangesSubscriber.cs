using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.EntityFrameworkCore;

public interface IActivitySaveChangesSubscriber
{
    Task ProcessAsync(DbContext dbContext, IEnumerable<EntityChange> entityChanges, CancellationToken cancellationToken);
}