"use client";

import { Separator } from "@/components/ui/separator";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { AppSidebar } from "@/components/app-sidebar";
import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";
import { SiteHeader } from "@/components/site-header";

export default function AboutPage() {
  const coreEndpoints = [
    { method: "GET", endpoint: "/api/league", description: "Get leagues" },
    { method: "GET", endpoint: "/api/club/league/{leagueId}/clubs", description: "Get clubs by league id" },
    { method: "GET", endpoint: "/api/club/{clubId}/trends", description: "Get club trend" },
    { method: "GET", endpoint: "/api/player/club/{clubId}/players", description: "Get players by club id" },
    { method: "GET", endpoint: "/api/player/club/{clubId}/players/current", description: "Get current players by club id" },
    { method: "GET", endpoint: "/api/player/{playerRefId}/season-comparisons", description: "Compare player with previous seasons" },
    { method: "GET", endpoint: "/api/player/{playerRefId}/current-previous-comparison", description: "Get player current vs previous season" },
    { method: "GET", endpoint: "/api/player/{playerRefId}/goalkeeping", description: "Get current goalkeeping by player" },
    { method: "GET", endpoint: "/api/player/{playerRefId}/shooting", description: "Get current shooting by player" },
  ];

  const crawlingEndpoints = [
    { method: "GET", endpoint: "/api/crawljobs/premier-league", description: "Get premier league data" },
    { method: "GET", endpoint: "/api/simplecrawler/players", description: "Extract players" },
    { method: "GET", endpoint: "/api/simplecrawler/goalkeeping", description: "Extract goalkeeping" },
    { method: "GET", endpoint: "/api/simplecrawler/shooting", description: "Extract shooting" },
    { method: "GET", endpoint: "/api/simplecrawler/match-logs", description: "Extract match logs" },
    { method: "GET", endpoint: "/api/simplecrawler/player-details", description: "Extract player details" },
    { method: "GET", endpoint: "/api/simplecrawler/raw-html", description: "Get raw HTML" },
    { method: "GET", endpoint: "/api/simplecrawler/all-data", description: "Extract all data" },
    { method: "GET", endpoint: "/api/simplecrawler/download-json", description: "Download JSON" },
    { method: "GET", endpoint: "/api/simplecrawler/download-zip", description: "Download ZIP" },
  ];

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
                <h1 className="text-3xl font-bold">⚽ Football Stats Platform</h1>
                <p className="text-muted-foreground mt-2">
                  A full-stack platform for collecting, processing, and visualizing top-tier football league statistics.
                </p>

                {/* Badges */}
                <div className="flex flex-wrap gap-2 mt-4">
                  <img src="https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet" alt=".NET 8" />
                  <img src="https://img.shields.io/badge/Next.js-14-000000?logo=nextdotjs" alt="Next.js 14" />
                  <img src="https://img.shields.io/badge/PostgreSQL-15-336791?logo=postgresql" alt="PostgreSQL 15" />
                  <img src="https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker" alt="Docker" />
                  <img src="https://img.shields.io/badge/License-MIT-green.svg" alt="MIT License" />
                </div>

                <Separator className="my-6" />

                {/* Features */}
                <h2 className="text-2xl font-bold">🚀 Features (Planned)</h2>
                <h3 className="text-xl font-semibold mt-4">🔄 Data Pipeline</h3>
                <ul className="list-disc pl-5 text-muted-foreground">
                  <li>Data Processing: Clean and transform raw football statistics</li>
                  <li>PostgreSQL Database: Optimized storage</li>
                </ul>

                <h3 className="text-xl font-semibold mt-4">🏗️ Backend (.NET 8)</h3>
                <ul className="list-disc pl-5 text-muted-foreground">
                  <li>Clean Architecture setup</li>
                  <li>Repository Pattern & Unit of Work initialized</li>
                  <li>RESTful API scaffolding</li>
                  <li>Entity Framework Core for database operations</li>
                  <li>Background Services for future scheduled tasks</li>
                </ul>

                <h3 className="text-xl font-semibold mt-4">🎨 Frontend (Next.js + TypeScript)</h3>
                <ul className="list-disc pl-5 text-muted-foreground">
                  <li>Modern React with TypeScript initialized</li>
                  <li>Responsive Dashboard placeholder</li>
                  <li>API Integration setup for future real-time updates</li>
                  <li>Component Testing with Testing Library</li>
                </ul>

                <h3 className="text-xl font-semibold mt-4">🛠️ DevOps & Infrastructure (Planned)</h3>
                <ul className="list-disc pl-5 text-muted-foreground">
                  <li>Docker containerization setup</li>
                  <li>CI/CD Pipeline with GitHub Actions (to be configured)</li>
                  <li>AWS EC2 deployment for backend (planned)</li>
                  <li>AWS RDS PostgreSQL (planned)</li>
                  <li>Vercel deployment for frontend (planned)</li>
                </ul>

                <Separator className="my-6" />

                {/* System Architecture */}
                <h2 className="text-2xl font-bold">📊 System Architecture (Planned)</h2>
                <pre className="bg-muted p-4 rounded-md text-sm overflow-x-auto">
                  {`
┌─────────────────┐ ┌──────────────────┐ ┌─────────────────┐
│ Next.js         │ │ .NET 8 API       │ │ PostgreSQL      │
│ Dashboard       │◄──►│ Backend         │◄──►│ Database       │
│ (Vercel)        │ │ (EC2)           │ │ (RDS)           │
└─────────────────┘ └──────────────────┘ └─────────────────┘
          │                  │                  │
          └───────────────────────┼───────────────────────┘
                                 │
                        ┌───────────┴───────────┐
                        │     FBref Crawler     │
                        │ (Scheduled Daily)     │
                        └───────────────────────┘
                  `}
                </pre>

                <Separator className="my-6" />

                {/* API Endpoints */}
                <h2 className="text-2xl font-bold">📡 API Endpoints</h2>
                
                <h3 className="text-xl font-semibold mt-4">Core Statistics</h3>
                <div className="rounded-md border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Method</TableHead>
                        <TableHead>Endpoint</TableHead>
                        <TableHead>Description</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {coreEndpoints.map((row, index) => (
                        <TableRow key={index}>
                          <TableCell className="font-medium">{row.method}</TableCell>
                          <TableCell><code className="bg-muted px-1 rounded">{row.endpoint}</code></TableCell>
                          <TableCell>{row.description}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>

                <h3 className="text-xl font-semibold mt-4">Data Crawling & Management</h3>
                <div className="rounded-md border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Method</TableHead>
                        <TableHead>Endpoint</TableHead>
                        <TableHead>Description</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {crawlingEndpoints.map((row, index) => (
                        <TableRow key={index}>
                          <TableCell className="font-medium">{row.method}</TableCell>
                          <TableCell><code className="bg-muted px-1 rounded">{row.endpoint}</code></TableCell>
                          <TableCell>{row.description}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>

                <Separator className="my-6" />

                {/* Quick Start */}
                <h2 className="text-2xl font-bold">🚀 Quick Start</h2>
                <h3 className="text-xl font-semibold mt-4">Prerequisites</h3>
                <ul className="list-disc pl-5 text-muted-foreground">
                  <li>.NET 8 SDK</li>
                  <li>Node.js 18+</li>
                  <li>PostgreSQL 15+</li>
                  <li>Docker & Docker Compose</li>
                </ul>

                <h3 className="text-xl font-semibold mt-4">Initial Setup</h3>
                <p className="text-muted-foreground">1. <strong>Clone the repository</strong></p>
                <pre className="bg-muted p-4 rounded-md text-sm">
                  <code>
                    {`git clone https://github.com/yourusername/football-stats-platform.git\ncd football-stats-platform`}
                  </code>
                </pre>
                <p className="text-muted-foreground mt-2">2. <strong>Run with Docker (Recommended)</strong></p>
                <pre className="bg-muted p-4 rounded-md text-sm">
                  <code>
                    {`docker-compose -f infra/docker/docker-compose.yml up --build`}
                  </code>
                </pre>
                <p className="text-muted-foreground mt-2">Access:</p>
                <ul className="list-disc pl-5 text-muted-foreground">
                  <li>Frontend: <a href="http://localhost:3000" className="text-blue-500">http://localhost:3000</a></li>
                  <li>Backend API: <a href="http://localhost:5000" className="text-blue-500">http://localhost:5000</a></li>
                  <li>Database: localhost:5432</li>
                </ul>

                <Separator className="my-6" />

                {/* League IDs */}
                <h2 className="text-2xl font-bold">🏆 League IDs</h2>
                <ul className="list-disc pl-5 text-muted-foreground">
                  <li>Premier League: 9</li>
                  <li>Serie A (Italy): 11</li>
                  <li>La Liga: 12</li>
                  <li>Primeira Liga (Portugal): 32</li>
                  <li>Ligue 1: 13</li>
                </ul>

                <Separator className="my-6" />

                {/* License */}
                <h2 className="text-2xl font-bold">📜 License</h2>
                <p className="text-muted-foreground">
                  This project is licensed under the MIT License. See the{" "}
                  <a href="/LICENSE" className="text-blue-500">LICENSE</a> file for details.
                </p>
              </div>
            </div>
          </div>
        </div>
      </SidebarInset>
    </SidebarProvider>
  );
}