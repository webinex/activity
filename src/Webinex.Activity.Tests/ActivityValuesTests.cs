using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

namespace Webinex.Activity.Tests;

public class ActivityValuesTests
{
    private ActivityValues _subject;

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

    [SetUp]
    public void SetUp()
    {
        _subject = new ActivityValues();
    }
}