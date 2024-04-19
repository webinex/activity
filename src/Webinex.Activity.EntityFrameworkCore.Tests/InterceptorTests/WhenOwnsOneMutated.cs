using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsOneMutated : InterceptorUpdateTestBase
{
    private string _newPhoneValue = null!;

    protected override void OnSetup()
    {
        _newPhoneValue = Phone.Random().Value;
    }

    protected override void OnUpdate(User user)
    {
        user.MutatePhoneValue(_newPhoneValue);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        values["Contact"].AsValues()["PrimaryPhone"] = new Phone(_newPhoneValue).ToDictionary();
        return values;
    }
}