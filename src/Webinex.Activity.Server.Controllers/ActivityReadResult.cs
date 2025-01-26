using System;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.Activity.Server.Controllers;

public class ActivityReadResult<TActivityRow>
{
    public ActivityReadResult(int total, IEnumerable<TActivityRow> rows)
    {
        Total = total;
        Rows = rows?.ToArray() ?? throw new ArgumentNullException(nameof(rows));
    }

    public int Total { get; }
    public IReadOnlyCollection<TActivityRow> Rows { get; }
}