using System;

namespace Webinex.Activity
{
    internal class ActivitySystemValues : IMutableActivitySystemValues
    {
        private readonly IActivitySystemValues? _parent;
        private string? _operationId;
        private string? _tenantId;
        private string? _userId;
        private DateTimeOffset? _performedAt;
        private bool? _success;
        private bool _frozen;
        private bool? _system;

        public ActivitySystemValues(IActivitySystemValues parent)
        {
            _parent = parent;
        }

        public ActivitySystemValues(string operationId, DateTimeOffset performedAt, string? userId, string? tenantId)
        {
            OperationId = operationId;
            PerformedAt = performedAt;
            UserId = userId;
            TenantId = tenantId;
        }

        public string OperationId
        {
            get => _operationId ?? _parent?.OperationId ?? throw new ArgumentNullException();
            set
            {
                AssertNotFrozen();
                _operationId = value;
            }
        }

        public string? TenantId
        {
            get => _tenantId ?? _parent?.TenantId;
            set
            {
                AssertNotFrozen();
                _tenantId = value;
            }
        }

        public string? UserId
        {
            get => _userId ?? _parent?.UserId;
            set
            {
                AssertNotFrozen();
                _userId = value;
            }
        }

        public DateTimeOffset PerformedAt
        {
            get => _performedAt ?? _parent?.PerformedAt ?? _performedAt!.Value;
            set
            {
                AssertNotFrozen();
                _performedAt = value;
            }
        }

        public bool Success
        {
            get => _success ?? _parent?.Success ?? true;
            set
            {
                AssertNotFrozen();
                _success = value;
            }
        }

        public bool System
        {
            get => _system ?? _parent?.System ?? false;
            set
            {
                AssertNotFrozen();
                _system = value;
            }
        }

        private void AssertNotFrozen()
        {
            if (_frozen)
                throw new InvalidOperationException("System values frozen");
        }

        public void Freeze()
        {
            AssertNotFrozen();
            _frozen = true;
        }
    }
}