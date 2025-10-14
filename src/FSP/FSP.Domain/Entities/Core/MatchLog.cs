using System;

namespace FSP.Domain.Entities.Core;

public class MatchLog
{
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Competition { get; set; } = string.Empty;
    public string Round { get; set; } = string.Empty;
    public string Day { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string GoalsFor { get; set; } = string.Empty;
    public string GoalsAgainst { get; set; } = string.Empty;
    public string Opponent { get; set; } = string.Empty;
    public string Possession { get; set; } = string.Empty;
    public string Attendance { get; set; } = string.Empty;
    public string Captain { get; set; } = string.Empty;
    public string Formation { get; set; } = string.Empty;
    public string OppFormation { get; set; } = string.Empty;
    public string Referee { get; set; } = string.Empty;
}
