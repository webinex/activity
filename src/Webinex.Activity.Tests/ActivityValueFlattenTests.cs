using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using Shouldly;

namespace Webinex.Activity.Tests;

public class ActivityValueFlattenTests
{
    [Test]
    public void WhenNestedString_ShouldBeOk()
    {
        var input = new
        {
            value1 = new
            {
                value2 = "value",
            },
        };

        var result = Flatten(input);
        ShouldContain(result, ActivityValueKind.String, "value1.value2", "value");
    }

    [Test]
    public void WhenNestedObjectStrings_ShouldBeOk()
    {
        var input = new
        {
            value1 = new
            {
                value2 = "value__2",
                value3 = "value__3",
            },
        };

        var result = Flatten(input);

        ShouldContain(result, ActivityValueKind.String, "value1.value2", "value__2");
        ShouldContain(result, ActivityValueKind.String, "value1.value3", "value__3");
    }

    [Test]
    public void WhenMultipleNestedObjectsStrings_ShouldBeOk()
    {
        var dateTime = new DateTime(2020, 01, 01);

        var input = new
        {
            parent1 = new
            {
                value1_1 = "value1_1",
                value1_2 = dateTime,
            },
            parent2 = new
            {
                value2_1 = 123,
            },
        };

        var result = Flatten(input);

        ShouldContain(result, ActivityValueKind.String, "parent1.value1_1", "value1_1");
        ShouldContain(result, ActivityValueKind.String, "parent1.value1_2", dateTime.ToString("s"));
        ShouldContain(result, ActivityValueKind.Number, "parent2.value2_1", "123");
    }

    [Test]
    public void WhenNullProperty_ShouldBeOk()
    {
        var input = new { value = (string)null };

        var result = Flatten(input);

        ShouldContain(result, ActivityValueKind.Null, "value", null);
    }

    [Test]
    public void WhenNestedArrayObject_ShouldBeOk()
    {
        var input = new
        {
            values = new[]
            {
                new { value = "123" },
            },
        };

        var result = Flatten(input);

        ShouldContain(result, ActivityValueKind.String, "values[0].value", "123");
    }

    [Test]
    public void WhenNestedArrayValueNull_ShouldBeOk()
    {
        var input = new
        {
            values = new[]
            {
                (string)null,
            },
        };

        var result = Flatten(input);

        ShouldContain(result, ActivityValueKind.Null, "values[0]", null);
    }

    private ActivityValueScalar[] Flatten(object value)
    {
        var jsonElement = JsonSerializer.SerializeToElement(value);
        return ActivityValueFlattener.Flatten(JsonObject.Create(jsonElement));
    }

    private void ShouldContain(ActivityValueScalar[] flatten, ActivityValueKind kind, string path, string value)
    {
        var found = flatten.First(x => x.Path.Value == path);
        found.ShouldNotBeNull();

        found.Kind.ShouldBe(kind);
        found.Value.ShouldBe(value);
    }
}