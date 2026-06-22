using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Modules.DataManagement.Services;
using ERPKeys.Application.Modules.Retail.Services;
using ERPKeys.Infrastructure.Persistence;
using ERPKeys.Infrastructure.Storage;
using ERPKeys.Worker.Jobs;
using ERPKeys.Worker.Workers;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

// Npgsql 8 legacy timestamp behaviour
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ── EF Core ───────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsql => npgsql.MigrationsAssembly("ERPKeys.Infrastructure")));

builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Worker runs without an HTTP context — org filters are bypassed (Guid.Empty = all orgs)
builder.Services.AddScoped<ICurrentOrganizationService, NoScopeOrganizationService>();

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IDataManagementService, DataManagementService>();
builder.Services.AddScoped<IBatchJobService, BatchJobService>();
builder.Services.AddScoped<IRetailStatementService, RetailStatementService>();

// ── Azure Blob Storage ────────────────────────────────────────────────────────
// Reads "AzureStorage:ConnectionString" from appsettings.json / environment variables.
// For Azure App Service, set this in Configuration → Application Settings.
// For local dev, use "UseDevelopmentStorage=true" with Azurite running.
builder.Services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();
builder.Services.AddSingleton<IFileShareService, AzureFileShareService>();

// ── Hangfire ──────────────────────────────────────────────────────────────────
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 4;
    options.ServerName  = "ERPKeys.Worker";
});

// ── Hosted workers ────────────────────────────────────────────────────────────
builder.Services.AddHostedService<BatchJobSchedulerWorker>();  // syncs DB → Hangfire every 60s
builder.Services.AddHostedService<FolderWatcherWorker>();       // optional: local folder fallback

// ── Hangfire job classes (DI) ─────────────────────────────────────────────────
builder.Services.AddScoped<ImportBatchJob>();
builder.Services.AddScoped<ImportBatchSweepJob>();
builder.Services.AddScoped<BatchImportJob>();
builder.Services.AddScoped<BatchExportJob>();

var host = builder.Build();

// Register the sweep job once at startup (not driven by DB config)
using (var scope = host.Services.CreateScope())
{
    var recurring = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurring.AddOrUpdate<ImportBatchSweepJob>(
        "sweep-queued-imports",
        j => j.SweepAsync(),
        Cron.Hourly());
}

await host.RunAsync();

// ── NoScopeOrganizationService ────────────────────────────────────────────────
public class NoScopeOrganizationService : ICurrentOrganizationService
{
    public Guid OrganizationId => Guid.Empty;
}
