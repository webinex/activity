using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Webinex.Activity.Server.EfCore
{
    internal class ActivityDbFactory : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActivityDbFactory> _logger;
        private readonly ActivityDbContextSettings _settings;

        public ActivityDbFactory(
            IServiceProvider serviceProvider,
            ILogger<ActivityDbFactory> logger,
            ActivityDbContextSettings settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ActivityDbContext>();
                new TableFactory(_logger, dbContext, _settings).EnsureCreated();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal exception had happened");
                Environment.Exit(151369);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private class TableFactory
        {
            private readonly ILogger _logger;
            private readonly ActivityDbContext _dbContext;
            private readonly ActivityDbContextSettings _settings;

            public TableFactory(
                ILogger logger,
                ActivityDbContext dbContext,
                ActivityDbContextSettings settings)
            {
                _logger = logger;
                _dbContext = dbContext;
                _settings = settings;
            }

            public void EnsureCreated()
            {
                _logger.LogInformation("Checking database for tables...");
                _dbContext.Database.ExecuteSqlRaw(Sql);
                _logger.LogInformation("Tables exists or created");
            }

            private string Sql => $@"
IF OBJECT_ID(N'{_settings.Schema}.{_settings.ActivityTableName}',N'U') IS NULL
BEGIN
CREATE TABLE {_settings.Schema}.{_settings.ActivityTableName} (
	[Id] [int] IDENTITY(1, 1) NOT NULL,
    [Uid] [nvarchar](50) NOT NULL,
    [Kind] [nvarchar](50) NOT NULL,
    [OperationUid] [nvarchar](50) NOT NULL,
    [UserId] [nvarchar](50) NULL,
    [Success] [bit] NOT NULL,
    [PerformedAt] [datetimeoffset] NOT NULL,
    [ParentUid] [nvarchar](50) NULL,
    [System] [bit] NOT NULL,
CONSTRAINT [PK_{_settings.ActivityTableName}] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ) 
WITH (
    PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 
ON [PRIMARY]
END;






IF OBJECT_ID(N'{_settings.Schema}.{_settings.ActivityValueTableName}',N'U') IS NULL
BEGIN
CREATE TABLE {_settings.Schema}.{_settings.ActivityValueTableName} (
	[Id] [int] IDENTITY(1, 1) NOT NULL,
    [ActivityId] [int] NOT NULL,
    [Path] [nvarchar](4000) NOT NULL,
    [SearchPath] [nvarchar](4000) NOT NULL,
    [Kind] [int] NOT NULL,
    [Value] [nvarchar](max) NULL,
CONSTRAINT FK_{_settings.ActivityValueTableName}_ActivityId_{_settings.ActivityTableName} FOREIGN KEY (ActivityId) REFERENCES {_settings.Schema}.{_settings.ActivityTableName} (Id),
CONSTRAINT [PK_{_settings.ActivityValueTableName}] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ) 
WITH (
    PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END;
";}
    }
}