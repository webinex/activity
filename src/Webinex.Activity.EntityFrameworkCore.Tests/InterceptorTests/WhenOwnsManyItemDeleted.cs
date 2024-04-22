using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsManyItemDeleted : InterceptorUpdateTestBase
{
    protected override void OnUpdate(User user)
    {
        user.RemoveContact(0);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        values["Contacts"] = values["Contacts"].AsValuesArray().Skip(1).ToArray();
        return values;
    }
}