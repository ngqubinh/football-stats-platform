FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FSP/FSP.sln", "."]
COPY ["src/FSP/FSP.WebApi/FSP.WebApi.csproj", "FSP.WebApi/"]
COPY ["src/FSP/FSP.Application/FSP.Application.csproj", "FSP.Application/"]
COPY ["src/FSP/FSP.Domain/FSP.Domain.csproj", "FSP.Domain/"]
COPY ["src/FSP/FSP.Infrastructure/FSP.Infrastructure.csproj", "FSP.Infrastructure/"]
RUN dotnet restore "FSP.sln"

COPY . .
WORKDIR "/src/src/FSP"
RUN dotnet build "FSP.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FSP.WebApi/FSP.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FSP.WebApi.dll"]