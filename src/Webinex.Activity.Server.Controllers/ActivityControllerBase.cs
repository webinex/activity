using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Webinex.Activity.Server.EfCore;
using Webinex.Asky;

namespace Webinex.Activity.Server.Controllers
{
    [Route("/api/activity")]
    public abstract class ActivityControllerBase : ControllerBase
    {
        [HttpGet]
        public virtual async Task<IActionResult> GetAllAsync(
            bool? includeTotal,
            [FromQuery(Name = "sort")] string sortRuleJson,
            [FromQuery(Name = "paging")] string pagingRuleJson,
            [FromQuery(Name = "filter")] string filterRuleJson)
        {
            if (!await AuthorizeAsync())
                return Unauthorized();

            var filterRule = FilterRule.FromJson(filterRuleJson, FieldMap);
            var pagingRule = PagingRule.FromJson(pagingRuleJson) ?? new PagingRule(0, 20);
            var sortRule = SortRule.FromJson(sortRuleJson) ?? new SortRule("performedAt", SortDir.Desc);

            var result = await ActivityReadService.GetAllAsync(filterRule, sortRule, pagingRule, includeTotal ?? false);

            var responseItems = await Mapper.MapManyAsync(result.Rows);
            var response = new ActivityListDto(responseItems, result.Total);
            return Ok(response);
        }

        [HttpGet("kinds")]
        public virtual async Task<IActionResult> GetKindsAsync()
        {
            var result = await DbContext.Activities.Select(x => x.Kind).Distinct().ToArrayAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetAsync(string id)
        {
            if (!await AuthorizeAsync())
                return Unauthorized();

            if (id == null)
                return BadRequest();

            var queryable = DbContext.Activities.AsQueryable();

            var row = await queryable
                .Include(x => x.Values)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Uid == id);

            if (row == null)
                return NotFound();

            var mapped = await Mapper.MapOneAsync(row);
            return Ok(mapped);
        }

        private IActivityReadService ActivityReadService =>
            HttpContext.RequestServices.GetRequiredService<IActivityReadService>();

        protected virtual IActivityDtoMapper Mapper =>
            HttpContext.RequestServices.GetRequiredService<IActivityDtoMapper>();

        protected virtual IActivityServerControllerSettings Settings =>
            HttpContext.RequestServices.GetRequiredService<IActivityServerControllerSettings>();

        protected virtual ActivityDbContext DbContext =>
            HttpContext.RequestServices.GetRequiredService<ActivityDbContext>();

        protected virtual IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        protected virtual IAuthenticationService AuthenticationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        protected virtual IAskyFieldMap<ActivityRow> FieldMap =>
            HttpContext.RequestServices.GetRequiredService<IAskyFieldMap<ActivityRow>>();

        protected virtual async Task<bool> AuthorizeAsync()
        {
            if (Settings.Policy == null)
                return true;

            var authenticationResult = await AuthenticationService.AuthenticateAsync(HttpContext, Settings.Schema);
            if (!authenticationResult.Succeeded)
                return false;

            var authorizationResult =
                await AuthorizationService.AuthorizeAsync(authenticationResult.Principal!, Settings.Policy);
            return authorizationResult.Succeeded;
        }

        internal static class Default
        {
            internal class ActivityController : ActivityControllerBase
            {
            }
        }

        internal static class Anonymous
        {
            [AllowAnonymous]
            internal class ActivityController : ActivityControllerBase
            {
            }
        }
    }
}