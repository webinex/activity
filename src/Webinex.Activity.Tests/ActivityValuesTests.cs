using System;
using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

namespace Webinex.Activity.Tests;

public class ActivityValuesTests
{
    private ActivityValues _subject = null!;

    [Test]
    public void WhenPlainString_ShouldBeOk()
    {
        _subject.Enrich("value", "123");
        _subject.Get<string>("value").ShouldBe("123");
    }

    [Test]
    public void WhenComplexObjectPathString_ShouldBeOk()
    {
        _subject.Enrich("obj1.obj2.value", "123");
        _subject.Get<string>("obj1.obj2.value").ShouldBe("123");
    }

    [Test]
    public void WhenArrayValue_ShouldBeOk()
    {
        _subject.Enrich("obj1.obj2.value", new[] { "1", "2" });
        _subject.Get<IEnumerable<string>>("obj1.obj2.value").ShouldBe(new[] { "1", "2" });
    }

    [Test]
    public void WhenArrayInPath_ShouldBeOk()
    {
        _subject.Enrich("obj1.array[0].value", "123");
        _subject.Get<string>("obj1.array[0].value").ShouldBe("123");
    }

    [Test]
    public void WhenMultipleArrayEnriches_ShouldBeOk()
    {
        _subject.Enrich("obj1.array[0].value", "1");
        _subject.Enrich("obj1.array[1].value", "2");
        _subject.Get<string>("obj1.array[0].value").ShouldBe("1");
        _subject.Get<string>("obj1.array[1].value").ShouldBe("2");
    }

    [Test]
    public void WhenNull_ShouldBeOk()
    {
        _subject.Enrich("value", null);
        _subject.Get<object?>("value").ShouldBe(null);
    }

    [Test]
    public void WhenNullValue_ShouldBeOk()
    {
        _subject.Enrich("obj1.array[0].value", null);
        _subject.Get<string>("obj1.array[0].value").ShouldBe(null);
    }

    [Test]
    public void WhenObjectValue_ShouldBeOk()
    {
        _subject.Enrich("obj1.array[0].value", new { value_child = "1" });
        _subject.Get<string>("obj1.array[0].value.value_child").ShouldBe("1");
    }

    [Test]
    public void WhenArrayValue_PathAccessShouldBeOk()
    {
        _subject.Enrich("obj1.array[0].value", new[] { "123", "321" });
        _subject.Get<string>("obj1.array[0].value[0]").ShouldBe("123");
        _subject.Get<string>("obj1.array[0].value[1]").ShouldBe("321");
    }

    [Test]
    public void WhenIntValue_ShouldBeOk()
    {
        _subject.Enrich("obj1.value", 1);
        _subject.Get<int>("obj1.value").ShouldBe(1);
    }

    [Test]
    public void WhenDictionaryValue_ShouldBeOk()
    {
        var value = new Dictionary<string, object?>
        {
            ["value1"] = 1,
            ["Value2"] = "Value 2",
            ["value3"] = null,
        };

        _subject.Enrich("value", value);
        _subject.Get<int>("value.value1").ShouldBe(1);
        _subject.Get<string>("value.Value2").ShouldBe("Value 2");
        _subject.Get<object?>("value.value3").ShouldBe(null);
    }

    [Test]
    public void WhenDateTimeOffset_ShouldBeOk()
    {
        var value = DateTimeOffset.UtcNow;
        _subject.Enrich("value", value);
        _subject.Get<DateTimeOffset>("value").ShouldBe(value);
    }

    [Test]
    public void WhenDateTime_ShouldBeOk()
    {
        var value = DateTime.UtcNow;
        _subject.Enrich("value", value);
        _subject.Get<DateTime>("value").ShouldBe(value);
    }

    [Test]
    public void WhenGuid_ShouldBeOk()
    {
        var value = Guid.NewGuid();
        _subject.Enrich("value", value);
        _subject.Get<Guid>("value").ShouldBe(value);
    }

    [Test]
    public void WhenTimeSpan_ShouldBeOk()
    {
        var value = TimeSpan.FromMinutes(30);
        _subject.Enrich("value", value);
        _subject.Get<TimeSpan>("value").ShouldBe(value);
    }

    [Test]
    public void WhenDateOnly_ShouldBeOk()
    {
        var value = DateOnly.FromDateTime(DateTime.Today);
        _subject.Enrich("value", value);
        _subject.Get<DateOnly>("value").ShouldBe(value);
    }

    [Test]
    public void WhenTimeOnly_ShouldBeOk()
    {
        var value = TimeOnly.FromDateTime(DateTime.Today);
        _subject.Enrich("value", value);
        _subject.Get<TimeOnly>("value").ShouldBe(value);
    }

    [Test]
    public void WhenUri_ShouldBeOk()
    {
        var value = new Uri("https://localhost:3000/api/activity");
        _subject.Enrich("value", value);
        _subject.Get<Uri>("value").ShouldBe(value);
    }

    [Test]
    public void WhenRootEnrichAnonymousType_PropertyOnly_ShouldBeOk()
    {
        var value = new
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTimeOffset.UtcNow,
            Gender = default(DateTimeOffset?),
        };

        _subject.Enrich(value);
        _subject.Get<string>("FirstName").ShouldBe(value.FirstName);
        _subject.Get<string>("LastName").ShouldBe(value.LastName);
        _subject.Get<DateTimeOffset>("DateOfBirth").ShouldBe(value.DateOfBirth);
        _subject.Get<object?>("Gender").ShouldBe(null);
    }

    [Test]
    public void WhenRootEnrichAnonymousTypeWithObjectValues_PropertyOnly_ShouldBeOk()
    {
        var value = new
        {
            DateOfBirth = DateTimeOffset.UtcNow,
            Name = new
            {
                FirstName = "James",
                LastName = "Doe",
            },
            Children = new[]
            {
                new { Name = "Antony" },
                new { Name = "Joe" },
            },
        };

        _subject.Enrich(value);
        _subject.Get<DateTimeOffset>("DateOfBirth").ShouldBe(value.DateOfBirth);
        _subject.Get<string>("Name.FirstName").ShouldBe(value.Name.FirstName);
        _subject.Get<string>("Name.LastName").ShouldBe(value.Name.LastName);
        _subject.Get<string>("Children[0].Name").ShouldBe("Antony");
        _subject.Get<string>("Children[1].Name").ShouldBe("Joe");
    }

    [Test]
    public void WhenRootEnrichWithSetOnlyProperties_ShouldBeOk()
    {
        var value = new ClassWithSetOnlyProperties
        {
            Name = "James Doe",
            PrivateGet = "Secret Value",
        };

        _subject.Enrich(value);
        _subject.Get<string>("Name").ShouldBe(value.Name);
        _subject.Get<string?>("PrivateGet").ShouldBe(null);
        _subject.Get<string?>("NoGet").ShouldBe(null);
        _subject.Get<string?>("Static").ShouldBe(null);
        _subject.Get<string?>("_noGetValue").ShouldBe(null);
        _subject.Get<string?>("noGetValue").ShouldBe(null);
    }

    [Test]
    public void WhenRootEnrichWithDictionary_ShouldBeOk()
    {
        var id = Guid.NewGuid();
        var dateOfBirth = DateTimeOffset.UtcNow;
        var joeDateOfBirth = DateTimeOffset.UtcNow.AddDays(-1);
        var antonyDateOfBirth = DateTimeOffset.UtcNow.AddDays(-53);

        var value = new Dictionary<string, object?>
        {
            ["Name"] = new Dictionary<string, object?>
            {
                ["FirstName"] = "James",
                ["LastName"] = "Doe"
            },
            ["Children"] = new[]
            {
                new Dictionary<string, object?> { ["Name"] = "Antony", ["DateOfBirth"] = antonyDateOfBirth },
                new Dictionary<string, object?> { ["Name"] = "Joe", ["DateOfBirth"] = joeDateOfBirth },
            },
            ["DateOfBirth"] = dateOfBirth,
            ["Id"] = id,
            ["Null"] = null,
        };

        _subject.Enrich(value);
        _subject.Get<string>("Name.FirstName").ShouldBe("James");
        _subject.Get<string>("Name.LastName").ShouldBe("Doe");
        _subject.Get<string>("Children[0].Name").ShouldBe("Antony");
        _subject.Get<DateTimeOffset>("Children[0].DateOfBirth").ShouldBe(antonyDateOfBirth);
        _subject.Get<string>("Children[1].Name").ShouldBe("Joe");
        _subject.Get<DateTimeOffset>("Children[1].DateOfBirth").ShouldBe(joeDateOfBirth);
        _subject.Get<DateTimeOffset>("DateOfBirth").ShouldBe(dateOfBirth);
        _subject.Get<Guid>("Id").ShouldBe(id);
        _subject.Get<object?>("Null").ShouldBe(null);
    }

    [SetUp]
    public void SetUp()
    {
        _subject = new ActivityValues();
    }

    private class ClassWithSetOnlyProperties
    {
        private string? _noGetValue;

        public static string Static { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; init; }
        public string PrivateGet { private get; init; }

        public string? NoGet
        {
            init => _noGetValue = value;
        }
    }
}