using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Webinex.Activity.Server.EfCore;

namespace Webinex.Activity.Server.Controllers
{
    public static class ActivityDtoMapperExtensions
    {
        public static async Task<ActivityDto> MapOneAsync(
            [NotNull] this IActivityDtoMapper mapper,
            [NotNull] ActivityRow row)
        {
            mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            row = row ?? throw new ArgumentNullException(nameof(row));

            var mapped = await mapper.MapManyAsync(new[] { row });
            return mapped.Single();
        }
    }
}