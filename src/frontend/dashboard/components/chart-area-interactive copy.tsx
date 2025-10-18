"use client"

import * as React from "react"
import { Area, AreaChart, CartesianGrid, XAxis } from "recharts"
import { useIsMobile } from "@/hooks/use-mobile"
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import {
  ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  ToggleGroup,
  ToggleGroupItem,
} from "@/components/ui/toggle-group"
import { useFootball } from "@/hooks/useFootball"

export const description = "Club Performance Trend"

const chartConfig = {
  totalGoals: {
    label: "Total Goals",
    color: "hsl(var(--chart-1))",
  },
  totalGoalsAgainst: {
    label: "Goals Against",
    color: "hsl(var(--chart-2))",
  },
  totalAssists: {
    label: "Total Assists", 
    color: "hsl(var(--chart-3))",
  },
} satisfies ChartConfig

export function ClubTrendChart() {
  const isMobile = useIsMobile()
  const [timeRange, setTimeRange] = React.useState("all")
  const [, setLocalSelectedClubId] = React.useState<number | null>(null)
  
  // Sử dụng hook football
  const { 
    leagues,
    clubs, 
    clubTrendData, 
    selectedLeagueId,
    selectedClubId,
    setSelectedLeagueId,
    setSelectedClubId,
    status 
  } = useFootball()

  // Tìm league và club đang được chọn
  // const selectedLeague = leagues.find(league => league.league_id === selectedLeagueId)
  const selectedClub = clubs.find(club => club.club_id === selectedClubId)

  // Chỉ set club khi league thay đổi và có clubs
  React.useEffect(() => {
    if (selectedLeagueId && clubs.length > 0 && !selectedClubId) {
      // Chỉ set club nếu chưa có club nào được chọn
      setSelectedClubId(clubs[0].club_id)
    }
  }, [selectedLeagueId, clubs]) // Loại bỏ selectedClubId và setSelectedClubId khỏi dependencies

  // Xử lý khi league thay đổi
  const handleLeagueChange = (value: string) => {
    const newLeagueId = parseInt(value)
    setSelectedLeagueId(newLeagueId)
    // Reset club selection khi league thay đổi
    setSelectedClubId(null)
    setLocalSelectedClubId(null)
  }

  // Xử lý khi club thay đổi
  const handleClubChange = (value: string) => {
    const newClubId = parseInt(value)
    setSelectedClubId(newClubId)
    setLocalSelectedClubId(newClubId)
  }

  // Chuẩn bị dữ liệu cho chart từ clubTrendData
  const chartData = React.useMemo(() => {
    if (!clubTrendData || clubTrendData.length === 0) {
      return []
    }

    return clubTrendData.map(trend => ({
      season: trend.season,
      totalGoals: trend.totalGoals,
      totalGoalsAgainst: trend.totalGoalsAgainst,
      totalAssists: trend.totalAssists,
    }))
  }, [clubTrendData])

  // Lọc dữ liệu theo timeRange
  const filteredData = React.useMemo(() => {
    if (timeRange === "all") {
      return chartData
    }
    
    const seasonsToShow = parseInt(timeRange)
    return chartData.slice(-seasonsToShow)
  }, [chartData, timeRange])

  // Tự động chọn range trên mobile
  React.useEffect(() => {
    if (isMobile) {
      setTimeRange("3")
    }
  }, [isMobile])

  return (
    <Card className="@container/card">
      <CardHeader>
        <CardTitle>
          {selectedClub ? `${selectedClub.club_name} Performance` : "Club Performance"}
        </CardTitle>
        <CardDescription>
          <span className="hidden @[540px]/card:block">
            Goals, assists and goals against trend
          </span>
          <span className="@[540px]/card:hidden">Performance trend</span>
        </CardDescription>
        <CardAction className="flex flex-col gap-4">
          {/* League và Club Selection */}
          <div className="flex flex-col gap-4 @[540px]/card:flex-row @[540px]/card:items-center">
  {/* League Selection Dropdown - DISABLED */}
  <div className="flex-1">
    <Select 
      value={selectedLeagueId?.toString() || ""} 
      onValueChange={handleLeagueChange}
      disabled={true} // 👈 Thêm dòng này để disable toàn bộ
    >
      <SelectTrigger className="w-full">
        <SelectValue placeholder="Select a league" />
      </SelectTrigger>
      <SelectContent>
        {leagues.map((league) => (
          <SelectItem 
            key={league.league_id} 
            value={league.league_id.toString()}
            className="rounded-lg"
          >
            {league.league_name}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  </div>

  {/* Club Selection Dropdown - DISABLED */}
  <div className="flex-1">
    <Select 
      value={selectedClubId?.toString() || ""} 
      onValueChange={handleClubChange}
      disabled={true} // 👈 Thêm dòng này để disable toàn bộ
    >
      <SelectTrigger className="w-full">
        <SelectValue 
          placeholder="Dropdown disabled" 
        />
      </SelectTrigger>
      <SelectContent>
        {clubs.map((club) => (
          <SelectItem 
            key={club.club_id} 
            value={club.club_id.toString()}
            className="rounded-lg"
          >
            {club.club_name}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  </div>
</div>

          {/* Time Range Selector */}
          <div className="flex items-center gap-2 self-end">
            <ToggleGroup
              type="single"
              value={timeRange}
              onValueChange={setTimeRange}
              variant="outline"
              className="hidden *:data-[slot=toggle-group-item]:!px-3 @[767px]/card:flex"
            >
              <ToggleGroupItem value="all">All</ToggleGroupItem>
              <ToggleGroupItem value="5">Last 5</ToggleGroupItem>
              <ToggleGroupItem value="3">Last 3</ToggleGroupItem>
            </ToggleGroup>
            <Select value={timeRange} onValueChange={setTimeRange}>
              <SelectTrigger
                className="flex w-32 **:data-[slot=select-value]:block **:data-[slot=select-value]:truncate @[767px]/card:hidden"
                size="sm"
                aria-label="Select time range"
              >
                <SelectValue placeholder="All seasons" />
              </SelectTrigger>
              <SelectContent className="rounded-xl">
                <SelectItem value="all" className="rounded-lg">
                  All seasons
                </SelectItem>
                <SelectItem value="5" className="rounded-lg">
                  Last 5
                </SelectItem>
                <SelectItem value="3" className="rounded-lg">
                  Last 3
                </SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardAction>
      </CardHeader>
      <CardContent className="px-2 pt-4 sm:px-6 sm:pt-6">
        {!selectedLeagueId ? (
          <div className="flex h-[250px] items-center justify-center">
            <div className="text-muted-foreground">Please select a league to view clubs</div>
          </div>
        ) : !selectedClubId ? (
          <div className="flex h-[250px] items-center justify-center">
            <div className="text-muted-foreground">Please select a club to view trend data</div>
          </div>
        ) : status === "loading" ? (
          <div className="flex h-[250px] items-center justify-center">
            <div className="text-muted-foreground">Loading chart data...</div>
          </div>
        ) : filteredData.length === 0 ? (
          <div className="flex h-[250px] items-center justify-center">
            <div className="text-muted-foreground">
              No trend data available for {selectedClub?.club_name}
            </div>
          </div>
        ) : (
          <ChartContainer
            config={chartConfig}
            className="aspect-auto h-[250px] w-full"
          >
            <AreaChart data={filteredData}>
              <defs>
                <linearGradient id="fillGoals" x1="0" y1="0" x2="0" y2="1">
                  <stop
                    offset="5%"
                    stopColor="var(--color-totalGoals)"
                    stopOpacity={1.0}
                  />
                  <stop
                    offset="95%"
                    stopColor="var(--color-totalGoals)"
                    stopOpacity={0.1}
                  />
                </linearGradient>
                <linearGradient id="fillGoalsAgainst" x1="0" y1="0" x2="0" y2="1">
                  <stop
                    offset="5%"
                    stopColor="var(--color-totalGoalsAgainst)"
                    stopOpacity={0.8}
                  />
                  <stop
                    offset="95%"
                    stopColor="var(--color-totalGoalsAgainst)"
                    stopOpacity={0.1}
                  />
                </linearGradient>
                <linearGradient id="fillAssists" x1="0" y1="0" x2="0" y2="1">
                  <stop
                    offset="5%"
                    stopColor="var(--color-totalAssists)"
                    stopOpacity={0.8}
                  />
                  <stop
                    offset="95%"
                    stopColor="var(--color-totalAssists)"
                    stopOpacity={0.1}
                  />
                </linearGradient>
              </defs>
              <CartesianGrid vertical={false} />
              <XAxis
                dataKey="season"
                tickLine={false}
                axisLine={false}
                tickMargin={8}
                minTickGap={32}
              />
              <ChartTooltip
                cursor={false}
                defaultIndex={isMobile ? -1 : Math.floor(filteredData.length / 2)}
                content={
                  <ChartTooltipContent
                    indicator="dot"
                  />
                }
              />
              <Area
                dataKey="totalGoals"
                type="natural"
                fill="url(#fillGoals)"
                stroke="var(--color-totalGoals)"
                stackId="a"
              />
              <Area
                dataKey="totalGoalsAgainst"
                type="natural"
                fill="url(#fillGoalsAgainst)"
                stroke="var(--color-totalGoalsAgainst)"
                stackId="a"
              />
              <Area
                dataKey="totalAssists"
                type="natural"
                fill="url(#fillAssists)"
                stroke="var(--color-totalAssists)"
                stackId="a"
              />
            </AreaChart>
          </ChartContainer>
        )}
      </CardContent>
    </Card>
  )
}