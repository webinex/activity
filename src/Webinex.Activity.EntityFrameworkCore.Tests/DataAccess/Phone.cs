namespace Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

public class Phone
{
    public string Value { get; protected set; } = null!;
    
    public Phone(string value)
    {
        Value = value;
    }

    protected Phone()
    {
    }

    public void MutateValue(string value)
    {
        Value = value;
    }

    public static Phone Random() => new(Guid.NewGuid().ToString());

    public Phone Clone() => new(Value);

    public IDictionary<string, object?> ToDictionary()
    {
        return new Dictionary<string, object?>
        {
            [nameof(Value)] = Value,
        };
    }
}