using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Webinex.Activity.Http
{
    public static class ActivityHttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddActivityForwarding([NotNull] this IHttpClientBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddTransient<ActivityForwardingDelegatingHandler>();
            builder.AddHttpMessageHandler<ActivityForwardingDelegatingHandler>();
            return builder;
        }
    }
}