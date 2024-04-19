using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;
using Webinex.Activity.EntityFrameworkCore.Tests.Extensions;

namespace Webinex.Activity.EntityFrameworkCore.Tests.InterceptorTests;

public class WhenRegularPropertyChange : InterceptorUpdateTestBase
{
    private string _newName = null!;

    protected override void OnSetup()
    {
        _newName = Guid.NewGuid().ToString();
    }

    protected override void OnUpdate(User user)
    {
        user.SetName(_newName);
    }

    protected override IDictionary<string, object?> ExpectedValues()
    {
        var values = InStorageInitialValues.CloneValues();
        values["Name"] = _newName;
        return values;
    }
}