using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Webinex.Activity
{
    internal static class ActivityValueFlattener
    {
        private const string ROOT_MARKER = "$";
        private const string ROOT_PREFIX = "$.";

        public static ActivityValueScalar[] Flatten([MaybeNull] JsonObject jObject)
        {
            return FlattenNode(jObject).ToArray();
        }

        private static IEnumerable<ActivityValueScalar> FlattenNode(JsonNode? jNode)
        {
            if (jNode == null)
                return Array.Empty<ActivityValueScalar>();

            switch (jNode)
            {
                case JsonArray jArray:
                    return FlattenArray(jArray);
                case JsonObject jObject:
                    return FlattenObject(jObject);
                case JsonValue jValue:
                    return new[] { Value(jValue) };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<ActivityValueScalar> FlattenArray(JsonArray jArray)
        {
            return jArray.SelectMany((jNode, index) =>
            {
                if (jNode != null)
                    return FlattenNode(jNode);

                var jArrayPath = Path(jArray);
                var path = $"{jArrayPath}[{index}]";
                return new[] { new ActivityValueScalar(path, ActivityValueKind.Null, null) };

            });
        }

        private static IEnumerable<ActivityValueScalar> FlattenObject(JsonObject jObject)
        {
            return jObject.SelectMany(kv =>
            {
                if (kv.Value != null) 
                    return FlattenNode(kv.Value);

                var jObjectPath = Path(jObject);
                var path = jObjectPath == null ? kv.Key : $"{jObjectPath}.{kv.Key}";
                return new[] { new ActivityValueScalar(path, ActivityValueKind.Null, null) };

            });
        }

        private static ActivityValueScalar Value(JsonValue jValue)
        {
            var path = Path(jValue) ?? throw new ArgumentNullException();
            var element = jValue.GetValue<JsonElement>();
            return new ActivityValueScalar(path, Kind(element.ValueKind), jValue.ToString());
        }

        private static string? Path(JsonNode jNode)
        {
            var path = jNode.GetPath();
            return path == ROOT_MARKER ? null : path.Substring(ROOT_PREFIX.Length);
        }

        private static ActivityValueKind Kind(JsonValueKind kind)
        {
            switch (kind)
            {
                case JsonValueKind.String:
                    return ActivityValueKind.String;
                case JsonValueKind.Number:
                    return ActivityValueKind.Number;
                case JsonValueKind.True:
                    return ActivityValueKind.Boolean;
                case JsonValueKind.False:
                    return ActivityValueKind.Boolean;
                case JsonValueKind.Undefined:
                    return ActivityValueKind.Null;
                case JsonValueKind.Null:
                    return ActivityValueKind.Null;
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    throw new InvalidOperationException("Activity value kind might be resolved only for primitives or string");
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unexpected kind value");
            }
        }
    }
}