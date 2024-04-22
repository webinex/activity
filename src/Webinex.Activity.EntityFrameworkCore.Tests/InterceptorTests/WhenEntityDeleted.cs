using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenEntityDeleted : InterceptorTestBase
{
    private User _user = null!;

    private EntityChange Result => ProcessCalls.ElementAt(1).Single();

    [Test]
    public void ShouldBeTwoCalls()
    {
        ProcessCalls.Count.Should().Be(2);
    }

    [Test]
    public void ShouldMatchExpectedValue()
    {
        var expected = User.EntityChange(
            EntityChangeType.Deleted,
            _user.Id,
            _user.Values(),
            _user.Values());

        Result.Should().BeEquivalentTo(expected);
    }

    [SetUp]
    public async Task SetUp()
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

            _user = User.Random();
            dbContext.Add(_user);
            await dbContext.SaveChangesAsync();
        }

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var inStorage = await dbContext.Users.FindAsync(_user.Id);
            dbContext.Remove(inStorage!);
            await dbContext.SaveChangesAsync();
        }
    }
}