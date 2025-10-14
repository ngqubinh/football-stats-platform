import { Club, ClubTrend, EnhancedTeamDataResponse, Goalkeeping, League, Player, PlayerSeasonComparison, Shooting, UrlInformation } from "@/types/football_type";
import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { crawlAllDataService, crawlPremierLeagueService, crawlRomaniaLiga1Service, downloadJsonService, downloadZipService, fecthComparedSeasonPlayerService, fetchClubsService, fetchClubTrendService, fetchCurrentPlayersService, fetchGoalkeepingByPlayerService, fetchLeaguesService, fetchPlayersService, fetchShootingByPlayerSerivce, triggerFileDownload } from "@/services/football_service";
import { toast } from "sonner";

interface FootballState {
    leagues: League[];
    clubs: Club[];
    players: Player[];
    crawlData: UrlInformation[];
    playerSeasonComparisionData: PlayerSeasonComparison[];
    goalkeepingData: Goalkeeping | null;
    shootingData: Shooting | null;
    clubTrendData: ClubTrend[];
    selectedLeagueId: number | null;
    selectedClubId: number | null;
    selectedPlayerRefId: string | null;
    selectedPlayerPosition: string | null;
    status: "idle" | "loading" | "succeeded" | "failed";
    crawlStatus: "idle" | "loading" | "succeeded" | "failed";
    error: string | null;
    crawlError: string | null;

    extractAllData: EnhancedTeamDataResponse | null;
    extractAllDataStatus: "idle" | "loading" | "succeeded" | "failed";
    extractAllDataError: string | null;
}

const initialState: FootballState = {
    leagues: [],
    clubs: [],
    players: [],
    crawlData: [],
    playerSeasonComparisionData: [],
    goalkeepingData: null,
    shootingData: null,
    clubTrendData: [],
    selectedLeagueId: null,
    selectedClubId: null,
    selectedPlayerRefId: null,
    selectedPlayerPosition: null,
    status: "idle",
    crawlStatus: "idle",
    error: null,
    crawlError: null,

    extractAllData: null,
    extractAllDataStatus: "idle",
    extractAllDataError: null,
};

// FOOTBALL SLICES
export const fetchLeaguesSlice = createAsyncThunk("leagues/fetch", async () => {
  return await fetchLeaguesService();
});

export const fetchClubsSlice = createAsyncThunk("football/fetchClubs", async (leagueId: number) => {
  return await fetchClubsService(leagueId);
});

export const fetchPlayersSlice = createAsyncThunk("football/fetchPlayers", async (clubId: number) => {
  return await fetchPlayersService(clubId);
});

export const fetchCurrenntPlayersSlice = createAsyncThunk("football/fetchCurrentPlayers", async (clubId: number) => {
  return await fetchCurrentPlayersService(clubId);
});

export const fetchComparedSeasonPlayerSlice = createAsyncThunk("football/fetchComaparedSeasonPlayer", async (playerRefId: string) => {
  return await fecthComparedSeasonPlayerService(playerRefId);
});

export const fetchGoalkeepingByPlayerSlice = createAsyncThunk("football/fetchGoalkeepingByPlayer", async (playerRefId: string) => {
  return await fetchGoalkeepingByPlayerService(playerRefId);
});

export const fetchShootingByPlayerSlice = createAsyncThunk("football/eftchShootingByPlayer", async (playerRefId: string) => {
  return await fetchShootingByPlayerSerivce(playerRefId);
});

export const fetchClubTrendSlice = createAsyncThunk(
  "football/fetchClubTrend", 
  async ({ clubId, seasons = 5 }: { clubId: number; seasons?: number }) => {
    return await fetchClubTrendService(clubId, seasons);
  }
);

// CRAWLING SLICES
export const crawlPremierLeagueSlice = createAsyncThunk("crawl/premierleague", async () => {
  return await crawlPremierLeagueService();
})

export const crawlRomaniaLiga1Slice = createAsyncThunk("crawl/romanialiga1", async () => {
  return await crawlRomaniaLiga1Service();
})

export const extractAllDataSlice = createAsyncThunk(
  "football/extractAllData",
  async ({ url, id }: { url: string; id: string }) => {
    return await crawlAllDataService(url, id);
  }
);

export const downloadJsonSlice = createAsyncThunk(
  "football/downloadJson",
  async ({ url, id }: { url: string; id: string }, { dispatch }) => {
    try {
      const blob = await downloadJsonService(url, id);
      // Handle the Blob directly in the thunk
      const filename = `team_data_${Date.now()}.json`;
      triggerFileDownload(blob, filename); // Trigger download
      // toast.success("JSON file downloaded successfully");
      // Return serializable data only
      return { url, id, success: true };
    } catch (error) {
      toast.error("Failed to download JSON file");
      throw error; // Let Redux Toolkit handle the error
    }
  }
);

export const downloadZipSlice = createAsyncThunk(
  "football/downloadZip",
  async ({ url, id }: { url: string; id: string }, { dispatch }) => {
    try {
      const blob = await downloadZipService(url, id);
      const filename = `team_data_${Date.now()}.zip`;
      triggerFileDownload(blob, filename);
      // toast.success("ZIP file downloaded successfully");
      return { url, id, success: true };
    } catch (error) {
      toast.error("Failed to download ZIP file");
      throw error;
    }
  }
);

const footballSlice = createSlice({
  name: "football",
  initialState,
  reducers: {
    setSelectedLeagueId: (state, action: { payload: number | null }) => {
      state.selectedLeagueId = action.payload;
      if (!action.payload) {
        state.clubs = [];
        state.players = [];
        state.selectedClubId = null;
      }
    },
    setSelectedClubId: (state, action: { payload: number | null }) => {
      state.selectedClubId = action.payload;
      if (!action.payload) {
        state.players = [];
      }
    },
    setPlayerRefId: (state, action: { payload: { playerRefId: string | null; position?: string } }) => {
        //console.log("Setting player ref ID:", action.payload);
        state.selectedPlayerRefId = action.payload.playerRefId;
        state.selectedPlayerPosition = action.payload.position || null;
        
        // CHỈ reset dữ liệu chi tiết của player, KHÔNG reset danh sách players
        if (!action.payload.playerRefId) {
          // Giữ nguyên state.players - đây là danh sách 28 cầu thủ
          state.playerSeasonComparisionData = [];
          state.goalkeepingData = null;
          state.shootingData = null;
          state.selectedPlayerPosition = null;
        } else {
          // Khi chọn player mới, reset dữ liệu chi tiết cũ
          state.playerSeasonComparisionData = [];
          state.goalkeepingData = null;
          state.shootingData = null;
        }
    },

    resetCrawlData: (state) => {
      state.crawlData = [];
      state.crawlStatus = "idle";
      state.crawlError = null;
    },

    resetExtractAllData: (state) => {
      state.extractAllData = null;
      state.extractAllDataStatus = "idle";
      state.extractAllDataError = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchLeaguesSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchLeaguesSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.leagues = action.payload;
      })
      .addCase(fetchLeaguesSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to fetch leagues";
      })
      .addCase(fetchClubsSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchClubsSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.clubs = action.payload;
      })
      .addCase(fetchClubsSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to fetch clubs";
      })
      .addCase(fetchPlayersSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchPlayersSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.players = action.payload;
      })
      .addCase(fetchPlayersSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to fetch players";
      })
      .addCase(fetchCurrenntPlayersSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchCurrenntPlayersSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.players = action.payload;
      })
      .addCase(fetchCurrenntPlayersSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to fetch players";
      })
      
      .addCase(fetchComparedSeasonPlayerSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchComparedSeasonPlayerSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.playerSeasonComparisionData = action.payload;
      })
      .addCase(fetchComparedSeasonPlayerSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to fetch compared seasson player";
      })

      .addCase(fetchGoalkeepingByPlayerSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchGoalkeepingByPlayerSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.goalkeepingData = action.payload;
      })
      .addCase(fetchGoalkeepingByPlayerSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to fetch compared seasson player";
      })

      .addCase(fetchShootingByPlayerSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(fetchShootingByPlayerSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
        state.shootingData = action.payload;
      })
      .addCase(fetchShootingByPlayerSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to fetch compared seasson player";
      })

      .addCase(fetchClubTrendSlice.pending, (state) => {
          state.status = "loading";
      })
      .addCase(fetchClubTrendSlice.fulfilled, (state, action) => {
          state.status = "succeeded";
          state.clubTrendData = action.payload;
      })
      .addCase(fetchClubTrendSlice.rejected, (state, action) => {
          state.status = "failed";
          state.error = action.error.message || "Failed to fetch club trend data";
      })
      
      // CRAWL
      .addCase(crawlPremierLeagueSlice.pending, (state) => {
        state.crawlStatus = "loading";
        state.crawlError = null;
      })
      .addCase(crawlPremierLeagueSlice.fulfilled, (state, action) => {
        state.crawlStatus = "succeeded";
        state.crawlData = action.payload;
      })
      .addCase(crawlPremierLeagueSlice.rejected, (state, action) => {
        state.crawlStatus = "failed";
        state.crawlError = action.error.message || "Failed to crawl Premier League";
      })
      .addCase(crawlRomaniaLiga1Slice.pending, (state) => {
        state.crawlStatus = "loading";
        state.crawlError = null;
      })
      .addCase(crawlRomaniaLiga1Slice.fulfilled, (state, action) => {
        state.crawlStatus = "succeeded";
        state.crawlData = action.payload;
      })
      .addCase(crawlRomaniaLiga1Slice.rejected, (state, action) => {
        state.crawlStatus = "failed";
        state.crawlError = action.error.message || "Failed to crawl Romania Liga 1";
      })

      .addCase(extractAllDataSlice.pending, (state) => {
        state.extractAllDataStatus = "loading";
        state.extractAllDataError = null;
      })
      .addCase(extractAllDataSlice.fulfilled, (state, action) => {
        state.extractAllDataStatus = "succeeded";
        state.extractAllData = action.payload;
      })
      .addCase(extractAllDataSlice.rejected, (state, action) => {
        state.extractAllDataStatus = "failed";
        state.extractAllDataError = action.error.message || "Failed to extract all data";
      })
      
      // Download JSON cases
      .addCase(downloadJsonSlice.pending, (state) => {
        state.status = "loading"; // Optionally track download status
      })
      .addCase(downloadJsonSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
      })
      .addCase(downloadJsonSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to download JSON file";
      })
      
      // Download ZIP cases
      .addCase(downloadZipSlice.pending, (state) => {
        state.status = "loading";
      })
      .addCase(downloadZipSlice.fulfilled, (state, action) => {
        state.status = "succeeded";
      })
      .addCase(downloadZipSlice.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message || "Failed to download ZIP file";
      });
  },
});

export const { setSelectedLeagueId, setSelectedClubId, setPlayerRefId, resetCrawlData, resetExtractAllData } = footballSlice.actions;
export default footballSlice.reducer;