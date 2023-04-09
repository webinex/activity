using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webinex.Activity.Server.Controllers
{
    internal class DefaultActivityUserNameProvider : IActivityUserNameProvider
    {
        public Task<IDictionary<string, string>> GetUserNamesByIdAsync(IEnumerable<string> ids)
        {
            var result = ids.ToDictionary(x => x, x => x);
            return Task.FromResult<IDictionary<string, string>>(result);
        }
    }
}