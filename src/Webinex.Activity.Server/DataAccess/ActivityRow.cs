using System.Text.Json;

namespace Webinex.Activity.Server.DataAccess;

public class ActivityRow : ActivityRowBase
{
    public JsonElement? Values { get; protected set; }
    
    protected ActivityRow()
    {
    }

    protected ActivityRow(IActivityValue value) : base(value)
    {
        Values = value.Values.AsJsonElement();
    }
}