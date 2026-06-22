using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;
using Microsoft.OpenApi.Models;



using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Common.Security;
using ERPKeys.Application.Common.Services;
using ERPKeys.Application.Modules.AccountsPayable.Services;
using ERPKeys.Application.Modules.AccountsReceivable.Services;
using ERPKeys.Application.Modules.DataManagement.Services;
using ERPKeys.Application.Modules.GeneralLedger.Services;
using ERPKeys.Application.Modules.Organization.Services;
using ERPKeys.Application.Modules.ProductManagement.Services;
using ERPKeys.Application.Modules.InventoryManagement.Services;
using ERPKeys.Application.Modules.Workflow.Services;
using ERPKeys.Application.Modules.Expenses.Services;
using ERPKeys.Application.Modules.CashBank;
using ERPKeys.Application.Modules.FixedAssets;
using ERPKeys.Application.Modules.WarehouseManagement;
using ERPKeys.Application.Modules.Marketing.Services;
using ERPKeys.Application.Modules.Retail.Services;
using ERPKeys.Application.Modules.SystemAdmin.Services;
using ERPKeys.Infrastructure.Persistence;
using ERPKeys.Infrastructure.Persistence.Seed;
using ERPKeys.Infrastructure.Services;
using ERPKeys.Infrastructure.Storage;
using ERPKeys.Worker.Jobs;
using ERPKeys.Worker.Workers;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
//using Microsoft.OpenApi.Models;



// ── Npgsql: treat DateTime(Unspecified) as UTC globally ──────────────────────
// Angular sends dates as ISO strings (e.g. "2026-06-02") which .NET deserializes
// as Kind=Unspecified. Npgsql 6+ requires Kind=Utc for timestamptz columns.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ── HTTP Context (needed for CurrentOrganizationService) ─────────────────────
builder.Services.AddHttpContextAccessor();


// ── Database ──────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString,
            npgsql => npgsql.MigrationsAssembly("ERPKeys.Infrastructure"))
        .UseSnakeCaseNamingConvention()
        .ReplaceService<IHistoryRepository, LegacyNamingHistoryRepository>());

builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// ── Organization scope ────────────────────────────────────────────────────────
builder.Services.AddScoped<ICurrentOrganizationService, CurrentOrganizationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDocumentAuditService, DocumentAuditService>();

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IGeneralLedgerService, GeneralLedgerService>();
builder.Services.AddScoped<IAccountsReceivableService, AccountsReceivableService>();
builder.Services.AddScoped<IAccountsPayableService, AccountsPayableService>();
builder.Services.AddScoped<IProductManagementService, ProductManagementService>();
builder.Services.AddScoped<IInventoryManagementService, InventoryManagementService>();
builder.Services.AddScoped<IPurchaseInventoryPostingService, PurchaseInventoryPostingService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IWarehouseManagementService, WarehouseManagementService>();
builder.Services.AddScoped<ICashBankService, CashBankService>();
builder.Services.AddScoped<IFixedAssetService, FixedAssetService>();

// ── Data Management ───────────────────────────────────────────────────────────
builder.Services.AddScoped<IDataManagementService, DataManagementService>();
builder.Services.AddScoped<IBatchJobService, BatchJobService>();
builder.Services.AddScoped<IRetailService, RetailService>();
builder.Services.AddScoped<IRetailStatementService, RetailStatementService>();
builder.Services.AddScoped<IMarketingService, MarketingService>();
builder.Services.AddScoped<IPriceAgreementService, PriceAgreementService>();
builder.Services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();
builder.Services.AddSingleton<IFileShareService, AzureFileShareService>();

// ── Hangfire (server runs inside the API process) ─────────────────────────────
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 4;
    options.ServerName  = "ERPKeys.Api.HangfireServer";
});

// Batch job hosted services and job classes
builder.Services.AddHostedService<BatchJobSchedulerWorker>(); // syncs DB → Hangfire every 60s
builder.Services.AddScoped<BatchImportJob>();
builder.Services.AddScoped<BatchExportJob>();
builder.Services.AddScoped<ImportBatchJob>();
builder.Services.AddScoped<ImportBatchSweepJob>();

// ── System Admin services ─────────────────────────────────────────────────────
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISystemAdminService, SystemAdminService>();
builder.Services.AddHttpClient<IAddressLookupService, GoogleAddressLookupService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("address-lookup", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? context.Connection.RemoteIpAddress?.ToString()
                ?? "anonymous",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtSecret  = jwtSection["Secret"]   ?? throw new InvalidOperationException("JwtSettings:Secret missing.");
var jwtIssuer  = jwtSection["Issuer"]   ?? "ERPKeys";
var jwtAudience= jwtSection["Audience"] ?? "ERPKeys";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer           = true,
        ValidIssuer              = jwtIssuer,
        ValidateAudience         = true,
        ValidAudience            = jwtAudience,
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in PermissionCatalog.PolicyKeys)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("Admin") ||
                context.User.HasClaim("permission", permission)));
    }
});

// ── API ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

// ── Swagger with JWT support ──────────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ERP Keys API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter your JWT token (without the 'Bearer ' prefix)."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// ── CORS (Angular dev server) ─────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray() ?? [];

if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "At least one CORS origin must be configured in Cors:AllowedOrigins.");
}

builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

// ── Seed database + register Hangfire sweep job ───────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (app.Environment.IsDevelopment() &&
        builder.Configuration.GetValue("DatabaseInitialization:ApplyMigrationsOnStartup", false))
    {
        var startedAt = System.Diagnostics.Stopwatch.GetTimestamp();
        await db.Database.MigrateAsync();
        logger.LogInformation(
            "Database migrations completed in {ElapsedMs} ms.",
            System.Diagnostics.Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
    }

    if (builder.Configuration.GetValue(
            "DatabaseInitialization:SeedOnStartup",
            app.Environment.IsDevelopment()))
    {
        var startedAt = System.Diagnostics.Stopwatch.GetTimestamp();
        await DatabaseSeeder.SeedAsync(db, logger);
        logger.LogInformation(
            "Database seeding completed in {ElapsedMs} ms.",
            System.Diagnostics.Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
    }

    // Register the import sweep job once at startup
    var recurring = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurring.AddOrUpdate<ImportBatchSweepJob>(
        "sweep-queued-imports",
        j => j.SweepAsync(),
        Cron.Hourly());
}

// ── Middleware ────────────────────────────────────────────────────────────────
//app.UseDeveloperExceptionPage(); // full stack trace in response — remove before production

// Keep CORS outside the exception handler so handled application errors also
// include the appropriate Access-Control-Allow-Origin response header.
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exception = context.Features
                .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
                ?.Error;
            var logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("GlobalExceptionHandler");

            if (exception is not null)
                logger.LogError(exception, "Unhandled exception while processing {Method} {Path}.",
                    context.Request.Method, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected server error occurred.",
                traceId = context.TraceIdentifier
            });
        });
    });
    app.UseHsts();
}

app.UseAuthentication();
app.UseRateLimiter();

// Enforce organization access on every organization-scoped request. Admin users
// may work in any active organization; other users are restricted to the
// organization assigned to their account.
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true &&
        context.Request.Headers.TryGetValue("X-Organization-Id", out var organizationHeader))
    {
        if (!Guid.TryParse(organizationHeader.FirstOrDefault(), out var requestedOrganizationId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "The selected organization is invalid." });
            return;
        }

        var assignedOrganizationId = Guid.TryParse(
            context.User.FindFirstValue("orgId"), out var assignedId)
            ? assignedId
            : Guid.Empty;
        var hasAllOrganizationAccess = context.User.IsInRole("Admin");

        if (!hasAllOrganizationAccess && requestedOrganizationId != assignedOrganizationId)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "You do not have access to the selected organization."
            });
            return;
        }

        var db = context.RequestServices.GetRequiredService<AppDbContext>();
        var organizationIsActive = await db.Organizations
            .AnyAsync(o => o.Id == requestedOrganizationId &&
                           !o.IsDeleted &&
                           o.Status == ERPKeys.Domain.Modules.Organization.OrganizationStatus.Active,
                context.RequestAborted);
        if (!organizationIsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "The selected organization is inactive or no longer available."
            });
            return;
        }
    }

    await next();
});

app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestampUtc = DateTimeOffset.UtcNow
})).AllowAnonymous();
app.MapControllers();

// Hangfire dashboard (restrict in production as needed)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    IsReadOnlyFunc = _ => false,  // set true to make it read-only in prod
    Authorization  = []           // no extra auth on top of the app's auth for now
});

app.Run();
