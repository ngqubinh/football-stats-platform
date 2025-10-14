using FSP.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;

namespace FSP.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<League> Leagues { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<PlayerDetails> PlayerDetails { get; set; }
    public DbSet<Goalkeeping> Goalkeepings { get; set; }
    public DbSet<Shooting> Shootings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new LeagueConfiguration());
        builder.ApplyConfiguration(new ClubConfiguration());
        builder.ApplyConfiguration(new PlayerConfiguration());
        builder.ApplyConfiguration(new PlayerDetailsConfiguration());
        builder.ApplyConfiguration(new GoalkeepingConfiguration());
        builder.ApplyConfiguration(new ShootingConfiguration());

        base.OnModelCreating(builder);
    }
}
