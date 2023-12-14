using System;

namespace Webinex.Activity
{
    internal class NullActivityIncomeContext : IActivityIncomeContext
    {
        public IActivitySystemValues? SystemValues => null;
        public ActivityPathItem[] Path => Array.Empty<ActivityPathItem>();
    }
}