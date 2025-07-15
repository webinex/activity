using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Webinex.Activity.Server.DataAccess;

internal class ActivityDbFactory<TActivityRow> : IHostedService
    where TActivityRow : ActivityRowBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ActivityDbFactory<TActivityRow>> _logger;
    private readonly ActivityDbContextSettings _settings;

    public ActivityDbFactory(
        IServiceProvider serviceProvider,
        ILogger<ActivityDbFactory<TActivityRow>> logger,
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
            using var dbContext = new DbContext(_settings.Options);
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
        private readonly DbContext _dbContext;
        private readonly ActivityDbContextSettings _settings;

        public TableFactory(
            ILogger logger,
            DbContext dbContext,
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
    [TenantId] [nvarchar](50) NULL,
    [Success] [bit] NOT NULL,
    [PerformedAt] [datetimeoffset] NOT NULL,
    [ParentUid] [nvarchar](50) NULL,
    [System] [bit] NOT NULL,
    [Values] [nvarchar](max) NULL,
CONSTRAINT [PK_{_settings.ActivityTableName}] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ) 
WITH (
    PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 
ON [PRIMARY]
END;
";}
}