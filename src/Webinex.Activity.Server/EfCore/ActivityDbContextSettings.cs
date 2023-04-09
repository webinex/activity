using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.EfCore
{
    public class ActivityDbContextSettings
    {
        public ActivityDbContextSettings(
            DbContextOptions options,
            string schema,
            string activityTableName,
            string activityValueTableName)
        {
            Options = options;
            Schema = schema;
            ActivityTableName = activityTableName;
            ActivityValueTableName = activityValueTableName;
        }

        public string Schema { get; }

        public string ActivityTableName { get; }

        public string ActivityValueTableName { get; }

        public DbContextOptions Options { get; }
    }
}