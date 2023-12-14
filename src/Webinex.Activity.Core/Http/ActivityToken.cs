using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using JWT.Algorithms;
using JWT.Builder;

namespace Webinex.Activity.Http
{
    public class ActivityToken
    {
        public ActivityToken(ActivityPathItem[] path, IActivitySystemValues systemValues)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            SystemValues = systemValues ?? throw new ArgumentNullException(nameof(systemValues));
        }

        public ActivityPathItem[] Path { get; }

        public IActivitySystemValues SystemValues { get; }

        public string Serialize(string secret)
        {
            secret = secret ?? throw new ArgumentNullException(nameof(secret));

            return JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .AddClaim("path", Path)
                .AddClaim("context_system_values", SystemValues)
                .Encode();
        }

        public static ActivityToken Parse(string token, string secret)
        {
            var value = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .Decode(token);

            var json = JsonNode.Parse(value)!.Root.AsObject();
            var path = json["path"]!.Deserialize<ActivityPathItem[]>() ?? throw new ArgumentNullException();
            var systemValues = json["context_system_values"].Deserialize<ActivitySystemValuesJson>() ??
                               throw new ArgumentNullException();
            return new ActivityToken(path, systemValues);
        }

        private class ActivitySystemValuesJson : IActivitySystemValues
        {
            public string OperationId { get; set; } = null!;
            public string? UserId { get; set; }
            public DateTimeOffset PerformedAt { get; set; }
            public bool Success { get; set; }
            public bool System { get; set; }
        }
    }
}