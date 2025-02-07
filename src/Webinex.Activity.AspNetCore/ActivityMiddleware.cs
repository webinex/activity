using System;
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
            var action = PushActivity(httpContext);

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

        private IDisposableActivity? PushActivity(HttpContext context)
        {
            if (RequestActivityMetaFactory.TryGet(context, out var meta) && meta.IsActivity)
            {
                return _activityScopeProvider.RequiredValue.Push(meta.Kind!);
            }

            return null;
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
                await _activityScopeProvider.RequiredValue.CompleteAsync(success);
            }
            catch (Exception ex)
            {
                var status = success ? "success" : "failed";
                _logger.LogError(ex, "Failed to complete scope as {Status}, exception would be ignored", status);
            }
        }

        private ActivityToken? GetActivityToken(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey(ActivityHttpDefaults.HEADER_NAME))
                return null;

            var value = httpContext.Request.Headers[ActivityHttpDefaults.HEADER_NAME];
            return ActivityToken.Parse(value!, _activitySettings.ForwardingSecret ?? throw new ArgumentNullException());
        }
    }
}