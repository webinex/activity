using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public interface IActivityContextInitializer
    {
        void Initialize([NotNull] IActivityContext context);
    }
}