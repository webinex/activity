using System;
using System.Diagnostics.CodeAnalysis;

namespace Webinex.Activity
{
    public class ActivityValueScalar
    {
        public ActivityValueScalar([NotNull] string path, ActivityValueKind kind, [MaybeNull] string value)
        {
            Path = new ActivityValueScalarPath(path);
            Kind = kind;
            Value = value;
        }

        public ActivityValueScalarPath Path { get; }

        public ActivityValueKind Kind { get; }

        public string Value { get; }

        public object GetValue()
        {
            return Kind switch
            {
                ActivityValueKind.Number when Value.Contains(".") => decimal.Parse(Value),
                ActivityValueKind.Number => int.Parse(Value),
                ActivityValueKind.String => Value,
                ActivityValueKind.Boolean => bool.Parse(Value),
                ActivityValueKind.Null => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}