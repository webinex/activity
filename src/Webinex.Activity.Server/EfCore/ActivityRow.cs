using System;
using System.Collections.Generic;

namespace Webinex.Activity.Server.EfCore
{
    public class ActivityRow
    {
        public int Id { get; set; }
        public string Uid { get; set; } = null!;
        public string Kind { get; set; } = null!;
        public string OperationUid { get; set; } = null!;
        public string? UserId { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset PerformedAt { get; set; }
        public bool System { get; set; }
        public string? ParentUid { get; set; }
        public virtual ICollection<ActivityValueRow> Values { get; set; } = new List<ActivityValueRow>();
    }
}