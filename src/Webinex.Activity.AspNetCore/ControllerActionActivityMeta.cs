using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Webinex.Activity.AspNetCore;

internal class ControllerActionActivityMeta : IActivityMeta
{
    private readonly HttpContext _httpContext;

    private ControllerActionActivityMeta(
        HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public static bool TryGet(
        HttpContext httpContext,
        [NotNullWhen(true)] out ControllerActionActivityMeta? controllerActionActivityMeta)
    {
        controllerActionActivityMeta = null;
        if (httpContext.ControllerActionDescriptor() == null) return false;

        controllerActionActivityMeta = new ControllerActionActivityMeta(httpContext);
        return true;

    }

    private ControllerActionDescriptor Descriptor => _httpContext.ControllerActionDescriptor()!;

    public string Kind => Area == null
        ? ActionKind
        : $"{Area}:{ActionKind}";

    private string ActionKind => ActivityAttr?.Kind ?? Descriptor.ActionName;

    public bool IsActivity => HasActivityDecoration || IsImplicitActivity;

    public bool IsImplicitActivity => _httpContext.IsImplicitMatch() && !ControllerNotActivity &&
                                      !HasNotActivityAction && !HasActivityDecoration;

    private string? Area
    {
        get
        {
            if (NoArea)
                return null;

            var controllerArea = Controller.GetCustomAttribute<ActivityAreaAttribute>()?.Area;
            return !IsImplicitActivity ? controllerArea : controllerArea ?? Descriptor.ControllerName;
        }
    }

    private bool NoArea => ActivityAttr?.NoArea == true;

    private ActivityAttribute? ActivityAttr => Method.GetCustomAttribute<ActivityAttribute>();

    private bool HasActivityDecoration => Method.GetCustomAttribute<ActivityAttribute>() != null;

    private bool HasNotActivityAction => Method.GetCustomAttribute<NotActivityAttribute>() != null;

    private bool ControllerNotActivity => Controller.GetCustomAttribute<NotActivityAttribute>() != null;

    private TypeInfo Controller => Descriptor.ControllerTypeInfo;

    private MethodInfo Method => Descriptor.MethodInfo;
}