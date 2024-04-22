using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsManyItemAdded : InterceptorUpdateTestBase
{
    private Contact _newContact = null!;

    protected override void OnSetup()
    {
        _newContact = Contact.Random();
    }

    protected override void OnUpdate(User user)
    {
        user.AddContact(_newContact);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        values["Contacts"] = values["Contacts"].AsValuesArray().Concat(new[] { _newContact.ToDictionary() });
        return values;
    }
}