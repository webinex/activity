using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

[TestFixture]
public class WhenOwnsOneDeleted : InterceptorUpdateTestBase
{
    protected override void OnUpdate(User user)
    {
        if (user.Contact.AdditionalPhone == null) 
            throw new InvalidOperationException("Additional phone might exist");
        
        user.MutateAdditionalPhone(null);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        values["Contact"].AsValues()["AdditionalPhone"] = null;
        return values;
    }
}