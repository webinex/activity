using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Webinex.Activity.Server.DataAccess;

namespace Webinex.Activity.Server;

internal static class DefaultActivityRowFactory
{
    private static readonly ConcurrentDictionary<Type, object?> _cache = new();

    public static Func<IActivityValue, TActivityRow> GetFactory<TActivityRow>()
    {
        var type = typeof(TActivityRow);
        
        if (TryGetFactory<TActivityRow>(out var factory))
            return factory;
        
        throw new InvalidOperationException("Cannot create factory for type " + type.FullName);
    }

    public static bool TryGetFactory<TActivityRow>([NotNullWhen(true)] out Func<IActivityValue, TActivityRow>? factory)
    {
        var value = _cache.GetOrAdd(typeof(TActivityRow), key =>
        {
            if (TryGetConstructorFactory<TActivityRow>(out var result))
                return result;

            return null;
        });

        if (value != null)
        {
            factory = (Func<IActivityValue, TActivityRow>)value;
            return true;
        }

        factory = null;
        return false;
    }

    public static bool TryCreateFactory<TActivityRow>(out Func<IActivityValue, TActivityRow>? factory)
    {
        var type = typeof(TActivityRow);
        if (!type.IsClass) throw new InvalidOperationException("Type must be a class type");
        if (type.IsAbstract) throw new InvalidOperationException("Type must not be abstract");

        if (!type.IsAssignableTo(typeof(ActivityRowBase)))
            throw new InvalidOperationException("Type must be ActivityRow or it's subtype");

        if (TryGetConstructorFactory(out factory))
            return true;

        factory = null;
        return false;
    }

    private static bool TryGetConstructorFactory<TActivityRow>(out Func<IActivityValue, TActivityRow>? factory)
    {
        var type = typeof(TActivityRow);
        var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        constructors = constructors.OrderBy(x => x.IsPublic).ToArray();

        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IActivityValue))
            {
                factory = args => (TActivityRow)constructor.Invoke([args]);
                return true;
            }
        }

        factory = null;
        return false;
    }
}

internal class DefaultActivityRowFactory<TActivityRow> : IActivityRowFactory<TActivityRow>
    where TActivityRow : ActivityRowBase
{
    public Task<IReadOnlyCollection<TActivityRow>> MapAsync(IEnumerable<IActivityValue> values)
    {
        values = values?.ToArray() ?? throw new ArgumentNullException(nameof(values));
        var factory = DefaultActivityRowFactory.GetFactory<TActivityRow>();
        var result = values.Select(factory).ToArray();
        return Task.FromResult<IReadOnlyCollection<TActivityRow>>(result);
    }
} 