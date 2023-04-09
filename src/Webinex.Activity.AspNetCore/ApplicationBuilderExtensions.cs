using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;

namespace Webinex.Activity.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseActivity(
            [NotNull] this IApplicationBuilder app)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));

            app.UseMiddleware<ActivityMiddleware>();
            return app;
        }
    }
}