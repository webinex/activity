using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsOneReplacedButNotChanged : InterceptorUpdateTestBase
{
    protected override bool IsValuesEqual => true;

    protected override void OnUpdate(User user)
    {
        user.ReplacePrimaryPhone(new Phone(user.Contact.PrimaryPhone.Value));
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        return InStorageInitialValues.CloneValues();
    }
}