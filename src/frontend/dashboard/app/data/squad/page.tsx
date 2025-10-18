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

export default function SquadStandardExtractionPage() {
  const {
    extractSquadStandard,
    extractedSquadStandard,
    extractSquadStandardStatus,
    extractSquadStandardError,
    downloadSquadStandardJson,
    downloadSquadStandardZip,
    downloadSquadStandardStatus,
    downloadSquadStandardError,
    resetExtractSquadStandard
  } = useFootball();

  const [url, setUrl] = useState('');
  const [selector, setSelector] = useState('//table[@id="stats_squads_standard_for"]');

  // Debug log to inspect the response shape
  // console.log('Extracted Squad Standard Response:', extractedSquadStandard);
  // console.log('Status:', extractSquadStandardStatus);
  // console.log('Download Status:', downloadSquadStandardStatus);
  // console.log('Download Error:', downloadSquadStandardError);

  const handleExtractData = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!url) {
      toast.error('Please provide the URL');
      return;
    }

    try {
      await extractSquadStandard(url, selector).unwrap();
      toast.success('Squad standard data extracted successfully!');
    } catch (error) {
      toast.error('Failed to extract data');
    }
  };

  const handleDownloadJson = async () => {
    if (!extractedSquadStandard?.downloadLinks?.json) {
      toast.error('No JSON download link available');
      return;
    }

    try {
      const jsonUrl = new URL(extractedSquadStandard.downloadLinks.json);
      const params = new URLSearchParams(jsonUrl.search);
      const url = params.get('url');
      const selector = params.get('selector') || undefined;

      if (!url) {
        toast.error('Invalid download link: URL parameter missing');
        return;
      }

      await downloadSquadStandardJson(url, selector).unwrap();
      toast.success('JSON downloaded successfully');
    } catch (error) {
      toast.error(downloadSquadStandardError || 'Failed to download JSON');
    }
  };

  const handleDownloadZip = async () => {
    if (!extractedSquadStandard?.downloadLinks?.zip) {
      toast.error('No ZIP download link available');
      return;
    }

    try {
      const zipUrl = new URL(extractedSquadStandard.downloadLinks.zip);
      const params = new URLSearchParams(zipUrl.search);
      const url = params.get('url');
      const selector = params.get('selector') || undefined;

      if (!url) {
        toast.error('Invalid download link: URL parameter missing');
        return;
      }

      await downloadSquadStandardZip(url, selector).unwrap();
      toast.success('ZIP downloaded successfully');
    } catch (error) {
      toast.error(downloadSquadStandardError || 'Failed to download ZIP');
    }
  };

  const handleReset = () => {
    setUrl('');
    setSelector('//table[@id="stats_squads_standard_for"]');
    resetExtractSquadStandard();
  };

  // Safely access data with null checks
  const dataArray = extractSquadStandardStatus === 'succeeded' && Array.isArray(extractedSquadStandard?.data)
    ? extractedSquadStandard.data
    : [];

  const firstSquad = dataArray[0] || null;

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
                  <h1 className="text-3xl font-bold">Squad Standard Extraction</h1>
                  <Badge variant={extractSquadStandardStatus === 'succeeded' ? "default" : "secondary"}>
                    {extractSquadStandardStatus === 'idle' && 'Ready'}
                    {extractSquadStandardStatus === 'loading' && 'Extracting...'}
                    {extractSquadStandardStatus === 'succeeded' && 'Success'}
                    {extractSquadStandardStatus === 'failed' && 'Failed'}
                  </Badge>
                </div>

                {/* Input Form */}
                <Card className="mb-6">
                  <CardHeader>
                    <CardTitle>Extract Squad Standard Stats</CardTitle>
                    <CardDescription>
                      Enter the squad stats URL to extract standard squad statistics like goals, assists, possession, etc.
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <form onSubmit={handleExtractData} className="space-y-4">
                      <div className="space-y-2">
                        <label htmlFor="url" className="text-sm font-medium">
                          Squad Stats URL
                        </label>
                        <Input
                          id="url"
                          type="url"
                          placeholder="https://fbref.com/en/squads/.../Team-Name-Stats"
                          value={url}
                          onChange={(e) => setUrl(e.target.value)}
                          disabled={extractSquadStandardStatus === 'loading' || downloadSquadStandardStatus === 'loading'}
                          required
                        />
                      </div>
                      
                      <div className="space-y-2">
                        <label htmlFor="selector" className="text-sm font-medium">
                          XPath Selector (optional)
                        </label>
                        <Input
                          id="selector"
                          type="text"
                          placeholder="//table[@id='stats_squads_standard_for']"
                          value={selector}
                          onChange={(e) => setSelector(e.target.value)}
                          disabled={extractSquadStandardStatus === 'loading' || downloadSquadStandardStatus === 'loading'}
                        />
                      </div>

                      <div className="flex gap-2 pt-2">
                        <Button 
                          type="submit" 
                          disabled={extractSquadStandardStatus === 'loading' || downloadSquadStandardStatus === 'loading' || !url}
                          className="flex items-center gap-2"
                        >
                          {extractSquadStandardStatus === 'loading' ? (
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
                          disabled={extractSquadStandardStatus === 'loading' || downloadSquadStandardStatus === 'loading'}
                        >
                          Reset
                        </Button>
                      </div>
                    </form>
                  </CardContent>
                </Card>

                {/* Loading State */}
                {extractSquadStandardStatus === 'loading' && (
                  <Card>
                    <CardContent className="pt-6">
                      <div className="flex items-center justify-center py-8">
                        <div className="flex flex-col items-center gap-3">
                          <Loader2 className="h-8 w-8 animate-spin text-blue-500" />
                          <p className="text-muted-foreground">Extracting squad standard data from the provided URL...</p>
                          <p className="text-sm text-muted-foreground">This may take a few moments</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                )}

                {/* Download Loading State */}
                {downloadSquadStandardStatus === 'loading' && (
                  <Card>
                    <CardContent className="pt-6">
                      <div className="flex items-center justify-center py-8">
                        <div className="flex flex-col items-center gap-3">
                          <Loader2 className="h-8 w-8 animate-spin text-blue-500" />
                          <p className="text-muted-foreground">Downloading squad standard data...</p>
                          <p className="text-sm text-muted-foreground">This may take a moment</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                )}

                {/* Error State */}
                {(extractSquadStandardStatus === 'failed' || downloadSquadStandardStatus === 'failed') && (
                  <Card className="border-red-200 bg-red-50">
                    <CardContent className="pt-6">
                      <div className="flex items-center gap-3 text-red-800">
                        <AlertCircle className="h-5 w-5" />
                        <div>
                          <p className="font-medium">Operation failed</p>
                          <p className="text-sm text-red-600 mt-1">
                            {extractSquadStandardStatus === 'failed' ? extractSquadStandardError : downloadSquadStandardError || 'Unknown error'}
                          </p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                )}

                {/* Success State */}
                {extractSquadStandardStatus === 'succeeded' && dataArray.length > 0 && (
                  <div className="space-y-6">
                    {/* Success Alert */}
                    <Card className="border-green-200 bg-green-50">
                      <CardContent className="pt-6">
                        <div className="flex items-center gap-3 text-green-800">
                          <CheckCircle className="h-5 w-5" />
                          <div>
                            <p className="font-medium">Squad data extracted successfully!</p>
                            <p className="text-sm text-green-600 mt-1">
                              {dataArray.length} squad records processed and ready for download.
                            </p>
                          </div>
                        </div>
                      </CardContent>
                    </Card>

                    {/* Squad Summary */}
                    <Card>
                      <CardHeader>
                        <CardTitle>Squad Summary</CardTitle>
                        <CardDescription>
                          Overview of the extracted squad standard statistics
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-blue-600">
                              {firstSquad?.number_of_players || 0}
                            </div>
                            <div className="text-sm text-muted-foreground">Players</div>
                          </div>
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-green-600">
                              {firstSquad?.goals || 0}
                            </div>
                            <div className="text-sm text-muted-foreground">Goals</div>
                          </div>
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-purple-600">
                              {firstSquad?.assists || 0}
                            </div>
                            <div className="text-sm text-muted-foreground">Assists</div>
                          </div>
                          <div className="text-center p-4 border rounded-lg">
                            <div className="text-2xl font-bold text-orange-600">
                              {firstSquad?.possession !== undefined ? `${firstSquad.possession.toFixed(1)}%` : 'N/A'}
                            </div>
                            <div className="text-sm text-muted-foreground">Possession</div>
                          </div>
                        </div>
                      </CardContent>
                    </Card>

                    {/* Download Options */}
                    <Card>
                      <CardHeader>
                        <CardTitle>Download Options</CardTitle>
                        <CardDescription>
                          Download the extracted squad data in your preferred format
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
                                JSON file containing squad standard stats
                              </CardDescription>
                            </CardHeader>
                            <CardContent>
                              <Button 
                                onClick={handleDownloadJson}
                                className="w-full flex items-center gap-2"
                                size="lg"
                                disabled={downloadSquadStandardStatus === 'loading'}
                              >
                                {downloadSquadStandardStatus === 'loading' ? (
                                  <>
                                    <Loader2 className="h-4 w-4 animate-spin" />
                                    Downloading...
                                  </>
                                ) : (
                                  <>
                                    <Download className="h-4 w-4" />
                                    Download JSON
                                  </>
                                )}
                              </Button>
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
                                ZIP file with squad data
                              </CardDescription>
                            </CardHeader>
                            <CardContent>
                              <Button 
                                onClick={handleDownloadZip}
                                variant="outline"
                                className="w-full flex items-center gap-2"
                                size="lg"
                                disabled={downloadSquadStandardStatus === 'loading'}
                              >
                                {downloadSquadStandardStatus === 'loading' ? (
                                  <>
                                    <Loader2 className="h-4 w-4 animate-spin" />
                                    Downloading...
                                  </>
                                ) : (
                                  <>
                                    <Download className="h-4 w-4" />
                                    Download ZIP
                                  </>
                                )}
                              </Button>
                            </CardContent>
                          </Card>
                        </div>
                      </CardContent>
                    </Card>

                    {/* Data Preview Table */}
                    <Card>
                      <CardHeader>
                        <CardTitle>Data Preview</CardTitle>
                        <CardDescription>
                          Preview of extracted squad standard statistics
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        <div className="rounded-md border overflow-x-auto">
                          <Table>
                            <TableHeader>
                              <TableRow>
                                <TableHead>Squad</TableHead>
                                <TableHead>Players</TableHead>
                                <TableHead>Avg Age</TableHead>
                                <TableHead>Possession</TableHead>
                                <TableHead>MP</TableHead>
                                <TableHead>Starts</TableHead>
                                <TableHead>Min</TableHead>
                                <TableHead>90s</TableHead>
                                <TableHead>Gls</TableHead>
                                <TableHead>Ast</TableHead>
                                <TableHead>G+A</TableHead>
                                <TableHead>G-PK</TableHead>
                                <TableHead>PK</TableHead>
                                <TableHead>PKatt</TableHead>
                                <TableHead>YC</TableHead>
                                <TableHead>RC</TableHead>
                                <TableHead>xG</TableHead>
                                <TableHead>npxG</TableHead>
                                <TableHead>xAG</TableHead>
                                <TableHead>PrgC</TableHead>
                                <TableHead>PrgP</TableHead>
                              </TableRow>
                            </TableHeader>
                            <TableBody>
                              {dataArray.map((squad, index) => (
                                <TableRow key={`${squad.squad}-${index}`}>
                                  <TableCell className="font-medium">{squad.squad || 'N/A'}</TableCell>
                                  <TableCell>{squad.number_of_players ?? 0}</TableCell>
                                  <TableCell>{typeof squad.average_age === 'number' ? squad.average_age.toFixed(1) : 'N/A'}</TableCell>
                                  <TableCell>{typeof squad.possession === 'number' ? `${squad.possession.toFixed(1)}%` : 'N/A'}</TableCell>
                                  <TableCell>{squad.matches_played ?? 0}</TableCell>
                                  <TableCell>{squad.starts ?? 0}</TableCell>
                                  <TableCell>{squad.minutes ?? 0}</TableCell>
                                  <TableCell>{typeof squad.nineties === 'number' ? squad.nineties.toFixed(1) : 'N/A'}</TableCell>
                                  <TableCell>{squad.goals ?? 0}</TableCell>
                                  <TableCell>{squad.assists ?? 0}</TableCell>
                                  <TableCell>{squad.goals_plus_assists ?? 0}</TableCell>
                                  <TableCell>{squad.non_penalty_goals ?? 0}</TableCell>
                                  <TableCell>{squad.penalty_kicks_made ?? 0}</TableCell>
                                  <TableCell>{squad.penalty_kicks_attempted ?? 0}</TableCell>
                                  <TableCell>{squad.yellow_cards ?? 0}</TableCell>
                                  <TableCell>{squad.red_cards ?? 0}</TableCell>
                                  <TableCell>{typeof squad.expected_goals === 'number' ? squad.expected_goals.toFixed(1) : 'N/A'}</TableCell>
                                  <TableCell>{typeof squad.non_penalty_expected_goals === 'number' ? squad.non_penalty_expected_goals.toFixed(1) : 'N/A'}</TableCell>
                                  <TableCell>{typeof squad.expected_assisted_goals === 'number' ? squad.expected_assisted_goals.toFixed(1) : 'N/A'}</TableCell>
                                  <TableCell>{squad.progressive_carries ?? 0}</TableCell>
                                  <TableCell>{squad.progressive_passes ?? 0}</TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </div>
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
                            <span className="text-muted-foreground">Source URL:</span>
                            <span className="font-medium truncate max-w-xs">{url}</span>
                          </div>
                          <Separator />
                          <div className="flex justify-between">
                            <span className="text-muted-foreground">XPath Selector:</span>
                            <span className="font-medium">{selector}</span>
                          </div>
                          <Separator />
                          <div className="flex justify-between">
                            <span className="text-muted-foreground">Records Extracted:</span>
                            <span className="font-medium">{dataArray.length}</span>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  </div>
                )}

                {/* Fallback Message if Succeeded but No Data */}
                {extractSquadStandardStatus === 'succeeded' && dataArray.length === 0 && (
                  <Card className="border-yellow-200 bg-yellow-50">
                    <CardContent className="pt-6">
                      <div className="flex items-center gap-3 text-yellow-800">
                        <AlertCircle className="h-5 w-5" />
                        <div>
                          <p className="font-medium">Extraction succeeded but no data found</p>
                          <p className="text-sm mt-1">Check the raw response above or try a different URL/selector.</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                )}

                {/* Initial State Instructions */}
                {extractSquadStandardStatus === 'idle' && (
                  <Card>
                    <CardContent className="pt-6">
                      <div className="text-center py-8">
                        <div className="mx-auto w-24 h-24 bg-muted rounded-full flex items-center justify-center mb-4">
                          <Download className="h-10 w-10 text-muted-foreground" />
                        </div>
                        <h3 className="text-lg font-semibold mb-2">Ready to Extract Squad Data</h3>
                        <p className="text-muted-foreground max-w-md mx-auto">
                          Enter a squad stats URL above to start extracting standard squad statistics including goals, assists, possession, and advanced metrics.
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