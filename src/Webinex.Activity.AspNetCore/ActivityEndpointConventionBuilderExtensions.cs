using Microsoft.AspNetCore.Builder;

namespace Webinex.Activity.AspNetCore;

public static class ActivityEndpointConventionBuilderExtensions
{
    public static TBuilder WithActivity<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithActivity(true);
    }
    
    public static TBuilder WithActivity<TBuilder>(this TBuilder builder, bool activity) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new ActivityEndpointMetadata(activity));
    }
    
    public static TBuilder WithActivity<TBuilder>(this TBuilder builder, string? kind) where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new ActivityEndpointMetadata(true, kind));
    }
}