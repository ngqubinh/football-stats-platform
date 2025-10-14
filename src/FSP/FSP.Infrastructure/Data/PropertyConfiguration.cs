using FSP.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSP.Infrastructure.Data;

public class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> builder)
    {
        builder.HasKey(l => l.LeagueId);
        builder.Property(l => l.LeagueName).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Nation).IsRequired(false).HasMaxLength(50);
    }
}

public class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        builder.HasKey(c => c.ClubId);
        builder.Property(c => c.ClubName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Nation).IsRequired(false).HasMaxLength(50);
        builder.HasOne(c => c.League)
                .WithMany(l => l.Clubs)
                .HasForeignKey(c => c.LeagueId)
                .HasConstraintName("LeagueId")
                .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(p => p.PlayerId);
        builder.Property(p => p.PlayerName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Nation).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Position).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Age).IsRequired().HasMaxLength(10);
        builder.Property(p => p.MatchPlayed).IsRequired();
        builder.Property(p => p.Starts).IsRequired();
        builder.Property(p => p.Minutes).IsRequired();
        builder.Property(p => p.NineteenMinutes).IsRequired(false).HasMaxLength(10);
        builder.Property(p => p.Goals).IsRequired();
        builder.Property(p => p.Assists).IsRequired();
        builder.Property(p => p.GoalsAssists).IsRequired();
        builder.Property(p => p.NonPenaltyGoals).IsRequired();
        builder.Property(p => p.PenaltyKicksMade).IsRequired();
        builder.Property(p => p.PenaltyKickAttempted).IsRequired();
        builder.Property(p => p.YellowCards).IsRequired();
        builder.Property(p => p.RedCards).IsRequired();
        builder.Property(p => p.GoalsPer90s).IsRequired().HasMaxLength(10);
        builder.Property(p => p.AssistsPer90s).IsRequired().HasMaxLength(10);
        builder.Property(p => p.GoalsAssistsPer90s).IsRequired().HasMaxLength(10);
        builder.Property(p => p.NonPenaltyGoalsPer90s).IsRequired().HasMaxLength(10);
        builder.Property(p => p.NonPenaltyGoalsAssistsPer90s).IsRequired().HasMaxLength(10);
        builder.Property(p => p.Season).IsRequired();
        builder.Property(p => p.PlayerRefId).IsRequired();

        // Relationships
        builder.HasOne(p => p.Club)
            .WithMany(c => c.Players)
            .HasForeignKey(p => p.ClubId)
            .HasConstraintName("FK_Player_Club")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Goalkeeping)
            .WithOne(g => g.Player)
            .HasForeignKey<Goalkeeping>(g => g.PlayerId)
            .HasConstraintName("FK_Goalkeeping_Player")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Shooting)
            .WithOne(s => s.Player)
            .HasForeignKey<Shooting>(s => s.PlayerId)
            .HasConstraintName("FK_Shooting_Player")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.PlayerName, p.ClubId })
            .IsUnique(false);
    }
}

public class PlayerDetailsConfiguration : IEntityTypeConfiguration<PlayerDetails>
{
    public void Configure(EntityTypeBuilder<PlayerDetails> builder)
    {
        builder.HasKey(pd => pd.PlayerDetailId);

        builder.Property(pd => pd.FullName).IsRequired().HasMaxLength(100);
        builder.Property(pd => pd.OriginalName).HasMaxLength(100);
        builder.Property(pd => pd.Position).HasMaxLength(50);
        builder.Property(pd => pd.Born).HasMaxLength(50);
        builder.Property(pd => pd.Citizenship).HasMaxLength(50);
        builder.Property(pd => pd.Club).HasMaxLength(100);

        builder.HasOne(pd => pd.Player)
            .WithOne(p => p.PlayerDetails)
            .HasForeignKey<PlayerDetails>(pd => pd.PlayerId)
            .HasConstraintName("FK_PlayerDetails_Player")
            .OnDelete(DeleteBehavior.Cascade);
    }
}


public class GoalkeepingConfiguration : IEntityTypeConfiguration<Goalkeeping>
{
    public void Configure(EntityTypeBuilder<Goalkeeping> builder)
    {
        builder.HasKey(g => g.GoalkeepingId);
        builder.Property(g => g.PlayerName).IsRequired().HasMaxLength(100);
        builder.Property(g => g.Nation).IsRequired().HasMaxLength(50);
        builder.Property(g => g.Position).IsRequired().HasMaxLength(50);
        builder.Property(g => g.Age).IsRequired().HasMaxLength(10);
        builder.Property(g => g.MatchPlayed).IsRequired();
        builder.Property(g => g.Starts).IsRequired();
        builder.Property(g => g.Minutes).IsRequired().HasMaxLength(10);
        builder.Property(g => g.NineteenMinutes).IsRequired().HasMaxLength(10);
        builder.Property(g => g.GoalsAgainst).IsRequired();
        builder.Property(g => g.GoalsAssistsPer90s).IsRequired().HasMaxLength(10);
        builder.Property(g => g.ShotsOnTargetAgainst).IsRequired().HasMaxLength(10);
        builder.Property(g => g.Saves).IsRequired().HasMaxLength(10);
        builder.Property(g => g.SavePercentage).IsRequired().HasMaxLength(10);
        builder.Property(g => g.Wins).IsRequired();
        builder.Property(g => g.Draws).IsRequired();
        builder.Property(g => g.Losses).IsRequired();
        builder.Property(g => g.CleanSheets).IsRequired();
        builder.Property(g => g.CleanSheetsPercentage).IsRequired().HasMaxLength(10);
        builder.Property(g => g.PenaltyKicksAttempted).IsRequired().HasMaxLength(10);
        builder.Property(g => g.PenaltyKicksAllowed).IsRequired().HasMaxLength(10);
        builder.Property(g => g.PenaltyKicksSaved).IsRequired().HasMaxLength(10);
        builder.Property(g => g.PenaltyKicksMissed).IsRequired().HasMaxLength(10);
        builder.Property(g => g.PenaltyKicksSavedPercentage).IsRequired().HasMaxLength(10);
        builder.Property(p => p.PlayerRefId).IsRequired();
    }
}

public class ShootingConfiguration : IEntityTypeConfiguration<Shooting>
{
    public void Configure(EntityTypeBuilder<Shooting> builder)
    {
        builder.HasKey(s => s.ShootingId);
        builder.Property(s => s.PlayerName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Nation).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Position).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Age).IsRequired().HasMaxLength(10);
        builder.Property(s => s.NineteenMinutes).IsRequired().HasMaxLength(10);
        builder.Property(s => s.Goals).IsRequired();
        builder.Property(s => s.ShotsTotal).IsRequired();
        builder.Property(s => s.ShotsOnTarget).IsRequired();
        builder.Property(s => s.ShotsOnTargetPercentage).IsRequired().HasMaxLength(10);
        builder.Property(s => s.ShotsTotalPer90).IsRequired().HasMaxLength(10);
        builder.Property(s => s.ShotsOnTargetPer90).IsRequired().HasMaxLength(10);
        builder.Property(s => s.GoalsShots).IsRequired().HasMaxLength(10);
        builder.Property(s => s.GoalsShotsOnTarget).IsRequired().HasMaxLength(10);
        builder.Property(s => s.AverageShotDistance).IsRequired().HasMaxLength(10);
        builder.Property(s => s.PenaltyKicksMade).IsRequired();
        builder.Property(s => s.PenaltyKicksAttempted).IsRequired();
        builder.Property(p => p.PlayerRefId).IsRequired();
    }
}