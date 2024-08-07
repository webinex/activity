namespace Webinex.Activity;

public interface IActivityValue
{
    string Id { get; }
    string Kind { get; }
    string? ParentId { get; }
    IActivitySystemValues SystemValues { get; }
    ActivityValues Values { get; }
}