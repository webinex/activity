using System.Collections.Generic;
using System.Threading.Tasks;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server;

public interface IActivityRowFactory<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    Task<IReadOnlyCollection<TActivityRow>> MapAsync(IEnumerable<IActivityValue> values);
}