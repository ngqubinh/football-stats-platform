# ⚽ Football Stats Platform

A full-stack platform for collecting, processing, and visualizing top-tier football league statistics. The project is currently in its initial setup phase, establishing the foundation for data crawling, RESTful APIs, and an interactive dashboard.

![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)
![Next.js](https://img.shields.io/badge/Next.js-14-000000?logo=nextdotjs) 
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)
![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)

## 🚀 Features (Planned)

### 🔄 Data Pipeline
- **Data Processing**: Clean and transform raw football statistics
- **PostgreSQL Database**: Optimized storage

### 🏗️ Backend (.NET 8)
- **Clean Architecture** setup
- **Repository Pattern** & **Unit of Work** initialized
- **RESTful API** scaffolding
- **Entity Framework Core** for database operations
- **Background Services** for future scheduled tasks

### 🎨 Frontend (Next.js + TypeScript)
- **Modern React** with TypeScript initialized
- **Responsive Dashboard** placeholder
- **API Integration** setup for future real-time updates

### 🛠️ DevOps & Infrastructure (Planned)
- **Docker** containerization setup
- **CI/CD Pipeline** with GitHub Actions (to be configured)
- **AWS EC2** deployment for backend (planned)
- **AWS RDS** PostgreSQL (planned)
- **Vercel** deployment for frontend (planned)

## 📊 System Architecture (Planned)
```
┌─────────────────┐ ┌──────────────────┐ ┌─────────────────┐
│ Next.js         │ │ .NET 8 API       │ │ PostgreSQL      │
│ Dashboard       │◄──►│ Backend         │◄──►│ Database       │
│ (Vercel)        │ │ (EC2)           │ │ (RDS)           │
└─────────────────┘ └──────────────────┘ └─────────────────┘
          │                  │                  │
          │                  │                  │
          └───────────────────────┼───────────────────────┘
                                 │
                        ┌───────────┴───────────┐
                        │     FBref Crawler     │
                        │ (Scheduled Daily)     │
                        └───────────────────────┘
```

## 🏗️ Project Structure
```
football-stats-platform/
├── 📁 src/
│   ├── 📁 FSF/                    # .NET 8 Clean Architecture
│   │   ├── FSF.WebApi/            # Presentation Layer
│   │   ├── FSF.Application/       # Use Cases & DTOs
│   │   ├── FSF.Domain/            # Domain Models & Interfaces
│   │   ├── FSF.Infrastructure/    # Data & External Services
│   │   └── FSF.Tests/             # Unit & Integration Tests
│   ├── 📁 frontend/               # Next.js + TypeScript
├── 📁 infra/
│   ├── docker/                    # Docker Configuration
│   ├── cicd/                      # GitHub Actions Workflows
│   └── aws/                       # AWS Deployment Scripts
└── 📁 docs/                      # Documentation & Diagrams
```

## 🛠️ Technology Stack
| Layer         | Technology                                    |
|---------------|-----------------------------------------------|
| **Frontend**  | Next.js 14, TypeScript, Tailwind CSS          |
| **Backend**   | .NET 8, ASP.NET Core, Entity Framework Core   |
| **Database**  | PostgreSQL, EF Core Migrations                |
| **Testing**   | xUnit, Moq, React Testing Library, Jest       |
| **DevOps**    | Docker, GitHub Actions, AWS EC2, AWS RDS, Vercel |
| **Architecture** | Clean Architecture, Repository Pattern     |

## 🚀 Quick Start
### Prerequisites
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 15+
- Docker & Docker Compose

### Initial Setup
1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/football-stats-platform.git
   cd football-stats-platform
   ```

2. **Run with Docker (Recommended)**
   ```bash
   # Build and start all services using docker-compose
   docker compose -f infra/docker/docker-compose.yml up --build

   # Alternatively, build and run only the backend manually (if Dockerfile is not yet configured for port 5000):
   docker build -f infra/docker/FSF.Dockerfile -t football-stats-backend .
   docker run -d -p 5000:8080 -e ASPNETCORE_ENVIRONMENT=Development --name football-backend football-stats-backend

   # If Dockerfile is configured with ENV ASPNETCORE_URLS=http://+:5000:
   docker build -f infra/docker/FSF.Dockerfile -t football-stats-backend .
   docker run -d -p 5000:5000 -e ASPNETCORE_ENVIRONMENT=Development --name football-backend football-stats-backend
   ```

   Access:
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Database: localhost:5432

3. **Manual Setup**
   ```bash
   # Backend
   cd src/FSF
   dotnet restore
   dotnet run --project FSF.WebApi

   # Frontend
   cd src/frontend
   npm install
   npm run dev
   ```

## 📡 API Endpoints

The backend provides RESTful APIs for accessing football statistics and data crawling operations.

### Core Statistics
| Method | Endpoint | Description |
|--------|-----------|-------------|
| GET | `/api/league` | Get leagues |
| GET | `/api/club/league/{leagueId}/clubs` | Get clubs by league id |
| GET | `/api/club/{clubId}/trends` | Get club trend |
| GET | `/api/player/club/{clubId}/players` | Get players by club id |
| GET | `/api/player/club/{clubId}/players/current` | Get current players by club id |
| GET | `/api/player/{playerRefId}/season-comparisons` | Compare player with previous seasons |
| GET | `/api/player/{playerRefId}/current-previous-comparison` | Get player current vs previous season |
| GET | `/api/player/{playerRefId}/goalkeeping` | Get current goalkeeping by player |
| GET | `/api/player/{playerRefId}/shooting` | Get current shooting by player |

### Data Crawling & Management
| Method | Endpoint | Description |
|--------|-----------|-------------|
| GET | `/api/crawljobs/premier-league` | Get premier league data |
| GET | `/api/simplecrawler/players` | Extract players data |
| GET | `/api/simplecrawler/goalkeeping` | Extract goalkeeping data |
| GET | `/api/simplecrawler/shooting` | Extract shooting data |
| GET | `/api/simplecrawler/match-logs` | Extract match logs |
| GET | `/api/simplecrawler/player-details` | Extract player details |
| GET | `/api/simplecrawler/raw-html` | Get raw HTML |
| GET | `/api/simplecrawler/all-data` | Extract all data |
| GET | `/api/simplecrawler/download-json` | Download JSON |
| GET | `/api/simplecrawler/download-zip` | Download ZIP |

API documentation is available at `http://localhost:5000/swagger` when the backend is running.

## 🧪 Running Tests
### Backend Tests
```bash
cd src/FSF/FSF.Tests
dotnet test
```

### Frontend Tests
```bash
cd src/frontend
npm run test
```

## 🚀 Deployment (Planned)
### Backend (AWS EC2)
1. Build and push the Docker image:
   ```bash
   docker build -t football-stats-api ./src/FSF
   docker tag football-stats-api your-ecr-repo:tag
   docker push your-ecr-repo:tag
   ```
2. Deploy to EC2 using scripts in `infra/aws` (to be implemented).

### Frontend (Vercel)
1. Connect the frontend repository to Vercel.
2. Deploy using Vercel CLI:
   ```bash
   cd src/frontend
   vercel --prod
   ```

### Database (AWS RDS)
1. Set up a PostgreSQL instance on AWS RDS.
2. Apply migrations:
   ```bash
   cd src/FSF
   dotnet ef database update --project FSF.Infrastructure
   ```

## 📜 License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.