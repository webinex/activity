using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Webinex.Activity
{
    public static class ActivityJson
    {
        private static JsonSerializerOptions JsonOptions => new JsonSerializerOptions
        {
            Converters = { new ActivityValuesJsonConverter() }
        };

        public static string? Serialize(IActivityBatchValue? batch)
        {
            return batch == null ? null : JsonSerializer.Serialize(batch, typeof(IActivityBatchValue), JsonOptions);
        }

        public static string? Serialize(ActivityValues? values)
        {
            return values == null ? null : JsonSerializer.Serialize(values, JsonOptions);
        }

        [return: NotNullIfNotNull(nameof(json))]
        public static IActivityBatchValue? DeserializeBatch(string? json)
        {
            return json == null ? null : JsonSerializer.Deserialize<ActivityBatchValue>(json, JsonOptions);
        }

        public static ActivityPathItem[] DeserializePath(string? json)
        {
            return json == null
                ? Array.Empty<ActivityPathItem>()
                : JsonSerializer.Deserialize<ActivityPathItem[]>(json, JsonOptions) ??
                  throw new ArgumentNullException(nameof(json));
        }

        [return: NotNullIfNotNull(nameof(json))]
        public static ActivityValues? DeserializeValues(string? json)
        {
            return json == null ? null : JsonSerializer.Deserialize<ActivityValues>(json, JsonOptions);
        }

        public static IActivitySystemValues? DeserializeSystemValues(string? json)
        {
            return json == null ? null : JsonSerializer.Deserialize<ActivitySystemValues>(json, JsonOptions);
        }

        private class ActivityBatchValue : IActivityBatchValue
        {
            public ActivityContextValue Context { get; set; } = null!;
            IActivityContextValue IActivityBatchValue.Context => Context;
            public IEnumerable<ActivityValue> Activities { get; set; } = Array.Empty<ActivityValue>();
            IEnumerable<IActivityValue> IActivityBatchValue.Activities => Activities;
        }

        private class ActivityValue : IActivityValue
        {
            public string Id { get; set; } = null!;
            public string Kind { get; set; } = null!;
            public string? ParentId { get; set; }
            public ActivitySystemValues SystemValues { get; set; } = null!;
            IActivitySystemValues IActivityValue.SystemValues => SystemValues;
            public JsonObject Values { get; set; } = null!;

            private ActivityValues? _values = null;

            ActivityValues IActivityValue.Values
            {
                get
                {
                    if (_values != null)
                        return _values;

                    _values = new ActivityValues(Values);
                    return _values;
                }
            }
        }

        private class ActivityContextValue : IActivityContextValue
        {
            public ActivitySystemValues SystemValues { get; set; } = null!;
            IActivitySystemValues IActivityContextValue.SystemValues => SystemValues;
        }

        private class ActivitySystemValues : IActivitySystemValues
        {
            public string OperationId { get; set; } = null!;
            public string? UserId { get; set; }
            public DateTimeOffset PerformedAt { get; set; }
            public bool Success { get; set; }
            public bool System { get; set; }
        }

        private class ActivityValuesJsonConverter : JsonConverter<ActivityValues>
        {
            public override ActivityValues? Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException($"`{nameof(ActivityValues)}` might be object.");

                using var jsonDocument = JsonDocument.ParseValue(ref reader);
                var jsonObject = jsonDocument.RootElement.Deserialize<JsonObject>();
                return new ActivityValues(jsonObject);
            }

            public override void Write(
                Utf8JsonWriter writer,
                ActivityValues values,
                JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, values.Value, typeof(JsonObject), options);
            }
        }
    }
}