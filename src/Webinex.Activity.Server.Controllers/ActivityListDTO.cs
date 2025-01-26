using System;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.Activity.Server.Controllers;

public class ActivityListDTO
{
    public ActivityListDTO(IEnumerable<ActivityDTO> items, int total)
    {
        Items = items?.ToArray() ?? throw new ArgumentNullException(nameof(items));
        Total = total;
    }

    public ActivityDTO[] Items { get; }
    public int Total { get; }
}