using FSP.Infrastructure.Data;
using FSP.Infrastructure.DI;
using FSP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// EXPLICITLY set the URLs for Docker
builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.MainService(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Logging configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

Log.Information("Application starting...");

try
{
    // Database initialization
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await DatabaseInitializer.Initialize(dbContext);
        Log.Information("Database initialization completed");
    }

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    app.UseCors("client_cors");
    app.MapControllers();
    app.MapHealthChecks("/health");

    Log.Information("All middleware configured, starting application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}