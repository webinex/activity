using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Webinex.Activity
{
    public interface IActivityScopeProvider
    {
        IActivityScope? Value { get; set; }

        IActivityScope RequiredValue =>
            Value ?? throw new InvalidOperationException("Activity scope not provided");
    }

    public interface IActivityScopeAccessor
    {
        IActivityScope? Value { get; }

        IActivityScope RequiredValue =>
            Value ?? throw new InvalidOperationException("Activity scope not provided");
    }

    internal class ActivityScopeAccessor : IActivityScope, IActivityScopeAccessor, IActivityScopeProvider
    {
        private readonly AsyncLocal<IActivityScope> _value = new AsyncLocal<IActivityScope>();

        public IActivityScope? Value
        {
            get => _value.Value;
            set => _value.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IActivityScope RequiredValue =>
            Value ?? throw new InvalidOperationException("Activity scope not provided");

        public bool Initialized => Value != null;
        public IActivity? Current => RequiredValue.Current;

        public IActivityContext Context => RequiredValue.Context;

        public ActivityPathItem[] OutboundPath => RequiredValue.OutboundPath;

        public IEnumerable<IActivity> Root => RequiredValue.Root;

        public IDisposableActivity Push(string kind)
        {
            return RequiredValue.Push(kind);
        }

        public IActivityScope Add(string kind, Action<IActivity>? action = null)
        {
            return RequiredValue.Add(kind, action);
        }

        public Task CompleteAsync(bool success = true)
        {
            return RequiredValue.CompleteAsync();
        }

        public IActivityBatchValue ToBatch()
        {
            return RequiredValue.ToBatch();
        }
    }
}