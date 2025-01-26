using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webinex.Activity.Server.DataAccess;
using Webinex.Asky;

namespace Webinex.Activity.Server.Controllers;

public interface IActivityReadService<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    Task<ActivityReadResult<TActivityRow>> GetAllAsync(
        FilterRule? filterRule,
        SortRule sortRule,
        PagingRule pagingRule,
        bool includeTotal);

    Task<TActivityRow?> ByUidAsync(string uid);
    Task<TActivityRow?> ByIdAsync(int id);

    Task<IReadOnlyCollection<string>> GetAllKindsAsync();
}

internal class ActivityReadService<TActivityRow> : IActivityReadService<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    private readonly IActivityDbContext<TActivityRow> _dbContext;
    private readonly IAskyFieldMap<TActivityRow> _fieldMap;

    public ActivityReadService(
        IActivityDbContext<TActivityRow> dbContext,
        IAskyFieldMap<TActivityRow> fieldMap)
    {
        _dbContext = dbContext;
        _fieldMap = fieldMap;
    }

    public async Task<ActivityReadResult<TActivityRow>> GetAllAsync(
        FilterRule? filterRule,
        SortRule sortRule,
        PagingRule pagingRule,
        bool includeTotal)
    {
        var queryable = _dbContext.Activities.AsQueryable().AsNoTracking();

        if (filterRule != null)
            queryable = queryable.Where(_fieldMap, filterRule);

        var total = includeTotal ? await queryable.CountAsync() : -1;

        var rows = await queryable
            .SortBy(_fieldMap, sortRule)
            .PageBy(pagingRule)
            .AsNoTracking()
            .ToArrayAsync();

        return new ActivityReadResult<TActivityRow>(total, rows);
    }

    public async Task<TActivityRow?> ByUidAsync(string uid)
    {
        return _dbContext.Activities.Local.FirstOrDefault(x => x.Uid == uid)
               ?? await _dbContext.Activities.AsNoTracking().FirstOrDefaultAsync(x => x.Uid == uid);
    }

    public async Task<TActivityRow?> ByIdAsync(int id)
    {
        return await _dbContext.Activities.FindAsync(id);
    }

    public async Task<IReadOnlyCollection<string>> GetAllKindsAsync()
    {
        return await _dbContext.Activities.Select(x => x.Kind).Distinct().AsNoTracking().ToArrayAsync();
    }
}