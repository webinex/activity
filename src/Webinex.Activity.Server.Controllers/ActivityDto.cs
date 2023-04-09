using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Webinex.Activity.Server.Controllers
{
    public class ActivityDto
    {
        public string Id { get; set; }
        public string Kind { get; set; }
        public string OperationId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset PerformedAt { get; set; }
        public string ParentId { get; set; }
        public JsonObject Values { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalValues { get; } = new Dictionary<string, object>();
    }
}