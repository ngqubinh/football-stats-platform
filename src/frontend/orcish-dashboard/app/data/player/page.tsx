"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Drawer,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerDescription,
  DrawerFooter,
  DrawerClose,
  DrawerTrigger,
} from "@/components/ui/drawer";
import { Separator } from "@/components/ui/separator";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { AppSidebar } from "@/components/app-sidebar";
import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";
import { SiteHeader } from "@/components/site-header";
import { useIsMobile } from "@/hooks/use-mobile";
import { useFootball } from "@/hooks/useFootball";
import { Club, League, Player } from "@/types/football_type";

export default function PlayerPage() {
  const isMobile = useIsMobile();
  const {
    leagues,
    clubs,
    players,
    comparedSeasonPlayer,
    setSelectedLeagueId,
    setSelectedClubId,
    setSeletedPlayerRefId,
    status,
  } = useFootball();
  const [view, setView] = useState<"leagues" | "clubs" | "players">("leagues");
  const [selectedItem, setSelectedItem] = useState<
    League | Club | Player | null
  >(null);
  const [searchQuery, setSearchQuery] = useState("");

  const handleLeagueClick = (league: League) => {
    setSelectedLeagueId(league.league_id);
    setView("clubs");
  };

  const handleClubClick = (club: Club) => {
    setSelectedClubId(club.club_id);
    setView("players");
  };

  const handlePlayerDrawerOpen = (player: Player) => {
    setSelectedItem(player);
    setSeletedPlayerRefId(player.player_ref_id);
  };

  const handleDrawerClose = () => {
    setSelectedItem(null);
    setSeletedPlayerRefId(null);
  };

  const handleBack = () => {
    if (view === "players") {
      setSelectedClubId(null);
      setView("clubs");
    } else if (view === "clubs") {
      setSelectedLeagueId(null);
      setView("leagues");
    }
  };

  // Filter players based on search query
  const filteredPlayers = players.filter((player) =>
    player.player_name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <SidebarProvider
      style={
        {
          "--sidebar-width": "calc(var(--spacing) * 72)",
          "--header-height": "calc(var(--spacing) * 12)",
        } as React.CSSProperties
      }
    >
      <AppSidebar variant="inset" />
      <SidebarInset>
        <SiteHeader />
        <div className="flex flex-1 flex-col">
          <div className="@container/main flex flex-1 flex-col gap-2">
            <div className="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
              <div className="px-4 lg:px-6">
                <h1 className="text-2xl font-bold">
                  {view === "leagues"
                    ? "Leagues"
                    : view === "clubs"
                    ? "Clubs"
                    : "Players"}
                </h1>
                {view !== "leagues" && (
                  <Button
                    variant="outline"
                    onClick={handleBack}
                    className="w-fit mt-2"
                  >
                    Back to {view === "players" ? "Clubs" : "Leagues"}
                  </Button>
                )}
                {view === "players" && (
                  <div className="mt-4">
                    <Input
                      type="text"
                      placeholder="Search players by name..."
                      value={searchQuery}
                      onChange={(e) => setSearchQuery(e.target.value)}
                      className="max-w-md"
                    />
                  </div>
                )}
                {status === "loading" && (
                  <div className="flex justify-center items-center py-8">
                    <div className="text-muted-foreground">Loading...</div>
                  </div>
                )}
                {status === "failed" && (
                  <div className="text-red-500">
                    Error loading data. Please try again.
                  </div>
                )}
                {status === "succeeded" &&
                  view === "leagues" &&
                  leagues.length === 0 && (
                    <div className="text-muted-foreground">No leagues found.</div>
                  )}
                {status === "succeeded" &&
                  view === "leagues" &&
                  leagues.length > 0 && (
                    <div className="rounded-md border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Nation</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {leagues.map((league) => (
                            <TableRow key={league.league_id}>
                              <TableCell>
                                <Button
                                  variant="link"
                                  className="text-foreground w-fit px-0 text-left"
                                  onClick={() => handleLeagueClick(league)}
                                >
                                  {league.league_name}
                                </Button>
                              </TableCell>
                              <TableCell>{league.nation}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  )}
                {status === "succeeded" &&
                  view === "clubs" &&
                  clubs.length === 0 && (
                    <div className="text-muted-foreground">No clubs found.</div>
                  )}
                {status === "succeeded" &&
                  view === "clubs" &&
                  clubs.length > 0 && (
                    <div className="rounded-md border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Nation</TableHead>
                            <TableHead>League</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {clubs.map((club) => (
                            <TableRow key={club.club_id}>
                              <TableCell>
                                <Button
                                  variant="link"
                                  className="text-foreground w-fit px-0 text-left"
                                  onClick={() => handleClubClick(club)}
                                >
                                  {club.club_name}
                                </Button>
                              </TableCell>
                              <TableCell>{club.nation}</TableCell>
                              <TableCell>{club.league_name}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  )}
                {status === "succeeded" &&
                  view === "players" &&
                  filteredPlayers.length === 0 && (
                    <div className="text-muted-foreground">No players found.</div>
                  )}
                {status === "succeeded" &&
                  view === "players" &&
                  filteredPlayers.length > 0 && (
                    <div className="rounded-md border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Nation</TableHead>
                            <TableHead>Position</TableHead>
                            <TableHead>Club</TableHead>
                            <TableHead>Goals</TableHead>
                            <TableHead>Goals-90</TableHead>
                            <TableHead>Assists</TableHead>
                            <TableHead>Assists-90</TableHead>
                            <TableHead>Matches</TableHead>
                            <TableHead>Starts</TableHead>
                            <TableHead>Minutes</TableHead>
                            <TableHead>90s</TableHead>
                            <TableHead>GA</TableHead>
                            <TableHead>GA-90</TableHead>
                            <TableHead>Non PG</TableHead>
                            <TableHead>PK</TableHead>
                            <TableHead>PK ATT</TableHead>
                            <TableHead>CrdY</TableHead>
                            <TableHead>CrdR</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {filteredPlayers.map((player) => (
                            <TableRow key={player.player_id}>
                              <TableCell>
                                <Drawer
                                  direction={isMobile ? "bottom" : "right"}
                                  onOpenChange={(open) => {
                                    if (open) {
                                      handlePlayerDrawerOpen(player);
                                    } else {
                                      handleDrawerClose();
                                    }
                                  }}
                                >
                                  <DrawerTrigger asChild>
                                    <Button
                                      variant="link"
                                      className="text-foreground w-fit px-0 text-left"
                                    >
                                      {player.player_name}
                                    </Button>
                                  </DrawerTrigger>
                                  <DrawerContent>
                                    <DrawerHeader className="gap-1">
                                      <DrawerTitle>{player.player_name}</DrawerTitle>
                                      <DrawerDescription>
                                        Player Details
                                      </DrawerDescription>
                                    </DrawerHeader>
                                    <div className="flex flex-col gap-4 overflow-y-auto px-4 text-sm">
                                      <div className="grid gap-2">
                                        <div className="font-medium">
                                          Player ID: {player.player_id}
                                        </div>
                                        <div className="font-medium">
                                          Player Ref ID: {player.player_ref_id}
                                        </div>
                                        <div className="font-medium">
                                          Nation: {player.nation}
                                        </div>
                                        <div className="font-medium">
                                          Club: {player.club_name}
                                        </div>
                                        <div className="font-medium">
                                          Position: {player.position}
                                        </div>
                                        <div className="font-medium">
                                          Age: {player.age}
                                        </div>
                                        <div className="font-medium">
                                          Season: {player.season}
                                        </div>
                                        <div className="text-muted-foreground">
                                          This is a player from {player.nation}{" "}
                                          playing for {player.club_name}.
                                        </div>
                                      </div>
                                      {!isMobile &&
                                        comparedSeasonPlayer &&
                                        comparedSeasonPlayer.length > 0 && (
                                          <>
                                            <Separator />
                                            <div className="grid gap-2">
                                              <h3 className="font-semibold">
                                                Season Comparison
                                              </h3>
                                              <div className="grid grid-cols-2 gap-4 text-xs">
                                                <div>
                                                  <div className="font-medium text-muted-foreground">
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .currentSeason
                                                    }
                                                  </div>
                                                  <div>
                                                    Goals:{" "}
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .currentGoals
                                                    }
                                                  </div>
                                                  <div>
                                                    Assists:{" "}
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .currentAssists
                                                    }
                                                  </div>
                                                  <div>
                                                    Apps:{" "}
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .currentAppearances
                                                    }
                                                  </div>
                                                  <div>
                                                    G/90:{" "}
                                                    {comparedSeasonPlayer[0].currentGoalsPer90.toFixed(
                                                      2
                                                    )}
                                                  </div>
                                                </div>
                                                <div>
                                                  <div className="font-medium text-muted-foreground">
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .previousSeason
                                                    }
                                                  </div>
                                                  <div>
                                                    Goals:{" "}
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .previousGoals
                                                    }
                                                  </div>
                                                  <div>
                                                    Assists:{" "}
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .previousAssists
                                                    }
                                                  </div>
                                                  <div>
                                                    Apps:{" "}
                                                    {
                                                      comparedSeasonPlayer[0]
                                                        .previousAppearances
                                                    }
                                                  </div>
                                                  <div>
                                                    G/90:{" "}
                                                    {comparedSeasonPlayer[0].previousGoalsPer90.toFixed(
                                                      2
                                                    )}
                                                  </div>
                                                </div>
                                              </div>
                                            </div>
                                            <Separator />
                                            <div className="grid gap-2">
                                              <div className="flex gap-2 items-center font-medium">
                                                <span>
                                                  Performance Trend:{" "}
                                                  {comparedSeasonPlayer[0]
                                                    .performanceTrend ?? "N/A"}
                                                </span>
                                                {comparedSeasonPlayer[0]
                                                  .goalsChangePercentage !==
                                                undefined ? (
                                                  comparedSeasonPlayer[0]
                                                    .goalsChangePercentage > 0 ? (
                                                    <span className="size-4 text-green-500">
                                                      ↑
                                                    </span>
                                                  ) : comparedSeasonPlayer[0]
                                                      .goalsChangePercentage <
                                                    0 ? (
                                                    <span className="size-4 text-red-500">
                                                      ↓
                                                    </span>
                                                  ) : null
                                                ) : null}
                                              </div>
                                              <div className="text-muted-foreground text-xs">
                                                Goals:{" "}
                                                {comparedSeasonPlayer[0]
                                                  .goalsChangePercentage !==
                                                undefined
                                                  ? `${
                                                      comparedSeasonPlayer[0]
                                                        .goalsChangePercentage > 0
                                                        ? "+"
                                                        : ""
                                                    }${comparedSeasonPlayer[0].goalsChangePercentage.toFixed(
                                                      1
                                                    )}%`
                                                  : "N/A"}{" "}
                                                • Assists:{" "}
                                                {comparedSeasonPlayer[0]
                                                  .assistsChangePercentage !==
                                                undefined
                                                  ? `${
                                                      comparedSeasonPlayer[0]
                                                        .assistsChangePercentage >
                                                      0
                                                        ? "+"
                                                        : ""
                                                    }${comparedSeasonPlayer[0].assistsChangePercentage.toFixed(
                                                      1
                                                    )}%`
                                                  : "N/A"}{" "}
                                                • Appearances:{" "}
                                                {comparedSeasonPlayer[0]
                                                  .appearancesChangePercentage !==
                                                undefined
                                                  ? `${
                                                      comparedSeasonPlayer[0]
                                                        .appearancesChangePercentage >
                                                      0
                                                        ? "+"
                                                        : ""
                                                    }${comparedSeasonPlayer[0].appearancesChangePercentage.toFixed(
                                                      1
                                                    )}%`
                                                  : "N/A"}
                                              </div>
                                            </div>
                                          </>
                                        )}
                                      {!isMobile &&
                                        !comparedSeasonPlayer &&
                                        selectedItem && (
                                          <div className="flex justify-center items-center py-8">
                                            <div className="text-muted-foreground">
                                              Loading comparison data...
                                            </div>
                                          </div>
                                        )}
                                    </div>
                                    <DrawerFooter>
                                      <DrawerClose asChild>
                                        <Button variant="outline">Close</Button>
                                      </DrawerClose>
                                    </DrawerFooter>
                                  </DrawerContent>
                                </Drawer>
                              </TableCell>
                              <TableCell>{player.nation}</TableCell>
                              <TableCell>{player.position}</TableCell>
                              <TableCell>{player.age}</TableCell>
                              <TableCell>{player.goals}</TableCell>
                              <TableCell>{player.goals_per_90s}</TableCell>
                              <TableCell>{player.assists}</TableCell>
                              <TableCell>{player.assists_per_90s}</TableCell>
                              <TableCell>{player.match_played}</TableCell>
                              <TableCell>{player.starts}</TableCell>
                              <TableCell>{player.minutes}</TableCell>
                              <TableCell>{player.ninety_minutes}</TableCell>
                              <TableCell>{player.goals_assists}</TableCell>
                              <TableCell>{player.goals_assists_per_90s}</TableCell>
                              <TableCell>{player.non_penalty_goals}</TableCell>
                              <TableCell>{player.non_penalty_goals_per_90s}</TableCell>
                              <TableCell>{player.penalty_kicks_made}</TableCell>
                              <TableCell>{player.penalty_kicks_attempted}</TableCell>
                              <TableCell>{player.yellow_cards}</TableCell>
                              <TableCell>{player.red_cards}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  )}
              </div>
            </div>
          </div>
        </div>
      </SidebarInset>
    </SidebarProvider>
  );
}