namespace Webinex.Activity.EntityFrameworkCore.Utils;

internal static class ValuesUtil
{
    public static bool IsEqual(IDictionary<string, object?>? x, IDictionary<string, object?>? y)
    {
        if (x == null && y == null)
            return true;
        
        if (x == null || y == null)
            return false;
        
        if (x.Count != y.Count)
            return true;

        foreach (var xKey in x.Keys)
        {
            if (!y.ContainsKey(xKey))
                return false;

            var xValue = x[xKey];
            var yValue = y[xKey];

            if (xValue == null && yValue == null)
                continue;

            if (xValue == null || yValue == null)
                return false;

            if (xValue.GetType() != yValue.GetType())
                return false;
            
            if (xValue is IDictionary<string, object?> xDictionary)
            {
                if (!IsEqual(xDictionary, (IDictionary<string, object?>)y[xKey]!))
                    return false;
                
                continue;
            }

            if (xValue is IEnumerable<IDictionary<string, object?>> xEnumerable)
            {
                xEnumerable = xEnumerable.ToArray();
                var yEnumerable = ((IEnumerable<IDictionary<string, object?>>)yValue).ToArray();
                
                if (xEnumerable.Count() != yEnumerable.Length)
                    return false;

                for (var index = 0; index < xEnumerable.Count(); index++)
                {
                    if (!IsEqual(xEnumerable.ElementAt(index), yEnumerable[index]))
                        return false;
                }
                
                continue;
            }

            if (!xValue.Equals(yValue))
                return false;
        }

        return true;
    }
}