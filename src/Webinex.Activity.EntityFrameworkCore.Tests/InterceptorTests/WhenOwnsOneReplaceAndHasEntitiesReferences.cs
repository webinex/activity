using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

public class WhenOwnsOneReplaceAndHasEntitiesReferences : InterceptorTestBase
{
    private User _createdBy = null!;
    private IReadOnlyCollection<User> _related = null!;
    private User _user = null!;
    private Phone _newPhone = null!;
    private IDictionary<string, object?> _inStorageInitialValues = null!;

    [Test]
    public void ShouldBeTwoCalls()
    {
        ProcessCalls.Count.Should().Be(2);
    }

    [Test]
    public void ShouldMatchExpectedValue()
    {
        var result = ProcessCalls.ElementAt(1).Single();

        var values = _inStorageInitialValues.CloneValues();
        values["Contact"] = new Contact(_newPhone.Clone(), _user.Contact.AdditionalPhone?.Clone()).ToDictionary();

        var expected = User.EntityChange(
            EntityChangeType.Updated,
            _user.Id,
            values,
            _inStorageInitialValues);

        result.Should().BeEquivalentTo(expected);
    }

    [SetUp]
    public async Task SetUp()
    {
        _createdBy = User.Random();
        _related = Enumerable.Range(0, 2).Select(_ => User.Random()).ToArray();
        _user = User.Random(createdById: _createdBy.Id, related: _related);
        _newPhone = Phone.Random();

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            dbContext.Users.Add(_user);
            await dbContext.SaveChangesAsync();
        }

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

            var inStorage = await dbContext.Users.FindAsync(_user.Id);
            inStorage = inStorage ?? throw new InvalidOperationException();
            _inStorageInitialValues = inStorage.Values();
            inStorage.ReplacePrimaryPhone(_newPhone);
            await dbContext.SaveChangesAsync();
        }
    }
}