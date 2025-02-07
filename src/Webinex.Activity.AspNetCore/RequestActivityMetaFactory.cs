using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Webinex.Activity.AspNetCore;

internal static class RequestActivityMetaFactory
{
    public static bool TryGet(HttpContext context, [NotNullWhen(true)] out IActivityMeta? meta)
    {
        meta = null;

        if (ControllerActionActivityMeta.TryGet(context, out var controllerActionActivityMeta))
        {
            meta = controllerActionActivityMeta;
            return true;
        }

        if (RouteEndpointActivityMeta.TryGet(context, out var routeEndpointActivityMeta))
        {
            meta = routeEndpointActivityMeta;
            return true;
        }
        
        return false;
    }
}