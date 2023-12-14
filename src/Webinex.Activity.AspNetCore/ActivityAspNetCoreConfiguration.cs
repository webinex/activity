using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.AspNetCore
{
    internal interface IActivityAspNetCoreSettings
    {
        bool Implicit { get; }

        [MaybeNull]
        Func<HttpContext, ControllerActionDescriptor, bool> ImplicitPredicate { get; }
    }

    public interface IActivityAspNetCoreConfiguration
    {
        IMvcBuilder MvcBuilder { get; }

        IDictionary<string, object> Values { get; }

        IActivityAspNetCoreConfiguration UseImplicitWhen(Func<HttpContext, ControllerActionDescriptor, bool> predicate);
    }

    internal class ActivityAspNetCoreConfiguration : IActivityAspNetCoreConfiguration, IActivityAspNetCoreSettings
    {
        private ActivityAspNetCoreConfiguration(IMvcBuilder mvcBuilder)
        {
            MvcBuilder = mvcBuilder;

            mvcBuilder.Services.AddSingleton<IActivityAspNetCoreSettings>(this);
        }

        public IMvcBuilder MvcBuilder { get; }

        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public IActivityAspNetCoreConfiguration UseImplicitWhen(
            Func<HttpContext, ControllerActionDescriptor, bool> predicate)
        {
            Implicit = true;
            ImplicitPredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }

        public bool Implicit { get; private set; }
        public Func<HttpContext, ControllerActionDescriptor, bool>? ImplicitPredicate { get; private set; }

        public static ActivityAspNetCoreConfiguration GetOrCreate(IMvcBuilder mvcBuilder)
        {
            var instance = mvcBuilder.Services.FirstOrDefault(x =>
                    x.ServiceType == typeof(ActivityAspNetCoreConfiguration))
                ?.ImplementationInstance as ActivityAspNetCoreConfiguration;

            if (instance != null)
                return instance;

            instance = new ActivityAspNetCoreConfiguration(mvcBuilder);
            mvcBuilder.Services.AddSingleton(instance);
            return instance;
        }
    }

    public static class ActivityAspNetCoreConfigurationExtensions
    {
        public static IActivityAspNetCoreConfiguration UseImplicit(
            [NotNull] this IActivityAspNetCoreConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return configuration.UseImplicitWhen((_, __) => true);
        }
    }
}