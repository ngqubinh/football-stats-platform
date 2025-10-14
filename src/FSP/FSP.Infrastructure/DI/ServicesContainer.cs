using FSP.Application.Mappings;
using FSP.Application.Services;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using FSP.Domain.Interfaces.RepositoryPattern;
using FSP.Infrastructure.Data;
using FSP.Infrastructure.Implementations;
using FSP.Infrastructure.Implementations.RepositoryPattern;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FSP.Infrastructure.DI;

public static class ServicesContainer
{
    public static IServiceCollection MainService(this IServiceCollection services, IConfiguration config)
    {
		// Database
		string connectionString = config.GetValue<string>("ConnectionStrings:postgreSQL") 
			?? throw new InvalidOperationException("PostgreSQL connection string is missing");

		services.AddDbContext<ApplicationDbContext>(options => 
			options.UseNpgsql(connectionString));

        // CORS
		services.AddCors(options =>
		{
			options.AddPolicy("client_cors", builder =>
			{
				builder.WithOrigins("*")
					.AllowAnyHeader()
					.AllowAnyMethod();
				//.AllowCredentials();
			});
		});

		services.AddRazorPages();

		// Add SignalR service
		services.AddSignalR(options =>
		{
			options.EnableDetailedErrors = true;
			options.MaximumReceiveMessageSize = 102400; // ~100KB
		});

		// Services
		services.AddHttpClient();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		// Generic
		services.AddScoped<IGenericRepo<League>, GenericRepo<League>>();
		services.AddScoped<IGenericRepo<Club>, GenericRepo<Club>>();
		services.AddScoped<IGenericRepo<Player>, GenericRepo<Player>>();

		// Core
		services.AddScoped<ICrawlingService, CrawlingService>();
		services.AddScoped<ICoreMappingService, CoreMappingService>();
		services.AddScoped<ICrawlingAppService, CrawlingAppService>();
		services.AddScoped<ISimpleCrawlerService, SimpleCrawlerService>();
		services.AddScoped<ISimpleCrawlerAppService, SimpleCrawlerAppService>();
		services.AddScoped<IHtmlParserService, HtmlParserService>();
		services.AddScoped<IDataStorageService, DataStorageService>();
		services.AddScoped<IImportService, ImportService>();
		services.AddScoped<IFootballService, FootballService>();

		// League DI
		services.AddScoped<ILeagueMappingService, LeagueMappingService>();
		services.AddScoped<ILeagueAppService, LeagueAppService>();

		// Club DI
		services.AddScoped<IClubMappingService, ClubMappingService>();
		services.AddScoped<IClubAppService, ClubAppService>();

		// Player DI
		services.AddScoped<IPlayerMappingService, PlayerMappingService>();
		services.AddScoped<IPlayerAppService, PlayerAppService>();

		// Match Log DI
		services.AddScoped<IMatchLogMappingService, MatchLogMappingService>();

		return services;
    }
}