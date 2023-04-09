using System.Threading.Tasks;

namespace Webinex.Activity
{
    public interface IActivityStore
    {
        Task StoreAsync(IActivityBatchValue batch);
    }
}