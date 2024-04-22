using Microsoft.EntityFrameworkCore;
using Moq;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

public class InterceptorTestBase : TestFixtureBase
{
    private Mock<IActivitySaveChangesSubscriber> _subscriberMock = null!;
    protected List<IEnumerable<EntityChange>> ProcessCalls { get; private set; } = null!;

    protected override IActivitySaveChangesSubscriber SaveChangesSubscriber => _subscriberMock.Object;

    [SetUp]
    public void InterceptorTestBase_SetUp()
    {
        _subscriberMock = new Mock<IActivitySaveChangesSubscriber>();
        ProcessCalls = new List<IEnumerable<EntityChange>>();

        _subscriberMock.Setup(x =>
                x.ProcessAsync(It.IsAny<DbContext>(), It.IsAny<IEnumerable<EntityChange>>(),
                    It.IsAny<CancellationToken>()))
            .Callback((DbContext _, IEnumerable<EntityChange> changeSet, CancellationToken _) =>
                ProcessCalls.Add(changeSet));
    }
}