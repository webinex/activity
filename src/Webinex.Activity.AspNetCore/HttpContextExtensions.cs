using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.AspNetCore;

internal static class HttpContextExtensions
{
    public static IEndpointFeature? EndpointFeature(this HttpContext httpContext)
    {
        return httpContext.Features.Get<IEndpointFeature>();
    }

    public static ControllerActionDescriptor? ControllerActionDescriptor(this HttpContext httpContext)
    {
        return httpContext.EndpointFeature()?.Endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
    }

    public static bool IsImplicitMatch(this HttpContext httpContext)
    {
        var settings = httpContext.RequestServices.GetRequiredService<IActivityAspNetCoreSettings>();
        return settings.Implicit && (settings.ImplicitPredicate == null || settings.ImplicitPredicate(httpContext));
    }

    public static EndpointMetadataCollection? EndpointMetadata(this HttpContext httpContext)
    {
        return httpContext.EndpointFeature()?.Endpoint?.Metadata;
    }

    public static ActivityEndpointMetadata? ActivityEndpointMetadata(this HttpContext httpContext)
    {
        return httpContext.EndpointMetadata()?.GetMetadata<ActivityEndpointMetadata>();
    }
}