using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Webinex.Activity.Server.Controllers
{
    public class ActivityListDto
    {
        public ActivityListDto([NotNull] IEnumerable<ActivityDto> items, int total)
        {
            Items = items?.ToArray() ?? throw new ArgumentNullException(nameof(items));
            Total = total;
        }

        public ActivityDto[] Items { get; }
        public int Total { get; }
    }
}