using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server.Worker.DataAccess.EntityFrameworkCore
{
    internal class DbContextActivityStore<TActivityRow> : IActivityStore
        where TActivityRow : ActivityRowBase
    {
        private readonly IActivityDbContext<TActivityRow> _dbContext;
        private readonly IActivityRowFactory<TActivityRow> _factory;

        public DbContextActivityStore(
            IActivityDbContext<TActivityRow> dbContext,
            IActivityRowFactory<TActivityRow> factory)
        {
            _dbContext = dbContext;
            _factory = factory;
        }

        public async Task AddAsync(IEnumerable<ActivityStoreArgs> argsEnumerable)
        {
            var args = argsEnumerable?.ToArray() ?? throw new ArgumentNullException(nameof(argsEnumerable));
            if (!args.Any()) return;

            await AddInternalAsync(args);
            await _dbContext.SaveChangesAsync();
        }

        private async Task AddInternalAsync(ActivityStoreArgs[] args)
        {
            var rows = await _factory.MapAsync(args.Select(x => x.Value).ToArray());
            await _dbContext.Activities.AddRangeAsync(rows);
        }
    }
}