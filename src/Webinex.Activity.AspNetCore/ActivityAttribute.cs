using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity.AspNetCore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ActivityAttribute : Attribute
    {
        public ActivityAttribute([NotNull] string kind, bool noArea = false)
        {
            Kind = kind ?? throw new ArgumentNullException(nameof(kind));
            NoArea = noArea;
        }

        [NotNull] public string Kind { get; }
            
        public bool NoArea { get; }
    }
}