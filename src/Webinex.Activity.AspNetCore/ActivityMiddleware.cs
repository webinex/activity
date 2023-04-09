using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Webinex.Activity.Http;

namespace Webinex.Activity.AspNetCore
{
    internal class ActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IActivityScopeProvider _activityScopeProvider;
        private readonly IActivitySettings _activitySettings;
        private readonly IActivityAspNetCoreSettings _activityAspNetCoreSettings;
        private readonly ILogger<ActivityMiddleware> _logger;

        public ActivityMiddleware(
            RequestDelegate next,
            IActivityScopeProvider activityScopeProvider,
            IActivitySettings activitySettings,
            IActivityAspNetCoreSettings activityAspNetCoreSettings,
            ILogger<ActivityMiddleware> logger)
        {
            _next = next;
            _activityScopeProvider = activityScopeProvider;
            _activitySettings = activitySettings;
            _activityAspNetCoreSettings = activityAspNetCoreSettings;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            CreateScope(httpContext);
            var meta = GetActivityMeta(httpContext);
            var action = meta?.IsActivity == true ? _activityScopeProvider.Value.Push(meta.Kind) : null;

            try
            {
                await _next(httpContext);
                action?.Complete(true);
                await CompleteAsync(true);
            }
            catch
            {
                action?.Complete(false);
                _logger.LogInformation("Exception appeared, completing scope as failed");
                await CompleteAsync(false);

                throw;
            }
        }

        private void CreateScope(HttpContext httpContext)
        {
            var scopeFactory = httpContext.RequestServices.GetRequiredService<IActivityScopeFactory>();
            var incomeToken = GetActivityToken(httpContext);
            _activityScopeProvider.Value = scopeFactory.Create(incomeToken);
        }

        private async Task CompleteAsync(bool success)
        {
            try
            {
                await _activityScopeProvider.Value.CompleteAsync(success);
            }
            catch (Exception ex)
            {
                var status = success ? "success" : "failed";
                _logger.LogError(ex, "Failed to complete scope as {Status}, exception would be ignored", status);
            }
        }

        private ActivityToken GetActivityToken(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(ActivityHttpDefaults.HEADER_NAME))
                return null;

            var value = httpContext.Request.Headers[ActivityHttpDefaults.HEADER_NAME];
            return ActivityToken.Parse(value, _activitySettings.ForwardingSecret);
        }

        private ActionMeta GetActivityMeta(HttpContext httpContext)
        {
            var descriptor = httpContext.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata
                .GetMetadata<ControllerActionDescriptor>();

            return descriptor == null ? null : new ActionMeta(_activityAspNetCoreSettings, descriptor, httpContext);
        }

        private class ActionMeta
        {
            private readonly IActivityAspNetCoreSettings _settings;
            private readonly ControllerActionDescriptor _descriptor;
            private readonly HttpContext _httpContext;

            public ActionMeta(
                IActivityAspNetCoreSettings settings,
                ControllerActionDescriptor descriptor,
                HttpContext httpContext)
            {
                _settings = settings;
                _descriptor = descriptor;
                _httpContext = httpContext;
            }

            public string Kind => Area == null
                ? ActionKind
                : $"{Area}:{ActionKind}";

            private string ActionKind => ActivityAttr?.Kind ?? _descriptor.ActionName;

            public bool IsActivity => HasActivityDecoration || IsImplicitActivity;

            public bool IsImplicitActivity => _settings.Implicit && ImplicitPredicateTrue && !ControllerNotActivity &&
                                              !HasNotActivityAction && !HasActivityDecoration;

            private string Area
            {
                get
                {
                    if (NoArea)
                        return null;

                    var controllerArea = Controller.GetCustomAttribute<ActivityAreaAttribute>()?.Area;
                    return !IsImplicitActivity ? controllerArea : controllerArea ?? _descriptor.ControllerName;
                }
            }

            private bool NoArea => ActivityAttr?.NoArea == true;

            private ActivityAttribute ActivityAttr => Method.GetCustomAttribute<ActivityAttribute>();

            private bool HasActivityDecoration => Method.GetCustomAttribute<ActivityAttribute>() != null;

            private bool HasNotActivityAction => Method.GetCustomAttribute<NotActivityAttribute>() != null;

            private bool ControllerNotActivity => Controller.GetCustomAttribute<NotActivityAttribute>() != null;

            private bool ImplicitPredicateTrue => _settings.ImplicitPredicate == null ||
                                                  _settings.ImplicitPredicate?.Invoke(_httpContext, _descriptor) ==
                                                  true;

            private TypeInfo Controller => _descriptor.ControllerTypeInfo;

            private MethodInfo Method => _descriptor.MethodInfo;
        }
    }
}