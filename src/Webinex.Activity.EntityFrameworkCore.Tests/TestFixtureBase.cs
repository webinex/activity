using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

namespace Webinex.Activity.EntityFrameworkCore.Tests;

public class TestFixtureBase
{
    private static readonly Guid Id = Guid.NewGuid();
    private static bool _isFirst = true;
    private static readonly object Lock = new();

    public IServiceProvider ServiceProvider { get; private set; }

    protected virtual IActivitySaveChangesSubscriber SaveChangesSubscriber =>
        new Mock<IActivitySaveChangesSubscriber>().Object;

    [OneTimeSetUp]
    public void TestBase_OneTimeSetUp()
    {
        lock (Lock)
        {
            if (_isFirst)
            {
                if (Directory.Exists("Data")) Directory.Delete("Data", true);
                Directory.CreateDirectory("Data");
                _isFirst = false;
            }
        }

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseActivitySaveChangesInterceptor()
            .UseSqlite($"DataSource=file:./Data/{Id}").Options;

        ServiceProvider = new ServiceCollection()
            .AddSingleton<IActivitySaveChangesSubscriber>(_ => SaveChangesSubscriber)
            .AddLogging()
            .AddDbContext<TestDbContext>(x => x
                .UseActivitySaveChangesInterceptor()
                .UseSqlite($"DataSource=file:./Data/{Id}"))
            .BuildServiceProvider();

        using var scope = ServiceProvider.CreateScope();
        var dbContext = ServiceProvider.GetRequiredService<TestDbContext>();
        dbContext.CreateDatabase();
    }
}