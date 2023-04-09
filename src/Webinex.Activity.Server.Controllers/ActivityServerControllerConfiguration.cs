using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Webinex.Activity.Server.EfCore;
using Webinex.Asky;

namespace Webinex.Activity.Server.Controllers
{
    public interface IActivityServerControllerConfiguration
    {
        IActivityServerControllerConfiguration UsePolicy([NotNull] string schema, [NotNull] string policy);

        IActivityServerControllerConfiguration UseAllowAnonymousController();
    }

    internal class ActivityServerControllerConfiguration : IActivityServerControllerConfiguration,
        IActivityServerControllerSettings
    {
        private readonly IMvcBuilder _mvcBuilder;
        private bool _allowAnonymousController = false;

        public ActivityServerControllerConfiguration(IMvcBuilder mvcBuilder)
        {
            _mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));
            _mvcBuilder.Services.AddSingleton<IActivityServerControllerSettings>(this);
            _mvcBuilder.Services.AddScoped<IAskyFieldMap<ActivityRow>, ActivityRowAskyFieldMap>();
            _mvcBuilder.Services.AddScoped<IActivityReadService, ActivityReadService>();
        }

        public IActivityServerControllerConfiguration UsePolicy(string schema, string policy)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            return this;
        }

        public IActivityServerControllerConfiguration UseAllowAnonymousController()
        {
            _allowAnonymousController = true;
            return this;
        }

        public string Schema { get; private set; }
        public string Policy { get; private set; }

        internal void Complete()
        {
            _mvcBuilder.AddController(_allowAnonymousController
                ? typeof(ActivityControllerBase.Anonymous.ActivityController)
                : typeof(ActivityControllerBase.Default.ActivityController));

            _mvcBuilder.Services.TryAddScoped<IActivityDtoMapper, DefaultActivityDtoMapper>();
            _mvcBuilder.Services.TryAddSingleton<IActivityUserNameProvider, DefaultActivityUserNameProvider>();
        }
    }
}