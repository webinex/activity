using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Activity.Server.EfCore;

namespace Webinex.Activity.Server.Worker.Stores.EfCore
{
    internal class DbContextActivityStore : IActivityStore
    {
        private readonly IActivityDbContext _dbContext;

        public DbContextActivityStore(IActivityDbContext dbContext)
        {
            _dbContext = dbContext;
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
            var rows = NewRows(args);
            await _dbContext.Activities.AddRangeAsync(rows);
        }

        private ActivityRow[] NewRows(ActivityStoreArgs[] args)
        {
            return args.Select(NewActivityRow).ToArray();
        }

        private ActivityRow NewActivityRow(ActivityStoreArgs args)
        {
            var value = args.Value;
            
            return new ActivityRow
            {
                Uid = value.Id,
                Kind = value.Kind,
                OperationUid = value.SystemValues.OperationId,
                Success = value.SystemValues.Success,
                UserId = value.SystemValues.UserId,
                TenantId = value.SystemValues.TenantId,
                PerformedAt = value.SystemValues.PerformedAt,
                ParentUid = value.ParentId,
                System = value.SystemValues.System,
                Values = NewActivityValueRows(value),
            };
        }

        private ActivityValueRow[] NewActivityValueRows(IActivityValue value)
        {
            var flatten = value.Values?.Flatten() ?? Array.Empty<ActivityValueScalar>();
            return flatten.Select(v => new ActivityValueRow
            {
                Path = v.Path.Value,
                SearchPath = v.Path.Pattern,
                Kind = v.Kind,
                Value = v.Value ?? throw new ArgumentNullException(),
            }).ToArray();
        }
    }
}