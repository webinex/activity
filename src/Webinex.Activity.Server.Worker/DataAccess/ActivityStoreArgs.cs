using System;

namespace Webinex.Activity.Server.Worker.DataAccess
{
    public class ActivityStoreArgs
    {
        public ActivityStoreArgs(IActivityValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IActivityValue Value { get; }
    }
}