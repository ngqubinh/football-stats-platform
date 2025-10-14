using FSP.Infrastructure.Data;
using FSP.Infrastructure.Persistence.Seeders;

namespace FSP.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task Initialize(ApplicationDbContext context)
	{
		await SystemSeeder.SeedLeagues(context);
	}
}
