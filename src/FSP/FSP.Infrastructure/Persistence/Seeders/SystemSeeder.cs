using FSP.Domain.Entities.Core;
using FSP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FSP.Infrastructure.Persistence.Seeders;

public static class SystemSeeder
{
    public static async Task SeedLeagues(ApplicationDbContext dbContext)
    {
        string[] leagues = new[]
        {
            StaticLeague.PremierLeague,
            StaticLeague.EuropaLeague,
            StaticLeague.Liga1,
        };

        var existingLeauge = await dbContext.Leagues.Select(l => l.LeagueName).ToListAsync();

        List<string> added = new();

        foreach (string league in leagues)
        {
            if (!existingLeauge.Contains(league))
            {
                var nation = StaticLeague.SystemLeagues
                    .FirstOrDefault(x => x.LeagueName.Equals(league, StringComparison.OrdinalIgnoreCase)).Nation;

                League newLeague = new League
                {
                    LeagueName = league,
                    Nation = nation
                };

                dbContext.Leagues.Add(newLeague);
                added.Add($"{league} ({nation})");
            }
        }

        if (added.Any())
        {
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Seeded leagues: {string.Join(", ", added)}");
        }
        else
        {
            Console.WriteLine("All leagues already exist.");
        }
    }
}
