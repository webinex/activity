using System;

namespace Webinex.Activity
{
    public class ActivityValueScalar
    {
        public ActivityValueScalar(string path, ActivityValueKind kind, string? value)
        {
            Path = new ActivityValueScalarPath(path);
            Kind = kind;
            Value = value;
        }

        public ActivityValueScalarPath Path { get; }

        public ActivityValueKind Kind { get; }

        public string? Value { get; }

        public object? GetValue()
        {
            return Kind switch
            {
                ActivityValueKind.Number when (Value ?? throw new ArgumentNullException()).Contains(".") =>
                    decimal.Parse(Value),
                ActivityValueKind.Number => int.Parse(Value ?? throw new ArgumentNullException()),
                ActivityValueKind.String => Value,
                ActivityValueKind.Boolean => bool.Parse(Value ?? throw new ArgumentNullException()),
                ActivityValueKind.Null => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}