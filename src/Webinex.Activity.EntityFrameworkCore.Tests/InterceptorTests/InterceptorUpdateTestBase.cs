using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

public abstract class InterceptorUpdateTestBase : InterceptorTestBase
{
    protected IDictionary<string, object?> InStorageInitialValues { get; private set; }
    
    protected virtual User SetupInitialUser() => User.Random();

    protected virtual void OnSetup()
    {
    }

    protected abstract void OnUpdate(User user);

    protected abstract IDictionary<string, object?> ExpectedValues();

    private User Initial { get; set; } = null!;
    protected User? InStorage { get; private set; }

    protected virtual bool IsValuesEqual => false;
    
    protected EntityChange Result => ProcessCalls.ElementAt(1).ElementAt(0);
    
    [Test]
    public void ShouldBeTwoCalls()
    {
        ProcessCalls.Count.Should().Be(2);
    }

    [Test]
    public void ShouldMatchExpectedValue()
    {
        var expectedValues = ExpectedValues();
        var expected =
            User.EntityChange(EntityChangeType.Updated, InStorage!.Id, expectedValues, InStorageInitialValues);
        Result.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    public void IsValuesEqual_ShouldBeExpectedValue()
    {
        Result.IsValuesEqual().Should().Be(IsValuesEqual);
    }

    [SetUp]
    public async Task InterceptorUpdateTestBase_SetUp()
    {
        OnSetup();
        Initial = SetupInitialUser();

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            dbContext.Users.Add(Initial);
            await dbContext.SaveChangesAsync();
        }

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

            InStorage = await dbContext.Users.FindAsync(Initial.Id);
            InStorage = InStorage ?? throw new InvalidOperationException();
            InStorageInitialValues = InStorage.Values();
            OnUpdate(InStorage);
            await dbContext.SaveChangesAsync();
        }
    }
}