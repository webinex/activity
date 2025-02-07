namespace Webinex.Activity.AspNetCore;

public class ActivityEndpointMetadata
{
    public bool Activity { get; }
    public string? Kind { get; }

    public ActivityEndpointMetadata(bool activity, string? kind = null)
    {
        Activity = activity;
        Kind = kind;
    }
}