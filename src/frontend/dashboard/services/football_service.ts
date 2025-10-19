import { Club, ClubTrend, EnhancedSquadResponse, EnhancedTeamDataResponse, Goalkeeping, League, Player, PlayerSeasonComparison, Shooting, UrlInformation } from "@/types/football_type";
import { toast } from "sonner";

// const API_BASE_URL = 'http://localhost:5000';
const API_BASE_URL = 'https://www.renragecnannhoj.site';

const METHODs = {
  GET: "GET",
  POST: "POST"
}


// FOOTBALL SERVICE
export const fetchLeaguesService = async (): Promise<League[]> => {
    const response = await fetch(`${API_BASE_URL}/api/leagues`);
    if (!response.ok)
    {
        toast.error('Failed to fetch leagues.');
        throw new Error('Failed to fetch leagues.');
    }

    return response.json();
}

export const fetchClubsService = async (leagueId: number): Promise<Club[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/club/league/${leagueId}/clubs`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
    });
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const data: Club[] = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to fetch clubs");
    throw error;
  }
};

export const fetchPlayersService = async (clubId: number): Promise<Player[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/player/club/${clubId}/players`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
    });
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const data: Player[] = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to fetch players");
    throw error;
  }
};

export const fetchCurrentPlayersService = async (clubId: number): Promise<Player[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/player/club/${clubId}/players/current`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
    });
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const data: Player[] = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to fetch players");
    throw error;
  }
};

export const fecthComparedSeasonPlayerService = async (playerRefId: string): Promise<PlayerSeasonComparison[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/player/${playerRefId}/season-comparisons`, {
      method: METHODs.GET,
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    };

    const data: PlayerSeasonComparison[] = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to fetch player comparison data");
    throw error;
  };
}

export const fetchGoalkeepingByPlayerService = async (playerRefId: string): Promise<Goalkeeping> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/player/${playerRefId}/goalkeeping`, {
      method: METHODs.GET,
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    };

    const data: Goalkeeping = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to fetch goalkeeping data");
    throw error;
  };
}

export const fetchShootingByPlayerSerivce = async (playerRefId: string): Promise<Shooting> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/player/${playerRefId}/shooting`, {
      method: METHODs.GET,
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    };

    const data: Shooting = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to fetch shooting data");
    throw error;
  };
}

export const fetchClubTrendService = async (clubId: number, seasons: number = 5): Promise<ClubTrend[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/club/${clubId}/trends?seasons=${seasons}`, {
      method: METHODs.GET,
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    };

    const data: ClubTrend[] = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to fetch club trend data");
    throw error;
  };
}

// CRAWLING SERVICE
export const crawlPremierLeagueService = async(): Promise<UrlInformation[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/crawljobs/premier-league`, {
      method: METHODs.GET,
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    };

    const data: UrlInformation[] = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to crawl players");
    throw error;
  }
};

export const crawlRomaniaLiga1Service = async(): Promise<UrlInformation[]> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/crawljobs/romania-liga1`, {
      method: METHODs.GET,
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    };

    const data: UrlInformation[] = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to crawl players");
    throw error;
  }
};

export const crawlAllDataService = async (url: string, id: string): Promise<EnhancedTeamDataResponse> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/simplecrawler/all-data?url=${encodeURIComponent(url)}
          &id=${encodeURIComponent(id)}`, {
            method: METHODs.GET,
            headers: { "Content-Type": "application/json" },
          });

       if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data: EnhancedTeamDataResponse = await response.json();
      return data;
  } catch (error) {
    toast.error("Failed to extract all data");
    throw error;
  }
};

export const extractSquadStandardService = async (url: string, selector?: string): Promise<EnhancedSquadResponse> => {
  try {
    const query = new URLSearchParams();
    query.append('url', url);
    if (selector) query.append('selector', selector);

    const response = await fetch(`${API_BASE_URL}/api/simplecrawler/squad-standard?${query.toString()}`, {
      method: METHODs.GET,
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data: EnhancedSquadResponse = await response.json();
    return data;
  } catch (error) {
    toast.error("Failed to extract squad standard data");
    throw error;
  }
};

export const downloadJsonService = async (url: string, id: string): Promise<Blob> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/simplecrawler/download-json?url=${encodeURIComponent(url)}&id=${encodeURIComponent(id)}`, {
      method: METHODs.GET,
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.blob();
  } catch (error) {
    toast.error("Failed to download JSON file");
    throw error;
  }
};

export const downloadZipService = async (url: string, id: string): Promise<Blob> => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/simplecrawler/download-zip?url=${encodeURIComponent(url)}&id=${encodeURIComponent(id)}`, {
      method: METHODs.GET,
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.blob();
  } catch (error) {
    toast.error("Failed to download ZIP file");
    throw error;
  }
};

export const downloadSquadStandardJsonService = async (url: string, selector?: string): Promise<Blob> => {
  try {
    const query = new URLSearchParams();
    query.append('url', url);
    if (selector) query.append('selector', selector);

    const response = await fetch(`${API_BASE_URL}/api/simplecrawler/download-squad-standard-json?${query.toString()}`, {
      method: METHODs.GET,
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.blob();
  } catch (error) {
    toast.error("Failed to download squad standard JSON file");
    throw error;
  }
};

export const downloadSquadStandardZipService = async (url: string, selector?: string): Promise<Blob> => {
  try {
    const query = new URLSearchParams();
    query.append('url', url);
    if (selector) query.append('selector', selector);

    const response = await fetch(`${API_BASE_URL}/api/simplecrawler/download-squad-standard-zip?${query.toString()}`, {
      method: METHODs.GET,
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.blob();
  } catch (error) {
    toast.error("Failed to download squad standard ZIP file");
    throw error;
  }
};

// Helper function to trigger file download
export const triggerFileDownload = (blob: Blob, filename: string) => {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
};