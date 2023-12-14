using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.EfCore;

public record ActivityDbContextSettings(
    DbContextOptions Options,
    string Schema,
    string ActivityTableName,
    string ActivityValueTableName);