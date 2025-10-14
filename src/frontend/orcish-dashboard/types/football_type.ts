export type League = {
    league_id: number;
    league_name: string;
    nation: string;
}

export type Club = {
  club_id: number;
  club_name: string;
  nation: string;
  league_id: number;
  league_name: string;
};

export type Player = {
  player_id: number;
  player_name: string;
  nation: string;
  position: string;
  age: string;
  match_played: number;
  starts: number;
  minutes: number;
  ninety_minutes: string;
  goals: number;
  assists: number;
  goals_assists: number;
  non_penalty_goals: number;
  penalty_kicks_made: number;
  penalty_kicks_attempted: number;
  yellow_cards: number;
  red_cards: number;
  goals_per_90s: string;
  assists_per_90s: string;
  goals_assists_per_90s: string;
  non_penalty_goals_per_90s: string;
  non_penalty_goals_assists_per_90s: string;
  player_ref_id: string;
  season: string;
  club_id: number;
  club_name: string;
};

export type UrlInformation = {
  label: string;
  url: string;
  league: League[];
  status_code: number;
  status: string;
  season: string;
}

export type PlayerSeasonComparison = {
  playerId: number;
  playerName: string;
  currentSeason: string;
  previousSeason: string;
  currentGoals: number;
  currentAssists: number;
  currentAppearances: number;
  currentMinutesPlayed: number;
  previousGoals: number;
  previousAssists: number;
  previousAppearances: number;
  previousMinutesPlayed: number;
  goalsDifference: number;
  assistsDifference: number;
  appearancesDifference: number;
  goalsChangePercentage: number;
  assistsChangePercentage: number;
  appearancesChangePercentage: number;
  currentGoalsPer90: number;
  previousGoalsPer90: number;
  goalsPer90Difference: number;
  performanceTrend: string;
}

export type Goalkeeping = {
  goal_keeping_id: number;
  player_name: string;
  nation: string;
  position: string;
  age: string;
  match_played: number;
  starts: number;
  minutes: string;
  nineteen_minutes: string;
  goals_against: number;
  goals_assists_per_90s: string;
  shots_on_target_against: string;
  saves: string;
  save_percentage: string;
  wins: number;
  draws: number;
  losses: number;
  clean_sheets: number;
  clean_sheets_percentage: string;
  penalty_kicks_attempted: string;
  penalty_kicks_allowed: string;
  penalty_kicks_saved: string;
  penalty_kicks_missed: string;
  penalty_kicks_saved_percentage: string;
  season: string;
  player_id: number;
  player: Player;
  player_ref_id: string;
}

export type Shooting = {
  shooting_id: number;
  player_name: string;
  nation: string;
  position: string;
  age: string;
  nineteen_minutes: string;
  goals: number;
  shots_total: number;
  shots_on_target: number;
  shots_on_target_percentage: string;
  shots_total_per_90: string;
  shots_on_target_90: string;
  goals_shot: string;
  goals_shots_on_target: string;
  average_shot_distance: string;
  penalty_kicks_made: number;
  penalty_kicks_attempted: number;
  season: string;
  player_id: number;
  player: Player;
  player_ref_id: string;
}

export interface ClubTrend {
  season: string;
  totalGoals: number;
  totalGoalsAgainst: number;
  totalAssists: number;
}

export type MatchLog = {

}

export type CompleteTeamDataDto = {
  team_name: string;
  team_id: string;
  source_url: string;
  extracted_at: string;
  players: Player[];
  goalkeeping: Goalkeeping[];
  shooting: Shooting[];
  match_logs: MatchLog[];
  raw_html?: string;
}

export type EnhancedTeamDataResponse = {
  data: CompleteTeamDataDto;
  downloadLinks: {
    json: string;
    zip: string;
  };
}

export interface CrawlAllDataRequest {
  url: string;
  id: string;
}