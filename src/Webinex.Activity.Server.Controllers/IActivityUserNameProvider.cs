using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webinex.Activity.Server.Controllers
{
    public interface IActivityUserNameProvider
    {
        Task<IDictionary<string, string>> GetUserNamesByIdAsync(IEnumerable<string> ids);
    }
}