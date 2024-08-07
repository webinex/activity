using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenComplexChange : InterceptorTestBase
{
    private User _toDelete = null!;
    private User _toUpdate = null!;
    private User _toAdd = null!;

    private IDictionary<string, object?> _initialAddedValues = null!;
    private IDictionary<string, object?> _initialToUpdateValues = null!;
    private IDictionary<string, object?> _initialToDeleteValues = null!;
    private Contact _newContact = null!;
    private Phone _newPhone = null!;

    private EntityChange[] Result => ProcessCalls.ElementAt(1).ToArray();

    [Test]
    public void ShouldBeTwoCalls()
    {
        ProcessCalls.Count.Should().Be(2);
    }

    [Test]
    public void ShouldBe3Changes()
    {
        Result.Length.Should().Be(3);
    }

    [Test]
    public void ShouldBeExpectedDeleted()
    {
        var result = Result.Single(x => x.Type == EntityChangeType.Deleted);
        var expected = User.EntityChange(EntityChangeType.Deleted, _toDelete.Id, _initialToDeleteValues,
            null);

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void ShouldBeExpectedUpdated()
    {
        var result = Result.Single(x => x.Type == EntityChangeType.Updated);

        var values = _initialToUpdateValues.CloneValues();
        values["Contacts"] = values["Contacts"].AsValuesArray().Skip(2).ToArray();
        values["Contacts"] = values["Contacts"].AsValuesArray().Concat(new[] { _newContact.ToDictionary() });
        values["Contact"].AsValues()["PrimaryPhone"] = _newPhone.ToDictionary();

        var expected = User.EntityChange(EntityChangeType.Updated, _toUpdate.Id, values,
            _initialToUpdateValues);

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void ShouldBeExpectedAdded()
    {
        var result = Result.Single(x => x.Type == EntityChangeType.Added);
        var expected = User.EntityChange(EntityChangeType.Added, _toAdd.Id, _initialAddedValues,
            null);
        result.Should().BeEquivalentTo(expected);
    }

    [SetUp]
    public async Task SetUp()
    {
        _toUpdate = User.Random(contactCount: 4);
        _toDelete = User.Random();
        _toAdd = User.Random(createdById: _toUpdate.Id);
        _initialAddedValues = _toAdd.Values();
        _newContact = Contact.Random();
        _newPhone = Phone.Random();

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            dbContext.Add(_toDelete);
            dbContext.Add(_toUpdate);
            await dbContext.SaveChangesAsync();
        }

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

            var toDeleteInStorage = await dbContext.Users.FindAsync(_toDelete.Id)
                                    ?? throw new InvalidOperationException();
            _initialToDeleteValues = toDeleteInStorage.Values();

            var toUpdateInStorage = await dbContext.Users.FindAsync(_toUpdate.Id)
                                    ?? throw new InvalidOperationException();
            _initialToUpdateValues = toUpdateInStorage.Values();

            dbContext.Remove(toDeleteInStorage);
            toUpdateInStorage.RemoveContact(0);
            toUpdateInStorage.RemoveContact(0);
            toUpdateInStorage.AddContact(_newContact);
            toUpdateInStorage.MutatePhoneValue(_newPhone.Value);
            
            _toAdd.AddRelated(toUpdateInStorage);
            dbContext.Users.Add(_toAdd);

            await dbContext.SaveChangesAsync();
        }
    }
}