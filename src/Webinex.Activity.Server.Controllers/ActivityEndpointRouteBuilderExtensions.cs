using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Webinex.Activity.Server.DataAccess;
using Webinex.Asky;

namespace Webinex.Activity.Server.Controllers;

public class ActivityEndpointMap
{
    public RouteHandlerBuilder Get { get; }
    public RouteHandlerBuilder GetAll { get; }
    public RouteHandlerBuilder GetAllKinds { get; }

    public ActivityEndpointMap(RouteHandlerBuilder get, RouteHandlerBuilder getAll, RouteHandlerBuilder getAllKinds)
    {
        Get = get ?? throw new ArgumentNullException(nameof(get));
        GetAll = getAll ?? throw new ArgumentNullException(nameof(getAll));
        GetAllKinds = getAllKinds ?? throw new ArgumentNullException(nameof(getAllKinds));
    }

    private IReadOnlyCollection<RouteHandlerBuilder> All => [GetAll, GetAllKinds];

    public void ForEach(Action<RouteHandlerBuilder> action)
    {
        foreach (var builder in All)
            action(builder);
    }
}

public static class ActivityEndpointRouteBuilderExtensions
{

    public static IEndpointRouteBuilder MapActivityApi(
        this IEndpointRouteBuilder endpoints,
        Action<ActivityEndpointMap>? configure = null)
    {
        return endpoints.MapActivityApi<ActivityRow>();
    }

    public static IEndpointRouteBuilder MapActivityApi<TActivityRow>(
        this IEndpointRouteBuilder endpoints,
        Action<ActivityEndpointMap>? configure = null)
        where TActivityRow : ActivityRowBase
    {
        var getAll = endpoints.MapGet(
                "/api/activity",
                async (
                    [FromQuery(Name = "sort")] string? sortRuleJson,
                    [FromQuery(Name = "paging")] string? pagingRuleJson,
                    [FromQuery(Name = "filter")] string? filterRuleJson,
                    [FromQuery] bool? includeTotal,
                    [FromServices] IAskyFieldMap<TActivityRow> fieldMap,
                    [FromServices] IActivityReadService<TActivityRow> readService,
                    [FromServices] IActivityDTOMapper<TActivityRow> mapper) =>
                {
                    var filterRule = FilterRule.FromJson(filterRuleJson, fieldMap);
                    var pagingRule = PagingRule.FromJson(pagingRuleJson) ?? new PagingRule(0, 20);
                    var sortRule = SortRule.FromJson(sortRuleJson) ?? new SortRule("performedAt", SortDir.Desc);

                    var result = await readService.GetAllAsync(filterRule, sortRule, pagingRule, includeTotal ?? false);

                    var responseItems = await mapper.MapManyAsync(result.Rows);
                    var response = new ActivityListDTO(responseItems, result.Total);
                    return Results.Ok(response);
                })
            .WithGroupName("Activity")
            .WithName("GetAllActivity")
            .WithOpenApi();

        var get = endpoints.MapGet(
                "/api/activity/{uid}",
                async (
                    [FromRoute] string uid,
                    [FromServices] IActivityReadService<TActivityRow> readService,
                    [FromServices] IActivityDTOMapper<TActivityRow> mapper) =>
                {
                    var row = await readService.ByUidAsync(uid);
                    if (row == null) return Results.NotFound();

                    var result = await mapper.MapManyAsync([row]);
                    return Results.Ok(result.Single());
                })
            .WithGroupName("Activity")
            .WithName("GetActivity")
            .WithOpenApi();

        var getAllKinds = endpoints.MapGet(
                "/api/activity/kinds",
                async ([FromServices] IActivityReadService<TActivityRow> readService) =>
                {
                    var result = await readService.GetAllKindsAsync();
                    return Results.Ok(result);
                })
            .WithGroupName("Activity")
            .WithName("GetAllKinds")
            .WithOpenApi();

        var map = new ActivityEndpointMap(get, getAll, getAllKinds);
        configure?.Invoke(map);

        return endpoints;
    }
}