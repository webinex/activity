using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Webinex.Activity.Server
{
    public interface IActivityServerConfiguration
    {
        [NotNull]
        IServiceCollection Services { get; }
        
        [NotNull]
        IDictionary<string, object> Values { get; }
    }
    
    internal class ActivityServerConfiguration : IActivityServerConfiguration
    {
        internal ActivityServerConfiguration(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();
    }
}