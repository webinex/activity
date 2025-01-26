using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server.Controllers;

internal class DefaultActivityDTOMapper<TActivityRow> : IActivityDTOMapper<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    private readonly IDefaultActivityDTOMapperSettings<TActivityRow> _settings;
    private readonly IServiceProvider _serviceProvider;

    public DefaultActivityDTOMapper(
        IDefaultActivityDTOMapperSettings<TActivityRow> settings,
        IServiceProvider serviceProvider)
    {
        _settings = settings;
        _serviceProvider = serviceProvider;
    }

    public async Task<IReadOnlyCollection<ActivityDTO>> MapManyAsync(IEnumerable<TActivityRow> rowsEnumerable)
    {
        var rows = rowsEnumerable?.ToArray() ?? throw new ArgumentNullException(nameof(rowsEnumerable));
        var extensionById = _settings.Extra != null
            ? await _settings.Extra.Invoke(_serviceProvider, rows)
            : null;

        return rows.Select(row =>
            MapOne(row, extensionById?.GetValueOrDefault(row.Uid))).ToArray();
    }

    private ActivityDTO MapOne(TActivityRow row, IDictionary<string, object?>? extra)
    {
        if (typeof(TActivityRow).IsAssignableTo(typeof(ActivityRow)) &&
            (extra == null || !extra.ContainsKey(nameof(ActivityRow.Values).ToLowerInvariant()) ||
             !extra.ContainsKey(nameof(ActivityRow.Values))))
        {
            var defaultActivityRow = (ActivityRow)(object)row;
            var dict = extra != null ? new Dictionary<string, object?>(extra) : new Dictionary<string, object?>();
            dict.Add(nameof(ActivityRow.Values).ToLowerInvariant(), defaultActivityRow.Values);
            extra = dict;
        }

        return new ActivityDTO(
            id: row.Uid,
            kind: row.Kind,
            operationId: row.OperationUid,
            tenantId: row.TenantId,
            userId: row.UserId,
            success: row.Success,
            performedAt: row.PerformedAt,
            parentId: row.ParentUid,
            extension: extra);
    }
}