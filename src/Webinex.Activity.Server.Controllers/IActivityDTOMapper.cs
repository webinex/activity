using System.Collections.Generic;
using System.Threading.Tasks;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server.Controllers
{
    public interface IActivityDTOMapper<TActivityRow>
        where TActivityRow : ActivityRowBase
    {
        Task<IReadOnlyCollection<ActivityDTO>> MapManyAsync(IEnumerable<TActivityRow> rows);
    }
}