using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.Activity.Server.EfCore;
using Webinex.Asky;

namespace Webinex.Activity.Server.Controllers
{
    internal class ActivityReadResult
    {
        public ActivityReadResult(int total, ActivityRow[] rows)
        {
            Total = total;
            Rows = rows;
        }

        public int Total { get; }
        public ActivityRow[] Rows { get; }
    }

    internal interface IActivityReadService
    {
        Task<ActivityReadResult> GetAllAsync(
            FilterRule? filterRule,
            SortRule sortRule,
            PagingRule pagingRule,
            bool includeTotal);
    }

    internal class ActivityReadService : IActivityReadService
    {
        private readonly IActivityDbContext _dbContext;
        private readonly IAskyFieldMap<ActivityRow> _fieldMap;

        public ActivityReadService(
            IActivityDbContext dbContext,
            IAskyFieldMap<ActivityRow> fieldMap)
        {
            _dbContext = dbContext;
            _fieldMap = fieldMap;
        }

        public async Task<ActivityReadResult> GetAllAsync(
            FilterRule? filterRule,
            SortRule sortRule,
            PagingRule pagingRule,
            bool includeTotal)
        {
            filterRule = filterRule?.Replace(new ActivityValueFilterRuleReplaceVisitor());

            if (sortRule.FieldId.StartsWith(ActivityRowAskyFieldMap.VALUE_FIELD_ID_PREFIX))
                throw new InvalidOperationException("Sorting by activity value is not supported");

            var queryable = _dbContext.Activities.AsQueryable().IgnoreQueryFilters();

            if (filterRule != null)
                queryable = queryable.Where(_fieldMap, filterRule);

            var total = includeTotal ? await queryable.CountAsync() : -1;

            var rows = await queryable
                .SortBy(_fieldMap, sortRule)
                .PageBy(pagingRule)
                .AsNoTracking()
                .ToArrayAsync();

            var ids = rows.Select(x => x.Id).ToArray();
            var values = await _dbContext.ActivityValues.AsNoTracking().Where(x => ids.Contains(x.ActivityId)).ToArrayAsync();
            var valuesById = values.ToLookup(x => x.ActivityId);

            // TODO: s.skalaban, rewrite to not mutate original object
            foreach (var activityRow in rows)
            {
                activityRow.Values = valuesById[activityRow.Id].ToArray();
            }
            
            return new ActivityReadResult(total, rows);
        }
    }
}