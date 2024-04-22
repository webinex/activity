namespace Webinex.Activity.EntityFrameworkCore;

public class EntityChange
{
    public EntityChange(EntityChangeType type, string name,
        IDictionary<string, object?>? primaryKey, IDictionary<string, object?> values,
        IDictionary<string, object?>? originalValues)
    {
        Type = type;
        PrimaryKey = primaryKey;
        Values = values;
        Name = name;
        OriginalValues = originalValues;
    }

    public EntityChangeType Type { get; }
    public string Name { get; }
    public IDictionary<string, object?>? PrimaryKey { get; }
    public IDictionary<string, object?> Values { get; }
    public IDictionary<string, object?>? OriginalValues { get; }
}