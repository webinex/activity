using System;
using System.Collections.Generic;

namespace Webinex.Activity.Server.EfCore
{
    public class ActivityRow
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public string Kind { get; set; }
        public string OperationUid { get; set; }
        public string UserId { get; set; }
        public bool Success { get; set; }
        public DateTimeOffset PerformedAt { get; set; }
        public bool System { get; set; }
        public string ParentUid { get; set; }
        public virtual ICollection<ActivityValueRow> Values { get; set; }
    }
}