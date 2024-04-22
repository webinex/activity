using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenEntityAdded : InterceptorTestBase
{
    private User _user = null!;

    private EntityChange Result => ProcessCalls.Single().Single();
    
    [Test]
    public void ShouldBeOneCall()
    {
        ProcessCalls.Count.Should().Be(1);
    }

    [Test]
    public void ShouldMatchExpectedValue()
    {
        var expected = User.EntityChange(
            EntityChangeType.Added,
            _user.Id,
            _user.Values(),
            null);

        Result.Should().BeEquivalentTo(expected);
    }

    [SetUp]
    public async Task SetUp()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        _user = User.Random();
        dbContext.Add(_user);
        await dbContext.SaveChangesAsync();
    }
}