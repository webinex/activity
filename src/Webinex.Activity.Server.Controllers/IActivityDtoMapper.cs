using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Webinex.Activity.Server.EfCore;

namespace Webinex.Activity.Server.Controllers
{
    public interface IActivityDtoMapper
    {
        Task<ActivityDto[]> MapManyAsync(IEnumerable<ActivityRow> rows);
    }
}