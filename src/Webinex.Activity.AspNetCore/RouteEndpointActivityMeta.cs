using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Webinex.Activity.AspNetCore;

internal class RouteEndpointActivityMeta : IActivityMeta
{
    private readonly HttpContext _httpContext;

    private RouteEndpointActivityMeta(HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public bool IsActivity => _httpContext.IsImplicitMatch()
        ? _httpContext.ActivityEndpointMetadata()?.Activity != false
        : _httpContext.ActivityEndpointMetadata()?.Activity == true;

    public string? Kind => _httpContext.ActivityEndpointMetadata()?.Kind ?? ImplicitName;

    private string ImplicitName
    {
        get
        {
            var groupName = _httpContext.EndpointMetadata()?.GetMetadata<EndpointGroupNameAttribute>()
                ?.EndpointGroupName;
            var endpointName = _httpContext.EndpointMetadata()?.GetMetadata<RouteNameMetadata>()?.RouteName;

            return endpointName != null ? groupName != null ? $"{groupName}:{endpointName}" : endpointName : "N/A";
        }
    }

    public static bool TryGet(HttpContext httpContext, [NotNullWhen(true)] out RouteEndpointActivityMeta? meta)
    {
        meta = null;

        if (httpContext.EndpointFeature()?.Endpoint?.Metadata.GetMetadata<MethodInfo>() == null)
            return false;

        meta = new RouteEndpointActivityMeta(httpContext);
        return true;
    }
}