using Chima.Api.Extensions;
using Chima.Api.Middleware;
using Chima.Infrastructure.Persistence;
using Chima.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ---- Serilog ----
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/chima-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

builder.Host.UseSerilog();

// ---- Startup validation (fail fast on bad config) ----
builder.ValidateConfiguration();

// ---- Upload limits ----
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 110 * 1024 * 1024; // 110 MB — allows 100MB file + overhead
    o.ValueLengthLimit = 1024 * 1024; // 1 MB per field
});

builder.WebHost.ConfigureKestrel(k =>
{
    k.Limits.MaxRequestBodySize = 110 * 1024 * 1024;
    k.AddServerHeader = false;
});

// ---- Services ----
builder.Services.AddControllers();
builder.Services.AddChimaPersistence(builder.Configuration);
builder.Services.AddChimaAuth(builder.Configuration);
builder.Services.AddChimaSwagger();
builder.Services.AddChimaCors(builder.Configuration);
builder.Services.AddChimaRateLimiting();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")!, name: "postgres");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(db, logger);
        logger.LogInformation("Database migrated and seeded.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration/seeding failed.");
        throw;
    }
}

// ---- Pipeline (order matters) ----
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseSerilogRequestLogging();
app.UseCors("Default");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chima API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });

app.Run();