using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.DataAccess;

public interface IActivityDbContext<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    DbSet<TActivityRow> Activities { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}