using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Webinex.Activity;

public class ActivityValues
{
    private bool _frozen;

    private static readonly Type[] PrimitiveSerializableReferenceTypes =
    [
        typeof(string), typeof(Guid), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan), typeof(Uri),
        typeof(DateOnly), typeof(TimeOnly)
    ];

    public ActivityValues(JsonObject? jsonObject = null)
    {
        Value = jsonObject ?? new JsonObject();
    }

    internal JsonObject Value { get; }

    public JsonObject AsJsonObject()
    {
        var jObject = JsonSerializer.SerializeToNode(Value);
        return jObject!.AsObject();
    }

    public static ActivityValues Parse(string value)
    {
        return new ActivityValues(JsonSerializer.Deserialize<JsonObject>(value));
    }

    public static ActivityValues Create(ActivityValueScalar[] flattens)
    {
        var result = new ActivityValues();
        var ordered = flattens
            .OrderBy(x => x.Path.Pattern)
            .ThenBy(x => x.Path.Value.Length)
            .ThenBy(x => x.Path.Value).ToArray();

        foreach (var activityValueFlatten in ordered)
        {
            result.Enrich(activityValueFlatten.Path.Value, activityValueFlatten.GetValue());
        }

        return result;
    }

    public void Freeze()
    {
        _frozen = true;
    }

    public ActivityValueScalar[] Flatten()
    {
        return ActivityValueFlattener.Flatten(Value);
    }

    public void Enrich(object value)
    {
        if (value == null!)
        {
            throw new InvalidOperationException("Value cannot be null");
        }

        if (IsPrimitive(value))
        {
            throw new InvalidOperationException("Primitive serializable type cannot be used as root value");
        }

        if (value is IEnumerable)
        {
            throw new InvalidOperationException("Enumerable type cannot be used as root value");
        }

        if (value is IDictionary dictionary)
        {
            Enrich(dictionary);
            return;
        }

        var type = value.GetType();

        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            if (property.GetGetMethod() != null && property.GetGetMethod()!.IsPublic)
                Enrich(property.Name, property.GetValue(value));
        }
    }

    public void Enrich(IDictionary values)
    {
        foreach (DictionaryEntry entry in values)
        {
            if (entry.Key.ToString() == null)
            {
                throw new InvalidOperationException("Key cannot be null");
            }

            Enrich(entry.Key.ToString()!, entry.Value);
        }
    }

    public void Enrich(string path, object? value)
    {
        if (_frozen)
            throw new InvalidOperationException($"{nameof(ActivityValues)} frozen");

        var jPath = new JsonPath(path);

        JsonNode current = Value;
        for (int i = 0; i < jPath.Path.Length; i++)
        {
            var item = jPath.Path[i];

            if (i == jPath.Path.Length - 1)
            {
                AppendValue(current, item, value);
                break;
            }

            var newCurrent = item.IsIndex
                ? current.AsArray().Count <= item.Index ? null : current.AsArray()[item.Index]
                : current[item.Value];

            if (newCurrent == null)
            {
                var next = jPath.Path[i + 1];
                var newCurrentNode = next.IsIndex ? new JsonArray() : (JsonNode)new JsonObject();
                Append(current, item, newCurrentNode);
                current = newCurrentNode;
            }
            else
            {
                current = newCurrent;
            }
        }
    }

    private void AppendValue(JsonNode node, JsonPath.Item path, object? value)
    {
        if (IsPrimitive(value))
        {
            Append(node, path, JsonValue.Create(value));
            return;
        }

        if (value is IEnumerable and not IDictionary)
        {
            Append(node, path, JsonArray.Create(JsonSerializer.SerializeToElement(value)));
            return;
        }

        Append(node, path, JsonObject.Create(JsonSerializer.SerializeToElement(value)));
    }

    private bool IsPrimitive(object? value)
    {
        return value == null || value.GetType().IsPrimitive ||
               PrimitiveSerializableReferenceTypes.Contains(value.GetType());
    }

    private void Append(JsonNode node, JsonPath.Item path, JsonNode? value)
    {
        if (path.IsIndex)
        {
            var jArray = node.AsArray();
            AssertValidIndex(jArray, path.Index);

            if (path.Index == jArray.Count)
            {
                jArray.Add(value);
            }
            else
            {
                jArray[path.Index] = value;
            }
        }
        else
        {
            var jObject = node.AsObject();
            jObject[path.Value] = value;
        }
    }

    private void AssertValidIndex(JsonArray jArray, int index)
    {
        if (index < 0 || index > jArray.Count)
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    public T? Get<T>(string path, T? defaultValue = default(T))
    {
        var jPath = new JsonPath(path);

        JsonNode? current = Value;

        for (int i = 0; i < jPath.Path.Length; i++)
        {
            if (current == null)
            {
                return defaultValue;
            }

            var item = jPath.Path[i];

            if (item.IsIndex)
            {
                current = current.AsArray()[item.Index];
            }
            else
            {
                current = current[item.Value];
            }
        }

        if (current == null)
        {
            return defaultValue;
        }

        return current.Deserialize<T>() ?? defaultValue;
    }
}