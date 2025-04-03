using Microsoft.EntityFrameworkCore;

namespace Test.Idp.Extensions;

internal static class HostExtensions
{
    public static async Task MigrateDbAsync<TDbContext>(this IHost host)
        where TDbContext : DbContext
    {
        await using var scope = host.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await context.Database.MigrateAsync();
    }
}