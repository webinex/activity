using Newtonsoft.Json.Linq;
using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsOneReplaced : InterceptorUpdateTestBase
{
    private Phone _newPhone = null!;

    protected override void OnSetup()
    {
        _newPhone = Phone.Random();
    }

    protected override void OnUpdate(User user)
    {
        user.ReplacePhone(_newPhone);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        values["Contact"] = new Contact(_newPhone, InStorage!.Contact.AdditionalPhone?.Clone()).ToDictionary();
        return values;
    }
}