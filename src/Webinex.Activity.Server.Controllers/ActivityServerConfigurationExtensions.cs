using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.Server.Controllers
{
    public static class ActivityServerConfigurationExtensions
    {
        public static IMvcBuilder AddActivityServerController(
            [NotNull] this IMvcBuilder mvcBuilder,
            Action<IActivityServerControllerConfiguration> configure = null)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));
            
            var configuration = new ActivityServerControllerConfiguration(mvcBuilder);
            configure?.Invoke(configuration);
            configuration.Complete();

            return mvcBuilder;
        }
    }
}