using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Webinex.Activity.Http
{
    public class ActivityForwardingDelegatingHandler : DelegatingHandler
    {
        private readonly IActivityScope _activityScope;
        private readonly IActivitySettings _activitySettings;

        public ActivityForwardingDelegatingHandler(
            IActivityScope activityScope,
            IActivitySettings activitySettings)
        {
            _activityScope = activityScope;
            _activitySettings = activitySettings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Headers.Contains(ActivityHttpDefaults.HEADER_NAME))
                return await base.SendAsync(request, cancellationToken);

            if (_activityScope.Current == null)
                return await base.SendAsync(request, cancellationToken);

            if (_activitySettings.ForwardingSecret == null)
                throw new InvalidOperationException("Activity forwarding secret not set");

            var token = new ActivityToken(_activityScope.Path(), _activityScope.Context.SystemValues);

            request.Headers.Add(ActivityHttpDefaults.HEADER_NAME, token.Serialize(_activitySettings.ForwardingSecret));
            return await base.SendAsync(request, cancellationToken);
        }
    }
}