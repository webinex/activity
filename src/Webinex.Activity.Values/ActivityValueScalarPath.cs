using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Webinex.Activity
{
    public class ActivityValueScalarPath
    {
        public ActivityValueScalarPath([NotNull] string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        public string Value { get; }

        [NotNull]
        public string Pattern => new Regex("\\[\\d+\\]").Replace(Value, "[*]");
    }
}