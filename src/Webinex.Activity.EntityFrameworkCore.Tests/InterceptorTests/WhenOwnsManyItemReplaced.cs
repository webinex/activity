using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsManyItemReplaced : InterceptorUpdateTestBase
{
    private Contact _newContact = null!;

    protected override User SetupInitialUser()
    {
        return User.Random(contactCount: 3);
    }

    protected override void OnSetup()
    {
        _newContact = Contact.Random();
    }

    protected override void OnUpdate(User user)
    {
        user.ReplaceContact(1, _newContact);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        var contacts = values["Contacts"].AsValuesArray();
        values["Contacts"] = contacts.Take(1).Concat(new[] { _newContact.ToDictionary() }).Concat(contacts.Skip(2));
        return values;
    }
}