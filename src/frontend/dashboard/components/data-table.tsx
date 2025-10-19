"use client"

import * as React from "react"
import {
  DndContext,
  KeyboardSensor,
  MouseSensor,
  TouchSensor,
  closestCenter,
  useSensor,
  useSensors,
  type DragEndEvent,
  type UniqueIdentifier,
} from "@dnd-kit/core"
import { restrictToVerticalAxis } from "@dnd-kit/modifiers"
import {
  SortableContext,
  arrayMove,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"
import {
  IconChevronDown,
  IconChevronLeft,
  IconChevronRight,
  IconChevronsLeft,
  IconChevronsRight,
  IconDotsVertical,
  IconGripVertical,
  IconLayoutColumns,
  IconRefresh,
  IconTrendingDown,
  IconTrendingUp,
} from "@tabler/icons-react"
import {
  ColumnDef,
  ColumnFiltersState,
  Row,
  SortingState,
  VisibilityState,
  flexRender,
  getCoreRowModel,
  getFacetedRowModel,
  getFacetedUniqueValues,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
} from "@tanstack/react-table"
import { Area, AreaChart, Bar, BarChart, CartesianGrid, XAxis } from "recharts"
import { toast } from "sonner"
import { z } from "zod"

import { useIsMobile } from "@/hooks/use-mobile"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerDescription,
  DrawerFooter,
  DrawerHeader,
  DrawerTitle,
  DrawerTrigger,
} from "@/components/ui/drawer"
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Separator } from "@/components/ui/separator"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@/components/ui/tabs"
import { useFootball } from "@/hooks/useFootball"
import { League, Club, Player } from "@/types/football_type"

export const schema = z.object({
  id: z.number(),
  header: z.string(),
  type: z.string(),
  status: z.string(),
  target: z.string(),
  limit: z.string(),
  reviewer: z.string(),
});

// Map League data to schema
const mapLeaguesToTableData = (leagues: League[]): z.infer<typeof schema>[] => {
  return leagues.map((league) => ({
    id: league.league_id,
    header: league.league_name,
    type: "League",
    status: "Active",
    target: league.nation,
    limit: "",
    reviewer: "Unassigned",
  }));
};

// Map Club data to schema
const mapClubsToTableData = (clubs: Club[]): z.infer<typeof schema>[] => {
  return clubs.map((club) => ({
    id: club.club_id,
    header: club.club_name,
    type: "Club",
    status: "Active",
    target: club.nation,
    limit: club.league_name,
    reviewer: "Unassigned",
  }));
};

// Map Player data to schema
const mapPlayersToTableData = (players: Player[]): z.infer<typeof schema>[] => {
  return players.map((player) => ({
    id: player.player_id,
    header: player.player_name,
    type: "Player",
    status: "Active",
    target: player.nation,
    limit: player.club_name,
    reviewer: player.position,
    player_ref_id: player.player_ref_id, 
  }));
};

// Create a separate component for the drag handle
function DragHandle({ id }: { id: number }) {
  const { attributes, listeners } = useSortable({
    id,
  })

  return (
    <Button
      {...attributes}
      {...listeners}
      variant="ghost"
      size="icon"
      className="text-muted-foreground size-7 hover:bg-transparent"
    >
      <IconGripVertical className="text-muted-foreground size-3" />
      <span className="sr-only">Drag to reorder</span>
    </Button>
  )
}

const leagueColumns: ColumnDef<z.infer<typeof schema>>[] = [
  {
    id: "drag",
    header: () => null,
    cell: ({ row }) => <DragHandle id={row.original.id} />,
  },
  {
    id: "select",
    header: ({ table }) => (
      <div className="flex items-center justify-center">
        <Checkbox
          checked={table.getIsSomePageRowsSelected()}
          onCheckedChange={() => {
            table.resetRowSelection();
          }}
          aria-label="Deselect all"
        />
      </div>
    ),
    cell: ({ row, table }) => (
      <div className="flex items-center justify-center">
        <Checkbox
          checked={row.getIsSelected()}
          onCheckedChange={(value) => {
            table.resetRowSelection();
            if (value) {
              row.toggleSelected(true);
            }
          }}
          aria-label="Select row"
        />
      </div>
    ),
    enableSorting: false,
    enableHiding: false,
  },
  {
    accessorKey: "id",
    header: "League ID",
    cell: ({ row }) => <div>{row.original.id}</div>,
  },
  {
    accessorKey: "header",
    header: "League Name",
    cell: ({ row }) => <TableCellViewer item={row.original} type="league" />,
    enableHiding: false,
  },
  {
    accessorKey: "target",
    header: "Nation",
    cell: ({ row }) => <div>{row.original.target}</div>,
  },
  {
    id: "actions",
    cell: () => (
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            className="data-[state=open]:bg-muted text-muted-foreground flex size-8"
            size="icon"
          >
            <IconDotsVertical />
            <span className="sr-only">Open menu</span>
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-32">
          <DropdownMenuItem>Edit</DropdownMenuItem>
          <DropdownMenuItem>View Clubs</DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem variant="destructive">Delete</DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    ),
  },
];

const clubColumns: ColumnDef<z.infer<typeof schema>>[] = [
  {
    id: "drag",
    header: () => null,
    cell: ({ row }) => <DragHandle id={row.original.id} />,
  },
  {
    id: "select",
    header: ({ table }) => (
      <div className="flex items-center justify-center">
        <Checkbox
          checked={table.getIsSomePageRowsSelected()}
          onCheckedChange={() => {
            table.resetRowSelection();
          }}
          aria-label="Deselect all"
        />
      </div>
    ),
    cell: ({ row, table }) => (
      <div className="flex items-center justify-center">
        <Checkbox
          checked={row.getIsSelected()}
          onCheckedChange={(value) => {
            table.resetRowSelection();
            if (value) {
              row.toggleSelected(true);
            }
          }}
          aria-label="Select row"
        />
      </div>
    ),
    enableSorting: false,
    enableHiding: false,
  },
  {
    accessorKey: "id",
    header: "Club ID",
    cell: ({ row }) => <div>{row.original.id}</div>,
  },
  {
    accessorKey: "header",
    header: "Club Name",
    cell: ({ row }) => <TableCellViewer item={row.original} type="club" />,
    enableHiding: false,
  },
  {
    accessorKey: "target",
    header: "Nation",
    cell: ({ row }) => <div>{row.original.target}</div>,
  },
  {
    accessorKey: "limit",
    header: "League",
    cell: ({ row }) => <div>{row.original.limit}</div>,
  },
  {
    id: "actions",
    cell: () => (
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            className="data-[state=open]:bg-muted text-muted-foreground flex size-8"
            size="icon"
          >
            <IconDotsVertical />
            <span className="sr-only">Open menu</span>
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-32">
          <DropdownMenuItem>Edit</DropdownMenuItem>
          <DropdownMenuItem>View Players</DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem variant="destructive">Delete</DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    ),
  },
];

const playerColumns: ColumnDef<z.infer<typeof schema>>[] = [
  {
    id: "drag",
    header: () => null,
    cell: ({ row }) => <DragHandle id={row.original.id} />,
  },
  {
    id: "select",
    header: () => null,
    cell: () => null,
    enableSorting: false,
    enableHiding: false,
  },
  {
    accessorKey: "id",
    header: "Player ID",
    cell: ({ row }) => <div>{row.original.id}</div>,
  },
  {
    accessorKey: "header",
    header: "Player Name",
    cell: ({ row }) => <TableCellViewer item={row.original} type="player" />,
    enableHiding: false,
  },
  {
    accessorKey: "target",
    header: "Nation",
    cell: ({ row }) => <div>{row.original.target}</div>,
  },
  {
    accessorKey: "limit",
    header: "Club",
    cell: ({ row }) => <div>{row.original.limit}</div>,
  },
  {
    accessorKey: "reviewer",
    header: "Position",
    cell: ({ row }) => <div>{row.original.reviewer}</div>,
  },
  {
    id: "actions",
    cell: () => (
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            className="data-[state=open]:bg-muted text-muted-foreground flex size-8"
            size="icon"
          >
            <IconDotsVertical />
            <span className="sr-only">Open menu</span>
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-32">
          <DropdownMenuItem>Edit</DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem variant="destructive">Delete</DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    ),
  },
];

function DraggableRow({ row }: { row: Row<z.infer<typeof schema>> }) {
  const { transform, transition, setNodeRef, isDragging } = useSortable({
    id: row.original.id,
  })

  return (
    <TableRow
      data-state={row.getIsSelected() && "selected"}
      data-dragging={isDragging}
      ref={setNodeRef}
      className="relative z-0 data-[dragging=true]:z-10 data-[dragging=true]:opacity-80"
      style={{
        transform: CSS.Transform.toString(transform),
        transition: transition,
      }}
    >
      {row.getVisibleCells().map((cell) => (
        <TableCell key={cell.id}>
          {flexRender(cell.column.columnDef.cell, cell.getContext())}
        </TableCell>
      ))}
    </TableRow>
  )
}

export function TableCellViewer({ item, type }: { item: z.infer<typeof schema>, type: "league" | "club" | "player" }) {
  const isMobile = useIsMobile();
  const { comparedSeasonPlayer, goalkeeping, shooting, setSeletedPlayerRefId } = useFootball();
  const getPlayerRefId = (item: z.infer<typeof schema>): string | null => {
    if ("player_ref_id" in item && typeof item.player_ref_id === "string") {
      return item.player_ref_id;
    }
    return null;
  };
  const handleDrawerOpen = () => {
    if (type === "player") {
        const playerRefId = getPlayerRefId(item);
        if (playerRefId) {
            console.log("Opening drawer for player:", { 
                playerRefId, 
                position: item.reviewer,
                isGK: item.reviewer === "GK" 
            });
            // ĐẢM BẢO truyền đúng position
            setSeletedPlayerRefId(playerRefId, item.reviewer);
        }
    }
};
  const handleDrawerClose = () => {
    if (type === "player") {
      setSeletedPlayerRefId(null);
    }
  };
  const playerRefId = getPlayerRefId(item);
  const playerComparison = comparedSeasonPlayer && comparedSeasonPlayer.length > 0 ? comparedSeasonPlayer[0] : null;
  const playerChartData = playerComparison
    ? [
        { metric: "Goals", current: playerComparison.currentGoals, previous: playerComparison.previousGoals },
        { metric: "Assists", current: playerComparison.currentAssists, previous: playerComparison.previousAssists },
        { metric: "Appearances", current: playerComparison.currentAppearances, previous: playerComparison.previousAppearances },
        {
          metric: "Minutes",
          current: Math.round(playerComparison.currentMinutesPlayed / 90),
          previous: Math.round(playerComparison.previousMinutesPlayed / 90),
        },
      ]
    : [];
  const playerChartConfig = {
    current: { label: playerComparison?.currentSeason || "Current", color: "hsl(var(--chart-1))" },
    previous: { label: playerComparison?.previousSeason || "Previous", color: "hsl(var(--chart-2))" },
  };
  const isGoalkeeper = item.reviewer === "GK";
  console.log("Player position:", item.reviewer, "Is Goalkeeper:", isGoalkeeper); // Debug log to verify position
  return (
    <Drawer
      direction={isMobile ? "bottom" : "right"}
      onOpenChange={(open) => {
        if (open) {
          handleDrawerOpen();
        } else {
          handleDrawerClose();
        }
      }}
    >
      <DrawerTrigger asChild>
        <Button variant="link" className="text-foreground w-fit px-0 text-left">
          {item.header}
        </Button>
      </DrawerTrigger>
      <DrawerContent>
        <DrawerHeader className="gap-1">
          <DrawerTitle>{item.header}</DrawerTitle>
          <DrawerDescription>{type.charAt(0).toUpperCase() + type.slice(1)} Details</DrawerDescription>
        </DrawerHeader>
        <div className="flex flex-col gap-4 overflow-y-auto px-4 text-sm">
          <div className="grid gap-2">
            <div className="font-medium">
              {type === "league" ? "League ID" : type === "club" ? "Club ID" : "Player ID"}: {item.id}
            </div>
            {type === "player" && <div className="font-medium">Player Ref ID: {playerRefId}</div>}
            <div className="font-medium">Nation: {item.target}</div>
            {type !== "league" && (
              <div className="font-medium">{type === "club" ? "League" : "Club"}: {item.limit}</div>
            )}
            {type === "player" && <div className="font-medium">Position: {item.reviewer}</div>}
            <div className="text-muted-foreground">This is a {type} from {item.target}.</div>
          </div>
          {!isMobile && type === "player" && (
            <>
              {playerComparison && (
                <>
                  <Separator />
                  <div className="grid gap-2">
                    <h3 className="font-semibold">Season Comparison</h3>
                    <div className="grid grid-cols-2 gap-4 text-xs">
                      <div>
                        <div className="font-medium text-muted-foreground">{playerComparison.currentSeason}</div>
                        <div>Goals: {playerComparison.currentGoals}</div>
                        <div>Assists: {playerComparison.currentAssists}</div>
                        <div>Apps: {playerComparison.currentAppearances}</div>
                        <div>G/90: {playerComparison.currentGoalsPer90.toFixed(2)}</div>
                      </div>
                      <div>
                        <div className="font-medium text-muted-foreground">{playerComparison.previousSeason}</div>
                        <div>Goals: {playerComparison.previousGoals}</div>
                        <div>Assists: {playerComparison.previousAssists}</div>
                        <div>Apps: {playerComparison.previousAppearances}</div>
                        <div>G/90: {playerComparison.previousGoalsPer90.toFixed(2)}</div>
                      </div>
                    </div>
                  </div>
                  <ChartContainer config={playerChartConfig}>
                    <BarChart
                      accessibilityLayer
                      data={playerChartData}
                      margin={{ left: 0, right: 10, top: 10, bottom: 10 }}
                    >
                      <CartesianGrid vertical={false} />
                      <XAxis dataKey="metric" tickLine={false} axisLine={false} tickMargin={8} />
                      <ChartTooltip cursor={false} content={<ChartTooltipContent indicator="dashed" />} />
                      <Bar dataKey="previous" fill="var(--color-previous)" radius={4} />
                      <Bar dataKey="current" fill="var(--color-current)" radius={4} />
                    </BarChart>
                  </ChartContainer>
                  <Separator />
                  <div className="grid gap-2">
                    <div className="flex gap-2 items-center font-medium">
                      <span>Performance Trend: {playerComparison.performanceTrend ?? "N/A"}</span>
                      {playerComparison.goalsChangePercentage !== undefined ? (
                        playerComparison.goalsChangePercentage > 0 ? (
                          <IconTrendingUp className="size-4 text-green-500" />
                        ) : playerComparison.goalsChangePercentage < 0 ? (
                          <IconTrendingDown className="size-4 text-red-500" />
                        ) : null
                      ) : null}
                    </div>
                    <div className="text-muted-foreground text-xs">
                      Goals:{" "}
                      {playerComparison.goalsChangePercentage !== undefined
                        ? `${playerComparison.goalsChangePercentage > 0 ? "+" : ""}${playerComparison.goalsChangePercentage.toFixed(1)}%`
                        : "N/A"}{" "}
                      • Assists:{" "}
                      {playerComparison.assistsChangePercentage !== undefined
                        ? `${playerComparison.assistsChangePercentage > 0 ? "+" : ""}${playerComparison.assistsChangePercentage.toFixed(1)}%`
                        : "N/A"}{" "}
                      • Appearances:{" "}
                      {playerComparison.appearancesChangePercentage !== undefined
                        ? `${playerComparison.appearancesChangePercentage > 0 ? "+" : ""}${playerComparison.appearancesChangePercentage.toFixed(1)}%`
                        : "N/A"}
                    </div>
                  </div>
                </>
              )}
              {isGoalkeeper && goalkeeping && (
                <>
                  <Separator />
                  <div className="grid gap-2">
                    <h3 className="font-semibold">Goalkeeping Statistics</h3>
                    <div className="grid grid-cols-2 gap-4 text-xs">
                      <div>
                        <div className="font-medium text-muted-foreground">{goalkeeping.season}</div>
                        <div>Matches Played: {goalkeeping.match_played}</div>
                        <div>Goals Against: {goalkeeping.goals_against}</div>
                        <div>Saves: {goalkeeping.saves}</div>
                        <div>Save Percentage: {goalkeeping.save_percentage}</div>
                        <div>Clean Sheets: {goalkeeping.clean_sheets}</div>
                      </div>
                      <div>
                        <div className="font-medium text-muted-foreground">Penalty Kicks</div>
                        <div>PK Attempted: {goalkeeping.penalty_kicks_attempted}</div>
                        <div>PK Saved: {goalkeeping.penalty_kicks_saved}</div>
                        <div>PK Save %: {goalkeeping.penalty_kicks_saved_percentage}</div>
                      </div>
                    </div>
                  </div>
                </>
              )}
              {!isGoalkeeper && shooting && (
                <>
                  <Separator />
                  <div className="grid gap-2">
                    <h3 className="font-semibold">Shooting Statistics</h3>
                    <div className="grid grid-cols-2 gap-4 text-xs">
                      <div>
                        <div className="font-medium text-muted-foreground">{shooting.season}</div>
                        <div>Goals: {shooting.goals}</div>
                        <div>Shots Total: {shooting.shots_total}</div>
                        <div>Shots on Target: {shooting.shots_on_target}</div>
                        <div>Shot on Target %: {shooting.shots_on_target_percentage}</div>
                      </div>
                      <div>
                        <div className="font-medium text-muted-foreground">Per 90 Stats</div>
                        <div>Shots Total/90: {shooting.shots_total_per_90}</div>
                        <div>Shots on Target/90: {shooting.shots_on_target_90}</div>
                        <div>Goals/Shot: {shooting.goals_shot}</div>
                        <div>Penalty Kicks Made: {shooting.penalty_kicks_made}</div>
                      </div>
                    </div>
                  </div>
                </>
              )}
              {!playerComparison && !goalkeeping && !shooting && (
                <div className="flex justify-center items-center py-8">
                  <div className="text-muted-foreground">Loading player data...</div>
                </div>
              )}
            </>
          )}
          {!isMobile && type !== "player" && (
            <>
              <Separator />
              <ChartContainer config={chartConfig}>
                <AreaChart
                  accessibilityLayer
                  data={chartData}
                  margin={{ left: 0, right: 10 }}
                >
                  <CartesianGrid vertical={false} />
                  <XAxis
                    dataKey="month"
                    tickLine={false}
                    axisLine={false}
                    tickMargin={8}
                    tickFormatter={(value) => value.slice(0, 3)}
                    hide
                  />
                  <ChartTooltip cursor={false} content={<ChartTooltipContent indicator="dot" />} />
                  <Area
                    dataKey="mobile"
                    type="natural"
                    fill="var(--color-mobile)"
                    fillOpacity={0.6}
                    stroke="var(--color-mobile)"
                    stackId="a"
                  />
                  <Area
                    dataKey="desktop"
                    type="natural"
                    fill="var(--color-desktop)"
                    fillOpacity={0.4}
                    stroke="var(--color-desktop)"
                    stackId="a"
                  />
                </AreaChart>
              </ChartContainer>
              <Separator />
              {/* <div className="grid gap-2">
                <div className="flex gap-2 leading-none font-medium">
                  Trending up by 5.2% this month <IconTrendingUp className="size-4" />
                </div>
                <div className="text-muted-foreground">
                  Showing total visitors for the last 6 months. This is just some random text to test the layout.
                </div>
              </div> */}
            </>
          )}
        </div>
        <DrawerFooter>
          <DrawerClose asChild>
            <Button variant="outline">Close</Button>
          </DrawerClose>
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  );
}


export function DataTable({}: {
  data: z.infer<typeof schema>[]
}) {
  const { leagues, clubs, players, selectedLeagueId, selectedClubId, setSelectedLeagueId, setSelectedClubId, status, error } = useFootball();
  
  // Initialize state with mapped data
  const [leagueData, setLeagueData] = React.useState(() => mapLeaguesToTableData(leagues));
  const [clubData, setClubData] = React.useState(() => mapClubsToTableData(clubs));
  const [playerData, setPlayerData] = React.useState(() => mapPlayersToTableData(players));
  
  const [leagueRowSelection, setLeagueRowSelection] = React.useState({});
  const [clubRowSelection, setClubRowSelection] = React.useState({});
  
  // const [columnVisibility, setColumnVisibility] =
  //   React.useState<VisibilityState>({ type: false, status: false, limit: false, reviewer: false });
  // const [columnFilters, setColumnFilters] = React.useState<ColumnFiltersState>([]);
  // const [sorting, setSorting] = React.useState<SortingState>([]);

  const [leagueColumnVisibility, setLeagueColumnVisibility] = React.useState<VisibilityState>({});
  const [clubColumnVisibility, setClubColumnVisibility] = React.useState<VisibilityState>({ limit: true });
  const [playerColumnVisibility, setPlayerColumnVisibility] = React.useState<VisibilityState>({ limit: true, reviewer: true });
  
  const [leagueColumnFilters, setLeagueColumnFilters] = React.useState<ColumnFiltersState>([]);
  const [clubColumnFilters, setClubColumnFilters] = React.useState<ColumnFiltersState>([]);
  const [playerColumnFilters, setPlayerColumnFilters] = React.useState<ColumnFiltersState>([]);
  
  const [leagueSorting, setLeagueSorting] = React.useState<SortingState>([]);
  const [clubSorting, setClubSorting] = React.useState<SortingState>([]);
  const [playerSorting, setPlayerSorting] = React.useState<SortingState>([]);

  // const [pagination, setPagination] = React.useState({
  //   pageIndex: 0,
  //   pageSize: 10,
  // });

  const [laeguePagnination, setLeaguePagnination] = React.useState({pageIndex:0, pageSize:10});
  const [clubPagnination, setClubPagnination] = React.useState({pageIndex:0, pageSize:10});
  const [playerPagnination, setPlayerPagnination] = React.useState({pageIndex:0, pageSize:10});


  const [activeTab, setActiveTab] = React.useState("outline");
  const sortableId = React.useId();
  const sensors = useSensors(
    useSensor(MouseSensor, {}),
    useSensor(TouchSensor, {}),
    useSensor(KeyboardSensor, {})
  );

  // Update data when source data changes
  React.useEffect(() => {
    setLeagueData(mapLeaguesToTableData(leagues));
  }, [leagues]);

  React.useEffect(() => {
    setClubData(mapClubsToTableData(clubs));
  }, [clubs]);

  React.useEffect(() => {
    setPlayerData(mapPlayersToTableData(players));
  }, [players]);

  // Update selected league ID based on league row selection
  React.useEffect(() => {
    const selectedIds = Object.keys(leagueRowSelection).map(Number);
    const newLeagueId = selectedIds.length > 0 ? selectedIds[0] : null;
    if (newLeagueId !== selectedLeagueId) {
      setSelectedLeagueId(newLeagueId);
      // Reset club and player data when league changes
      setClubData([]);
      setPlayerData([]);
      setClubRowSelection({});
    }
  }, [leagueRowSelection, selectedLeagueId, setSelectedLeagueId]);

  // Update selected club ID based on club row selection
  React.useEffect(() => {
    const selectedIds = Object.keys(clubRowSelection).map(Number);
    const newClubId = selectedIds.length > 0 ? selectedIds[0] : null;
    if (newClubId !== selectedClubId) {
      setSelectedClubId(newClubId);
      // Reset player data when club changes
      setPlayerData([]);
    }
  }, [clubRowSelection, selectedClubId, setSelectedClubId]);

  // Reset all selections and data
  const handleReset = () => {
    setLeagueRowSelection({});
    setClubRowSelection({});
    setSelectedLeagueId(null);
    setSelectedClubId(null);
    setClubData([]);
    setPlayerData([]);
    setActiveTab("outline");
    toast.success("Selections reset successfully");
  };

  const leagueDataIds = React.useMemo<UniqueIdentifier[]>(
    () => leagueData.map(({ id }) => id),
    [leagueData]
  );

  const clubDataIds = React.useMemo<UniqueIdentifier[]>(
    () => clubData.map(({ id }) => id),
    [clubData]
  );

  const playerDataIds = React.useMemo<UniqueIdentifier[]>(
    () => playerData.map(({ id }) => id),
    [playerData]
  );

  const leagueTable = useReactTable({
    data: leagueData,
    columns: leagueColumns,
    state: {
      sorting: leagueSorting,
      columnVisibility: leagueColumnVisibility,
      rowSelection: leagueRowSelection,
      columnFilters: leagueColumnFilters,
      pagination: laeguePagnination,
    },
    getRowId: (row) => row.id.toString(),
    enableRowSelection: true,
    enableMultiRowSelection: false,
    onRowSelectionChange: setLeagueRowSelection,
    onSortingChange: setLeagueSorting,
    onColumnFiltersChange: setLeagueColumnFilters,
    onColumnVisibilityChange: setLeagueColumnVisibility,
    onPaginationChange: setLeaguePagnination,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFacetedRowModel: getFacetedRowModel(),
    getFacetedUniqueValues: getFacetedUniqueValues(),
  });

  const clubTable = useReactTable({
    data: clubData,
    columns: clubColumns,
    state: {
      sorting: clubSorting,
      columnVisibility: clubColumnVisibility,
      rowSelection: clubRowSelection,
      columnFilters: clubColumnFilters,
      pagination: clubPagnination,
    },
    getRowId: (row) => row.id.toString(),
    enableRowSelection: true,
    enableMultiRowSelection: false,
    onRowSelectionChange: setClubRowSelection,
    onSortingChange: setClubSorting,
    onColumnFiltersChange: setClubColumnFilters,
    onColumnVisibilityChange: setClubColumnVisibility,
    onPaginationChange: setClubPagnination,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFacetedRowModel: getFacetedRowModel(),
    getFacetedUniqueValues: getFacetedUniqueValues(),
  });

  const playerTable = useReactTable({
    data: playerData,
    columns: playerColumns,
    state: {
      sorting: playerSorting,
      // columnVisibility: { ...columnVisibility, limit: true, reviewer: true },
      columnVisibility: playerColumnVisibility,
      rowSelection: {},
      columnFilters: playerColumnFilters,
      pagination: playerPagnination,
    },
    getRowId: (row) => row.id.toString(),
    enableRowSelection: false,
    onSortingChange: setPlayerSorting,
    onColumnFiltersChange: setPlayerColumnFilters,
    onColumnVisibilityChange: setPlayerColumnVisibility,
    onPaginationChange: setPlayerPagnination,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFacetedRowModel: getFacetedRowModel(),
    getFacetedUniqueValues: getFacetedUniqueValues(),
  });

  function handleDragEnd(event: DragEndEvent, dataType: "league" | "club" | "player") {
    const { active, over } = event;
    if (active && over && active.id !== over.id) {
      if (dataType === "league") {
        setLeagueData((data) => {
          const oldIndex = leagueDataIds.indexOf(active.id);
          const newIndex = leagueDataIds.indexOf(over.id);
          return arrayMove(data, oldIndex, newIndex);
        });
      } else if (dataType === "club") {
        setClubData((data) => {
          const oldIndex = clubDataIds.indexOf(active.id);
          const newIndex = clubDataIds.indexOf(over.id);
          return arrayMove(data, oldIndex, newIndex);
        });
      } else {
        setPlayerData((data) => {
          const oldIndex = playerDataIds.indexOf(active.id);
          const newIndex = playerDataIds.indexOf(over.id);
          return arrayMove(data, oldIndex, newIndex);
        });
      }
    }
  }

  // Handle tab change to sync with Select
  const handleTabChange = (value: string) => {
    setActiveTab(value);
  };

  // Handle Select change to sync with Tabs
  const handleSelectChange = (value: string) => {
    setActiveTab(value);
  };

  return (
    <Tabs
      value={activeTab}
      onValueChange={handleTabChange}
      className="w-full flex-col justify-start gap-6"
    >
      <div className="flex items-center justify-between px-4 lg:px-6">
        <Label htmlFor="view-selector" className="sr-only">
          View
        </Label>
        <Select value={activeTab} onValueChange={handleSelectChange}>
          <SelectTrigger
            className="flex w-fit @4xl/main:hidden"
            size="sm"
            id="view-selector"
          >
            <SelectValue placeholder="Select a view" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="outline">Leagues</SelectItem>
            <SelectItem value="past-performance" disabled={selectedLeagueId === null}>Clubs</SelectItem>
            <SelectItem value="key-personnel" disabled={selectedClubId === null}>Players</SelectItem>
            {/* <SelectItem value="focus-documents">Focus Documents</SelectItem> */}
          </SelectContent>
        </Select>
        <TabsList className="**:data-[slot=badge]:bg-muted-foreground/30 hidden **:data-[slot=badge]:size-5 **:data-[slot=badge]:rounded-full **:data-[slot=badge]:px-1 @4xl/main:flex">
          <TabsTrigger value="outline">Leagues</TabsTrigger>
          <TabsTrigger value="past-performance" disabled={selectedLeagueId === null}>
            Clubs <Badge variant="secondary">{clubs.length}</Badge>
          </TabsTrigger>
          <TabsTrigger value="key-personnel" disabled={selectedClubId === null}>
            Players <Badge variant="secondary">{players.length}</Badge>
          </TabsTrigger>
          {/* <TabsTrigger value="focus-documents">Focus Documents</TabsTrigger> */}
        </TabsList>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm" onClick={handleReset}>
            <IconRefresh />
            <span className="hidden lg:inline">Reset</span>
          </Button>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="outline" size="sm">
                <IconLayoutColumns />
                <span className="hidden lg:inline">Customize Columns</span>
                <span className="lg:hidden">Columns</span>
                <IconChevronDown />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              {leagueTable
                .getAllColumns()
                .filter(
                  (column) =>
                    typeof column.accessorFn !== "undefined" &&
                    column.getCanHide()
                )
                .map((column) => (
                  <DropdownMenuCheckboxItem
                    key={column.id}
                    className="capitalize"
                    checked={column.getIsVisible()}
                    onCheckedChange={(value) =>
                      column.toggleVisibility(!!value)
                    }
                  >
                    {column.id}
                  </DropdownMenuCheckboxItem>
                ))}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>
      <TabsContent
        value="outline"
        className="relative flex flex-col gap-4 overflow-auto px-4 lg:px-6"
      >
        {status === "loading" && leagues.length === 0 && (
          <div className="text-center p-4">Loading leagues...</div>
        )}
        {error && (
          <div className="text-center p-4 text-red-500">Error: {error}</div>
        )}
        {(leagues.length > 0 || error) && (
          <div className="overflow-hidden rounded-lg border">
            <DndContext
              collisionDetection={closestCenter}
              modifiers={[restrictToVerticalAxis]}
              onDragEnd={(event) => handleDragEnd(event, "league")}
              sensors={sensors}
              id={sortableId}
            >
              <Table>
                <TableHeader className="bg-muted sticky top-0 z-10">
                  {leagueTable.getHeaderGroups().map((headerGroup) => (
                    <TableRow key={headerGroup.id}>
                      {headerGroup.headers.map((header) => (
                        <TableHead key={header.id} colSpan={header.colSpan}>
                          {header.isPlaceholder
                            ? null
                            : flexRender(
                                header.column.columnDef.header,
                                header.getContext()
                              )}
                        </TableHead>
                      ))}
                    </TableRow>
                  ))}
                </TableHeader>
                <TableBody className="**:data-[slot=table-cell]:first:w-8">
                  {leagueTable.getRowModel().rows?.length ? (
                    <SortableContext
                      items={leagueDataIds}
                      strategy={verticalListSortingStrategy}
                    >
                      {leagueTable.getRowModel().rows.map((row) => (
                        <DraggableRow key={row.id} row={row} />
                      ))}
                    </SortableContext>
                  ) : (
                    <TableRow>
                      <TableCell
                        colSpan={leagueColumns.length}
                        className="h-24 text-center"
                      >
                        No leagues available.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </DndContext>
          </div>
        )}
        <div className="flex items-center justify-between px-4">
          <div className="text-muted-foreground hidden flex-1 text-sm lg:flex">
            {leagueTable.getFilteredSelectedRowModel().rows.length} league(s) selected.
          </div>
          <div className="flex w-full items-center gap-8 lg:w-fit">
            <div className="hidden items-center gap-2 lg:flex">
              <Label htmlFor="rows-per-page-leagues" className="text-sm font-medium">
                Rows per page
              </Label>
              <Select
                value={`${leagueTable.getState().pagination.pageSize}`}
                onValueChange={(value) => {
                  leagueTable.setPageSize(Number(value));
                }}
              >
                <SelectTrigger size="sm" className="w-20" id="rows-per-page-leagues">
                  <SelectValue
                    placeholder={leagueTable.getState().pagination.pageSize}
                  />
                </SelectTrigger>
                <SelectContent side="top">
                  {[10, 20, 30, 40, 50].map((pageSize) => (
                    <SelectItem key={pageSize} value={`${pageSize}`}>
                      {pageSize}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="flex w-fit items-center justify-center text-sm font-medium">
              Page {leagueTable.getState().pagination.pageIndex + 1} of{" "}
              {leagueTable.getPageCount()}
            </div>
            <div className="ml-auto flex items-center gap-2 lg:ml-0">
              <Button
                variant="outline"
                className="hidden h-8 w-8 p-0 lg:flex"
                onClick={() => leagueTable.setPageIndex(0)}
                disabled={!leagueTable.getCanPreviousPage()}
              >
                <span className="sr-only">Go to first page</span>
                <IconChevronsLeft />
              </Button>
              <Button
                variant="outline"
                className="size-8"
                size="icon"
                onClick={() => leagueTable.previousPage()}
                disabled={!leagueTable.getCanPreviousPage()}
              >
                <span className="sr-only">Go to previous page</span>
                <IconChevronLeft />
              </Button>
              <Button
                variant="outline"
                className="size-8"
                size="icon"
                onClick={() => leagueTable.nextPage()}
                disabled={!leagueTable.getCanNextPage()}
              >
                <span className="sr-only">Go to next page</span>
                <IconChevronRight />
              </Button>
              <Button
                variant="outline"
                className="hidden size-8 lg:flex"
                size="icon"
                onClick={() => leagueTable.setPageIndex(leagueTable.getPageCount() - 1)}
                disabled={!leagueTable.getCanNextPage()}
              >
                <span className="sr-only">Go to last page</span>
                <IconChevronsRight />
              </Button>
            </div>
          </div>
        </div>
      </TabsContent>
      <TabsContent
        value="past-performance"
        className="relative flex flex-col gap-4 overflow-auto px-4 lg:px-6"
      >
        {status === "loading" && clubs.length === 0 && (
          <div className="text-center p-4">Loading clubs...</div>
        )}
        {error && (
          <div className="text-center p-4 text-red-500">Error: {error}</div>
        )}
        {(clubs.length > 0 || error) && (
          <div className="overflow-hidden rounded-lg border">
            <DndContext
              collisionDetection={closestCenter}
              modifiers={[restrictToVerticalAxis]}
              onDragEnd={(event) => handleDragEnd(event, "club")}
              sensors={sensors}
              id={sortableId}
            >
              <Table>
                <TableHeader className="bg-muted sticky top-0 z-10">
                  {clubTable.getHeaderGroups().map((headerGroup) => (
                    <TableRow key={headerGroup.id}>
                      {headerGroup.headers.map((header) => (
                        <TableHead key={header.id} colSpan={header.colSpan}>
                          {header.isPlaceholder
                            ? null
                            : flexRender(
                                header.column.columnDef.header,
                                header.getContext()
                              )}
                        </TableHead>
                      ))}
                    </TableRow>
                  ))}
                </TableHeader>
                <TableBody className="**:data-[slot=table-cell]:first:w-8">
                  {clubTable.getRowModel().rows?.length ? (
                    <SortableContext
                      items={clubDataIds}
                      strategy={verticalListSortingStrategy}
                    >
                      {clubTable.getRowModel().rows.map((row) => (
                        <DraggableRow key={row.id} row={row} />
                      ))}
                    </SortableContext>
                  ) : (
                    <TableRow>
                      <TableCell
                        colSpan={clubColumns.length}
                        className="h-24 text-center"
                      >
                        No clubs available.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </DndContext>
          </div>
        )}
        <div className="flex items-center justify-between px-4">
          <div className="text-muted-foreground hidden flex-1 text-sm lg:flex">
            {clubTable.getFilteredSelectedRowModel().rows.length} club(s) selected.
          </div>
          <div className="flex w-full items-center gap-8 lg:w-fit">
            <div className="hidden items-center gap-2 lg:flex">
              <Label htmlFor="rows-per-page-clubs" className="text-sm font-medium">
                Rows per page
              </Label>
              <Select
                value={`${clubTable.getState().pagination.pageSize}`}
                onValueChange={(value) => {
                  clubTable.setPageSize(Number(value));
                }}
              >
                <SelectTrigger size="sm" className="w-20" id="rows-per-page-clubs">
                  <SelectValue
                    placeholder={clubTable.getState().pagination.pageSize}
                  />
                </SelectTrigger>
                <SelectContent side="top">
                  {[10, 20, 30, 40, 50].map((pageSize) => (
                    <SelectItem key={pageSize} value={`${pageSize}`}>
                      {pageSize}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="flex w-fit items-center justify-center text-sm font-medium">
              Page {clubTable.getState().pagination.pageIndex + 1} of{" "}
              {clubTable.getPageCount()}
            </div>
            <div className="ml-auto flex items-center gap-2 lg:ml-0">
              <Button
                variant="outline"
                className="hidden h-8 w-8 p-0 lg:flex"
                onClick={() => clubTable.setPageIndex(0)}
                disabled={!clubTable.getCanPreviousPage()}
              >
                <span className="sr-only">Go to first page</span>
                <IconChevronsLeft />
              </Button>
              <Button
                variant="outline"
                className="size-8"
                size="icon"
                onClick={() => clubTable.previousPage()}
                disabled={!clubTable.getCanPreviousPage()}
              >
                <span className="sr-only">Go to previous page</span>
                <IconChevronLeft />
              </Button>
              <Button
                variant="outline"
                className="size-8"
                size="icon"
                onClick={() => clubTable.nextPage()}
                disabled={!clubTable.getCanNextPage()}
              >
                <span className="sr-only">Go to next page</span>
                <IconChevronRight />
              </Button>
              <Button
                variant="outline"
                className="hidden size-8 lg:flex"
                size="icon"
                onClick={() => clubTable.setPageIndex(clubTable.getPageCount() - 1)}
                disabled={!clubTable.getCanNextPage()}
              >
                <span className="sr-only">Go to last page</span>
                <IconChevronsRight />
              </Button>
            </div>
          </div>
        </div>
      </TabsContent>
      <TabsContent
        value="key-personnel"
        className="relative flex flex-col gap-4 overflow-auto px-4 lg:px-6"
      >
        {status === "loading" && players.length === 0 && (
          <div className="text-center p-4">Loading players...</div>
        )}
        {error && (
          <div className="text-center p-4 text-red-500">Error: {error}</div>
        )}
        {(players.length > 0 || error) && (
          <div className="overflow-hidden rounded-lg border">
            <DndContext
              collisionDetection={closestCenter}
              modifiers={[restrictToVerticalAxis]}
              onDragEnd={(event) => handleDragEnd(event, "player")}
              sensors={sensors}
              id={sortableId}
            >
              <Table>
                <TableHeader className="bg-muted sticky top-0 z-10">
                  {playerTable.getHeaderGroups().map((headerGroup) => (
                    <TableRow key={headerGroup.id}>
                      {headerGroup.headers.map((header) => (
                        <TableHead key={header.id} colSpan={header.colSpan}>
                          {header.isPlaceholder
                            ? null
                            : flexRender(
                                header.column.columnDef.header,
                                header.getContext()
                              )}
                        </TableHead>
                      ))}
                    </TableRow>
                  ))}
                </TableHeader>
                <TableBody className="**:data-[slot=table-cell]:first:w-8">
                  {playerTable.getRowModel().rows?.length ? (
                    <SortableContext
                      items={playerDataIds}
                      strategy={verticalListSortingStrategy}
                    >
                      {playerTable.getRowModel().rows.map((row) => (
                        <DraggableRow key={row.id} row={row} />
                      ))}
                    </SortableContext>
                  ) : (
                    <TableRow>
                      <TableCell
                        colSpan={playerColumns.length}
                        className="h-24 text-center"
                      >
                        No players available.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </DndContext>
          </div>
        )}
        <div className="flex items-center justify-between px-4">
          <div className="text-muted-foreground hidden flex-1 text-sm lg:flex">
            No players selected.
          </div>
          <div className="flex w-full items-center gap-8 lg:w-fit">
            <div className="hidden items-center gap-2 lg:flex">
              <Label htmlFor="rows-per-page-players" className="text-sm font-medium">
                Rows per page
              </Label>
              <Select
                value={`${playerTable.getState().pagination.pageSize}`}
                onValueChange={(value) => {
                  playerTable.setPageSize(Number(value));
                }}
              >
                <SelectTrigger size="sm" className="w-20" id="rows-per-page-players">
                  <SelectValue
                    placeholder={playerTable.getState().pagination.pageSize}
                  />
                </SelectTrigger>
                <SelectContent side="top">
                  {[10, 20, 30, 40, 50].map((pageSize) => (
                    <SelectItem key={pageSize} value={`${pageSize}`}>
                      {pageSize}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="flex w-fit items-center justify-center text-sm font-medium">
              Page {playerTable.getState().pagination.pageIndex + 1} of{" "}
              {playerTable.getPageCount()}
            </div>
            <div className="ml-auto flex items-center gap-2 lg:ml-0">
              <Button
                variant="outline"
                className="hidden h-8 w-8 p-0 lg:flex"
                onClick={() => playerTable.setPageIndex(0)}
                disabled={!playerTable.getCanPreviousPage()}
              >
                <span className="sr-only">Go to first page</span>
                <IconChevronsLeft />
              </Button>
              <Button
                variant="outline"
                className="size-8"
                size="icon"
                onClick={() => playerTable.previousPage()}
                disabled={!playerTable.getCanPreviousPage()}
              >
                <span className="sr-only">Go to previous page</span>
                <IconChevronLeft />
              </Button>
              <Button
                variant="outline"
                className="size-8"
                size="icon"
                onClick={() => playerTable.nextPage()}
                disabled={!playerTable.getCanNextPage()}
              >
                <span className="sr-only">Go to next page</span>
                <IconChevronRight />
              </Button>
              <Button
                variant="outline"
                className="hidden size-8 lg:flex"
                size="icon"
                onClick={() => playerTable.setPageIndex(playerTable.getPageCount() - 1)}
                disabled={!playerTable.getCanNextPage()}
              >
                <span className="sr-only">Go to last page</span>
                <IconChevronsRight />
              </Button>
            </div>
          </div>
        </div>
      </TabsContent>
      <TabsContent
        value="focus-documents"
        className="flex flex-col px-4 lg:px-6"
      >
        <div className="aspect-video w-full flex-1 rounded-lg border border-dashed"></div>
      </TabsContent>
    </Tabs>
  )
}

const chartData = [
  { month: "January", desktop: 186, mobile: 80 },
  { month: "February", desktop: 305, mobile: 200 },
  { month: "March", desktop: 237, mobile: 120 },
  { month: "April", desktop: 73, mobile: 190 },
  { month: "May", desktop: 209, mobile: 130 },
  { month: "June", desktop: 214, mobile: 140 },
]

const chartConfig = {
  desktop: {
    label: "Desktop",
    color: "var(--primary)",
  },
  mobile: {
    label: "Mobile",
    color: "var(--primary)",
  },
} satisfies ChartConfig
