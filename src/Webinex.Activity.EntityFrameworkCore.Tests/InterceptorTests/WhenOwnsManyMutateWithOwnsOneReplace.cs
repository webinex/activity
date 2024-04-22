using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsManyMutateWithOwnsOneReplace : InterceptorUpdateTestBase
{
    private Phone _newPhone = null!;

    protected override User SetupInitialUser()
    {
        return User.Random(contactCount: 2);
    }

    protected override void OnSetup()
    {
        _newPhone = Phone.Random();
    }

    protected override void OnUpdate(User user)
    {
        user.MutateContactPhone(1, _newPhone);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        var contacts = values["Contacts"].AsValuesArray();
        values["Contacts"] = new[] { contacts.ElementAt(0), new Contact(_newPhone, InStorage!.Contacts.ElementAt(1).AdditionalPhone?.Clone()).ToDictionary() };
        return values;
    }
}