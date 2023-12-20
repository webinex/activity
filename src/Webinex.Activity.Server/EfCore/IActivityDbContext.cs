using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.EfCore;

public interface IActivityDbContext
{
    DbSet<ActivityRow> Activities { get; }
    DbSet<ActivityValueRow> ActivityValues { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}