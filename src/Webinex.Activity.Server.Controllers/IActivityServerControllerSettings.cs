using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity.Server.Controllers
{
    public interface IActivityServerControllerSettings
    {
        [MaybeNull]
        string Schema { get; }
        
        [MaybeNull]
        string Policy { get; }
    }
}