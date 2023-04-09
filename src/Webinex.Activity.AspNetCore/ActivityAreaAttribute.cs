using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ActivityAreaAttribute : Attribute
    {
        public ActivityAreaAttribute([NotNull] string area)
        {
            Area = area ?? throw new ArgumentNullException(nameof(area));
        }
            
        public string Area { get; }
    }
}