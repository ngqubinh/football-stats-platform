"use client";

import React, { useState } from 'react';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { AppSidebar } from "@/components/app-sidebar";
import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";
import { SiteHeader } from "@/components/site-header";
import { useFootball } from "@/hooks/useFootball";
import { Download, FileJson, FileArchive, Loader2, CheckCircle, AlertCircle } from "lucide-react";
import { toast } from "sonner";

export default function DataExtractionPage() {
  const {
    extractAllData,
    extractedData,
    extractAllDataStatus,
    extractAllDataError,
    downloadJson,
    downloadZip,
    resetExtractAllData
  } = useFootball();

  const [url, setUrl] = useState('');
  const [id, setId] = useState('');

  const handleExtractData = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!url || !id) {
      toast.error('Please provide both URL and ID');
      return;
    }

    try {
      await extractAllData(url, id);
      toast.success('Data extracted successfully!');
    } catch (error: unknown) {
      if (error instanceof Error) {
        toast.error(`Failed to extract data: ${error.message}`);
      } else {
        toast.error('Failed to extract data: Unknown error');
      }
    }
}


  const handleDownloadJson = () => {
    if (!url || !id) {
      toast.error('Please provide both URL and ID');
      return;
    }
    downloadJson(url, id);
  };

  const handleDownloadZip = () => {
    if (!url || !id) {
      toast.error('Please provide both URL and ID');
      return;
    }
    downloadZip(url, id);
  };

  const handleReset = () => {
    setUrl('');
    setId('');
    resetExtractAllData();
  };

  const extractionData = extractAllDataStatus === 'succeeded' ? extractedData : null;

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
                <div className="flex items-center justify-between mb-6">
                  <h1 className="text-3xl font-bold">Data Extraction</h1>
                  <Badge variant={extractAllDataStatus === 'succeeded' ? "default" : "secondary"}>
                    {extractAllDataStatus === 'idle' && 'Ready'}
                    {extractAllDataStatus === 'loading' && 'Extracting...'}
                    {extractAllDataStatus === 'succeeded' && 'Success'}
                    {extractAllDataStatus === 'failed' && 'Failed'}
                  </Badge>
                </div>

                {/* Input Form */}
                <Card className="mb-6">
                  <CardHeader>
                    <CardTitle>Extract Team Data</CardTitle>
                    <CardDescription>
                      Enter the team stats URL and ID to extract all data including players, goalkeeping, shooting, and match logs.
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <form onSubmit={handleExtractData} className="space-y-4">
                      <div className="space-y-2">
                        <label htmlFor="url" className="text-sm font-medium">
                          Team Stats URL
                        </label>
                        <Input
                          id="url"
                          type="url"
                          placeholder="https://fbref.com/en/squads/.../Team-Name-Stats"
                          value={url}
                          onChange={(e) => setUrl(e.target.value)}
                          disabled={extractAllDataStatus === 'loading'}
                          required
                        />
                      </div>
                      
                      <div className="space-y-2">
                        <label htmlFor="id" className="text-sm font-medium">
                          League ID
                        </label>
                        <Input
                          id="id"
                          type="text"
                          placeholder="Enter league ID (e.g., 18bb7c10, 19538871)"
                          value={id}
                          onChange={(e) => setId(e.target.value)}
                          disabled={extractAllDataStatus === 'loading'}
                          required
                        />
                      </div>

                      <div className="flex gap-2 pt-2">
                        <Button 
                          type="submit" 
                          disabled={extractAllDataStatus === 'loading' || !url || !id}
                          className="flex items-center gap-2"
                        >
                          {extractAllDataStatus === 'loading' ? (
                            <>
                              <Loader2 className="h-4 w-4 animate-spin" />
                              Extracting Data...
                            </>
                          ) : (
                            <>
                              <Download className="h-4 w-4" />
                              Extract Data
                            </>
                          )}
                        </Button>
                        
                        <Button 
                          type="button" 
                          variant="outline" 
                          onClick={handleReset}
                          disabled={extractAllDataStatus === 'loading'}
                        >
                          Reset
                        </Button>
                      </div>
                    </form>
                  </CardContent>
                </Card>

                {/* Loading State */}
                {extractAllDataStatus === 'loading' && (
                  <Card>
                    <CardContent className="pt-6">
                      <div className="flex items-center justify-center py-8">
                        <div className="flex flex-col items-center gap-3">
                          <Loader2 className="h-8 w-8 animate-spin text-blue-500" />
                          <p className="text-muted-foreground">Extracting data from the provided URL...</p>
                          <p className="text-sm text-muted-foreground">This may take a few moments</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                )}

                {/* Error State */}
                {extractAllDataStatus === 'failed' && (
                  <Card className="border-red-200 bg-red-50">
                    <CardContent className="pt-6">
                      <div className="flex items-center gap-3 text-red-800">
                        <AlertCircle className="h-5 w-5" />
                        <div>
                          <p className="font-medium">Failed to extract data</p>
                          <p className="text-sm text-red-600 mt-1">{extractAllDataError}</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                )}

                {/* Success State */}
                {extractAllDataStatus === 'succeeded' && extractionData && (
                  <div className="space-y-6">
                    {/* Success Alert */}
                    <Card className="border-green-200 bg-green-50">
                      <CardContent className="pt-6">
                        <div className="flex items-center gap-3 text-green-800">
                          <CheckCircle className="h-5 w-5" />
                          <div>
                            <p className="font-medium">Data extracted successfully!</p>
                            <p className="text-sm text-green-600 mt-1">
                              Team data has been processed and is ready for download.
                            </p>
                          </div>
                        </div>
                      </CardContent>
                    </Card>

                    {/* Team Summary */}
                    <Card>
                      <CardHeader>
                        <CardTitle>Team Summary</CardTitle>
                        <CardDescription>
                          Overview of the extracted data for {extractionData.data.team_name}
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-blue-600">
                              {extractionData.data.players.length}
                            </div>
                            <div className="text-sm text-muted-foreground">Players</div>
                          </div>
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-green-600">
                              {extractionData.data.goalkeeping.length}
                            </div>
                            <div className="text-sm text-muted-foreground">Goalkeeping</div>
                          </div>
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-purple-600">
                              {extractionData.data.shooting.length}
                            </div>
                            <div className="text-sm text-muted-foreground">Shooting</div>
                          </div>
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-orange-600">
                              {extractionData.data.match_logs.length}
                            </div>
                            <div className="text-sm text-muted-foreground">Match Logs</div>
                          </div>
                        </div>
                      </CardContent>
                    </Card>

                    {/* Download Options */}
                    <Card>
                      <CardHeader>
                        <CardTitle>Download Options</CardTitle>
                        <CardDescription>
                          Choose your preferred format to download the extracted data
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        <div className="grid gap-4 md:grid-cols-2">
                          {/* JSON Download Card */}
                          <Card className="border-2">
                            <CardHeader className="pb-3">
                              <div className="flex items-center gap-2">
                                <FileJson className="h-6 w-6 text-blue-500" />
                                <CardTitle className="text-lg">JSON Format</CardTitle>
                              </div>
                              <CardDescription>
                                Single JSON file containing all team data
                              </CardDescription>
                            </CardHeader>
                            <CardContent>
                              <div className="space-y-3">
                                <div className="text-sm text-muted-foreground">
                                  <ul className="list-disc list-inside space-y-1">
                                    <li>Complete team data structure</li>
                                    <li>Easy to parse and process</li>
                                    <li>Ideal for data analysis</li>
                                  </ul>
                                </div>
                                <Button 
                                  onClick={handleDownloadJson}
                                  className="w-full flex items-center gap-2"
                                  size="lg"
                                >
                                  <Download className="h-4 w-4" />
                                  Download JSON
                                </Button>
                              </div>
                            </CardContent>
                          </Card>

                          {/* ZIP Download Card */}
                          <Card className="border-2">
                            <CardHeader className="pb-3">
                              <div className="flex items-center gap-2">
                                <FileArchive className="h-6 w-6 text-purple-500" />
                                <CardTitle className="text-lg">ZIP Archive</CardTitle>
                              </div>
                              <CardDescription>
                                Multiple JSON files organized by data type
                              </CardDescription>
                            </CardHeader>
                            <CardContent>
                              <div className="space-y-3">
                                <div className="text-sm text-muted-foreground">
                                  <ul className="list-disc list-inside space-y-1">
                                    <li>Separate files for each data type</li>
                                    <li>Better organization</li>
                                    <li>Ideal for backup</li>
                                  </ul>
                                </div>
                                <Button 
                                  onClick={handleDownloadZip}
                                  variant="outline"
                                  className="w-full flex items-center gap-2"
                                  size="lg"
                                >
                                  <Download className="h-4 w-4" />
                                  Download ZIP
                                </Button>
                              </div>
                            </CardContent>
                          </Card>
                        </div>
                      </CardContent>
                    </Card>

                    {/* Data Preview */}
                    <Card>
                      <CardHeader>
                        <CardTitle>Data Preview</CardTitle>
                        <CardDescription>
                          Quick overview of the extracted players data
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        <div className="rounded-md border">
                          <Table>
                            <TableHeader>
                              <TableRow>
                                <TableHead>Player Name</TableHead>
                                <TableHead>Position</TableHead>
                                <TableHead>Nation</TableHead>
                                <TableHead>Age</TableHead>
                                <TableHead>Matches</TableHead>
                                <TableHead>Goals</TableHead>
                                <TableHead>Assists</TableHead>
                              </TableRow>
                            </TableHeader>
                            <TableBody>
                              {extractionData.data.players.slice(0, 10).map((player, index) => (
                                // <TableRow key={player.player_id}>
                                <TableRow key={`${player.player_id}-${index}`}>
                                  <TableCell className="font-medium">{player.player_name}</TableCell>
                                  <TableCell>{player.position}</TableCell>
                                  <TableCell>{player.nation}</TableCell>
                                  <TableCell>{player.age}</TableCell>
                                  <TableCell>{player.match_played}</TableCell>
                                  <TableCell>{player.goals}</TableCell>
                                  <TableCell>{player.assists}</TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </div>
                        {extractionData.data.players.length > 10 && (
                          <div className="text-center mt-4 text-sm text-muted-foreground">
                            Showing first 10 of {extractionData.data.players.length} players
                          </div>
                        )}
                      </CardContent>
                    </Card>

                    {/* Extraction Details */}
                    <Card>
                      <CardHeader>
                        <CardTitle>Extraction Details</CardTitle>
                      </CardHeader>
                      <CardContent>
                        <div className="grid gap-4 text-sm">
                          <div className="flex justify-between">
                            <span className="text-muted-foreground">Team Name:</span>
                            <span className="font-medium">{extractionData.data.team_name}</span>
                          </div>
                          <Separator />
                          <div className="flex justify-between">
                            <span className="text-muted-foreground">League ID:</span>
                            <span className="font-medium">{extractionData.data.team_id}</span>
                          </div>
                          <Separator />
                          <div className="flex justify-between">
                            <span className="text-muted-foreground">Source URL:</span>
                            <span className="font-medium truncate max-w-xs">{extractionData.data.source_url}</span>
                          </div>
                          <Separator />
                          <div className="flex justify-between">
                            <span className="text-muted-foreground">Extracted At:</span>
                            <span className="font-medium">
                              {new Date(extractionData.data.extracted_at).toLocaleString()}
                            </span>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  </div>
                )}

                {/* Initial State Instructions */}
                {extractAllDataStatus === 'idle' && (
                  <Card>
                    <CardContent className="pt-6">
                      <div className="text-center py-8">
                        <div className="mx-auto w-24 h-24 bg-muted rounded-full flex items-center justify-center mb-4">
                          <Download className="h-10 w-10 text-muted-foreground" />
                        </div>
                        <h3 className="text-lg font-semibold mb-2">Ready to Extract Data</h3>
                        <p className="text-muted-foreground max-w-md mx-auto">
                          Enter a team stats URL and ID above to start extracting player data, 
                          goalkeeping stats, shooting data, and match logs.
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                )}
              </div>
            </div>
          </div>
        </div>
      </SidebarInset>
    </SidebarProvider>
  );
}