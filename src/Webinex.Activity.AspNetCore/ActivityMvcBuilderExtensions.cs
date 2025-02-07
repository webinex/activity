using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.AspNetCore;

public static class ActivityMvcBuilderExtensions
{
    public static IMvcBuilder AddActivityAspNetCore(
        [NotNull] this IMvcBuilder mvcBuilder,
        Action<IActivityAspNetCoreConfiguration>? configure = null)
    {
        mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));

        var configuration = ActivityAspNetCoreConfiguration.GetOrCreate(mvcBuilder);
        configure?.Invoke(configuration);

        return mvcBuilder;
    }
}