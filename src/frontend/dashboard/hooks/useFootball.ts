import { useEffect } from "react";
import { useAppDispatch, useAppSelector } from "./hook";
import { resetExtractAllData, resetExtractSquadStandard, crawlPremierLeagueSlice, crawlRomaniaLiga1Slice, downloadJsonSlice, downloadZipSlice, extractAllDataSlice, extractSquadStandardSlice, fetchClubsSlice, fetchClubTrendSlice, fetchComparedSeasonPlayerSlice, fetchCurrenntPlayersSlice, fetchGoalkeepingByPlayerSlice, fetchLeaguesSlice, fetchPlayersSlice, fetchShootingByPlayerSlice, resetCrawlData, setPlayerRefId, setSelectedClubId, setSelectedLeagueId, downloadSquadStandardJsonSlice, downloadSquadStandardZipSlice } from "@/features/footballSlice";

export const useFootball = () => {
    const dispatch = useAppDispatch();
    const leagues = useAppSelector((state) => state.leagues.leagues);
    const clubs = useAppSelector((state) => state.leagues.clubs);
    const players = useAppSelector((state) => state.leagues.players);
    const comparedSeasonPlayer = useAppSelector((state) => state.leagues.playerSeasonComparisionData);
    const goalkeeping = useAppSelector((state) => state.leagues.goalkeepingData);
    const shooting = useAppSelector((state) => state.leagues.shootingData);
    const crawlData = useAppSelector((state) => state.leagues.crawlData);
    const selectedLeagueId = useAppSelector((state) => state.leagues.selectedLeagueId);
    const selectedClubId = useAppSelector((state) => state.leagues.selectedClubId);
    const selectedPlayerRefId = useAppSelector((state) => state.leagues.selectedPlayerRefId);
    const status = useAppSelector((state) => state.leagues.status);
    const crawlStatus = useAppSelector((state) => state.leagues.crawlStatus);
    const error = useAppSelector((state) => state.leagues.error);
    const crawlError = useAppSelector((state) => state.leagues.crawlError);

    const selectedPlayerPosition = useAppSelector((state) => state.leagues.selectedPlayerPosition);
    const clubTrendData = useAppSelector((state) => state.leagues.clubTrendData);

    const extractedData = useAppSelector((state) => state.leagues.extractAllData);
    const extractAllDataStatus = useAppSelector((state) => state.leagues.extractAllDataStatus);
    const extractAllDataError = useAppSelector((state) => state.leagues.extractAllDataError);

    const extractedSquadStandard = useAppSelector((state) => state.leagues.extractSquadStandard);
    const extractSquadStandardStatus = useAppSelector((state) => state.leagues.extractSquadStandardStatus);
    const extractSquadStandardError = useAppSelector((state) => state.leagues.extractSquadStandardError);

    const downloadSquadStandardStatus = useAppSelector((state) => state.leagues.downloadSquadStandardStatus);
    const downloadSquadStandardError = useAppSelector((state) => state.leagues.downloadSquadStandardError);

    useEffect(() => {
        if (status === "idle") {
        dispatch(fetchLeaguesSlice());
        }
    }, [status, dispatch]);

    useEffect(() => {
        if (selectedLeagueId !== null) {
        dispatch(fetchClubsSlice(selectedLeagueId));
        }
    }, [selectedLeagueId, dispatch]);

    // useEffect(() => {
    //     if (selectedClubId !== null) {
    //     dispatch(fetchPlayersSlice(selectedClubId));
    //     }
    // }, [selectedClubId, dispatch]);

    useEffect(() => {
        if (selectedClubId !== null) {
        dispatch(fetchCurrenntPlayersSlice(selectedClubId));
        }
    }, [selectedClubId, dispatch]);
    
    useEffect(() => {
        if (selectedPlayerRefId !== null) {
        dispatch(fetchComparedSeasonPlayerSlice(selectedPlayerRefId));
        }
    }, [selectedPlayerRefId, dispatch]);

    useEffect(() => {
        if (selectedClubId !== null) {
            dispatch(fetchClubTrendSlice({ clubId: selectedClubId, seasons: 5 }));
        }
    }, [selectedClubId, dispatch]);

    useEffect(() => {
        console.log("Goalkeeping useEffect:", { 
            selectedPlayerRefId, 
            selectedPlayerPosition,
            shouldFetch: selectedPlayerRefId !== null && selectedPlayerPosition === "GK"
        });
        
        if (selectedPlayerRefId !== null && selectedPlayerPosition === "GK") {
            console.log("Fetching goalkeeping data for GK");
            dispatch(fetchGoalkeepingByPlayerSlice(selectedPlayerRefId));
        }
    }, [selectedPlayerRefId, selectedPlayerPosition, dispatch]);

    // CHỈ gọi shooting nếu KHÔNG PHẢI GK
    useEffect(() => {
        console.log("Shooting useEffect:", { 
            selectedPlayerRefId, 
            selectedPlayerPosition,
            shouldFetch: selectedPlayerRefId !== null && selectedPlayerPosition !== "GK" && selectedPlayerPosition !== null
        });
        
        if (selectedPlayerRefId !== null && selectedPlayerPosition !== "GK" && selectedPlayerPosition !== null) {
            console.log("Fetching shooting data for non-GK");
            dispatch(fetchShootingByPlayerSlice(selectedPlayerRefId));
        }
    }, [selectedPlayerRefId, selectedPlayerPosition, dispatch]);
    

    // Crawling functions
    const crawlPremierLeague = () => {
        dispatch(crawlPremierLeagueSlice());
    };

    const crawlRomaniaLiga1 = () => {
        dispatch(crawlRomaniaLiga1Slice());
    };

    const resetCrawl = () => {
        dispatch(resetCrawlData());
    };

    const extractAllData = (url: string, id: string) => {
        return dispatch(extractAllDataSlice({ url, id }));
    };

    const extractSquadStandard = (url: string, selector?: string) => {
        return dispatch(extractSquadStandardSlice({ url, selector }));
    };

    const downloadJson = (url: string, id: string) => {
        return dispatch(downloadJsonSlice({ url, id }));
    };

    const downloadZip = (url: string, id: string) => {
        return dispatch(downloadZipSlice({ url, id }));
    };

    const resetExtractedData = () => {
        dispatch(resetExtractAllData());
    };

    const resetExtractedSquadStandard = () => {
        dispatch(resetExtractSquadStandard());
    };

    const downloadSquadStandardJson = (url: string, selector?: string) => {
        return dispatch(downloadSquadStandardJsonSlice({ url, selector }));
    };

    const downloadSquadStandardZip = (url: string, selector?: string) => {
        return dispatch(downloadSquadStandardZipSlice({ url, selector }));
    };

    return {
        leagues,
        clubs,
        players,
        comparedSeasonPlayer,
        goalkeeping,
        shooting,
        clubTrendData,

        selectedLeagueId,
        selectedClubId,
        selectedPlayerRefId,

        setSelectedLeagueId: (leagueId: number | null) => dispatch(setSelectedLeagueId(leagueId)),
        setSelectedClubId: (clubId: number | null) => dispatch(setSelectedClubId(clubId)),
        // setSeletedPlayerRefId: (playerRefId: string | null) => dispatch(setPlayerRefId(playerRefId)),
        setSeletedPlayerRefId: (playerRefId: string | null, position?: string) => 
            dispatch(setPlayerRefId({ playerRefId, position })),

        status,
        error,
        
        crawlData,
        crawlStatus,
        crawlError,
        crawlPremierLeague,
        crawlRomaniaLiga1,
        resetCrawl,

        extractAllData,
        extractedData,
        extractAllDataStatus,
        extractAllDataError,
        downloadJson,
        downloadZip,
        resetExtractAllData: resetExtractedData,

        extractSquadStandard,
        extractedSquadStandard,
        extractSquadStandardStatus,
        extractSquadStandardError,
        resetExtractSquadStandard: resetExtractedSquadStandard,

        downloadSquadStandardJson,
        downloadSquadStandardZip,
        downloadSquadStandardStatus,
        downloadSquadStandardError,
    };
}