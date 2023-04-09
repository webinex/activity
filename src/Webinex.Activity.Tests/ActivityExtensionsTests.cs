using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Webinex.Activity.Tests;

public class ActivityExtensionsTests
{
    private IActivityScope _subject;
    
    [Test]
    public void All_WhenEmpty_ShouldReturnEmptyActivities()
    {
        var activities = _subject.All();
        activities.ShouldBeEmpty();
    }
    
    [Test]
    public void All_WhenHas1LevelChildren_ShouldReturn1Activity()
    {
        _subject.Push("OneLevelChildren");
        
        var activities = _subject.All();
        activities.Length.ShouldBe(1);
    }
    
    [Test]
    public void All_WhenHas2LevelChildren_ShouldReturn2Activities()
    {
        _subject.Push("OneLevelChildren");
        _subject.Push("TwoLevelChildren");
        
        var activities = _subject.All();
        activities.Length.ShouldBe(2);
    }

    [SetUp]
    public void SetUp()
    {
        var activityStoreMock = new Mock<IActivityStore>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        _subject = new ActivityScope(
            activityStoreMock.Object,
            loggerFactoryMock.Object,
            Array.Empty<IActivityContextInitializer>(),
            new NullActivityIncomeContext());
    }
}