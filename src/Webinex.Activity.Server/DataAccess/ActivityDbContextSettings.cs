using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.Server.DataAccess;

public record ActivityDbContextSettings(
    DbContextOptions Options,
    string Schema,
    string ActivityTableName);