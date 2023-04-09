using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Activity.Server.EfCore;

namespace Webinex.Activity.Server.Controllers
{
    internal class DefaultActivityDtoMapper : IActivityDtoMapper
    {
        private readonly IActivityUserNameProvider _activityUserNameProvider;

        public DefaultActivityDtoMapper(IActivityUserNameProvider activityUserNameProvider)
        {
            _activityUserNameProvider = activityUserNameProvider;
        }

        public async Task<ActivityDto[]> MapManyAsync(IEnumerable<ActivityRow> rowsEnumerable)
        {
            var rows = rowsEnumerable?.ToArray() ?? throw new ArgumentNullException(nameof(rowsEnumerable));
            var userIds = rows.Select(x => x.UserId).Where(id => id != null).Distinct().ToArray();
            var userNamesById = await _activityUserNameProvider.GetUserNamesByIdAsync(userIds);

            return rows.Select(row =>
            {
                if (row.UserId == null || !userNamesById.TryGetValue(row.UserId, out var name))
                    name = null;

                return MapOne(row, name);
            }).ToArray();
        }

        private ActivityDto MapOne(ActivityRow row, string name)
        {
            var valuesScalars = row.Values.Select(x => x.ToScalar()).ToArray();
            var activityValues = ActivityValues.Create(valuesScalars);

            return new ActivityDto
            {
                Id = row.Uid,
                Kind = row.Kind,
                OperationId = row.OperationUid,
                UserId = row.UserId,
                UserName = name,
                Success = row.Success,
                PerformedAt = row.PerformedAt,
                ParentId = row.ParentUid,
                Values = activityValues.AsJsonObject(),
            };
        }
    }
}