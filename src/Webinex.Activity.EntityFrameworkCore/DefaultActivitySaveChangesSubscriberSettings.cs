namespace Webinex.Activity.EntityFrameworkCore;

public interface IDefaultActivitySaveChangesConfiguration
{
    string Kind { get; }
    string ValuePath { get; }
    bool SkipEqualValues { get; }

    IDefaultActivitySaveChangesConfiguration UseKind(string kind);
    IDefaultActivitySaveChangesConfiguration UseValuePath(string path);
    IDefaultActivitySaveChangesConfiguration UseSkipEqualValues(bool value);
}

internal class DefaultActivitySaveChangesSubscriberSettings : IDefaultActivitySaveChangesConfiguration
{
    public string Kind { get; private set; } = "DataChange";
    public string ValuePath { get; private set; } = "$dataChange";
    public bool SkipEqualValues { get; private set; } = true;

    public IDefaultActivitySaveChangesConfiguration UseKind(string kind)
    {
        Kind = kind ?? throw new ArgumentNullException(kind);
        return this;
    }

    public IDefaultActivitySaveChangesConfiguration UseValuePath(string path)
    {
        ValuePath = path ?? throw new ArgumentNullException(path);
        return this;
    }

    public IDefaultActivitySaveChangesConfiguration UseSkipEqualValues(bool value)
    {
        SkipEqualValues = value;
        return this;
    }
}