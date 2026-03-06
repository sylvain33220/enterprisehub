/*
@file Program.cs
@description Entry point of the EnterpriseHub API application.
@author Poteaux sylvain
@site https://www.studio-purple.com
@mail poteaux.sylvain@gmail.com
@date © 2026 EnterpriseHub
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
using EnterpriseHub.Infrastructure.Extensions;
// Auth 
using EnterpriseHub.Application.Auth;
using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Infrastructure.Auth;
// Clients
using EnterpriseHub.Application.Clients;
using EnterpriseHub.Application.Clients.Ports;
// Projects
using EnterpriseHub.Application.Projects;
using EnterpriseHub.Application.Projects.Ports;
// Tickets
using EnterpriseHub.Application.Tickets;
using EnterpriseHub.Application.Tickets.Ports;
// Repositories
using EnterpriseHub.Infrastructure.Persistence.Repositories;
// JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
// Middleware
using EnterpriseHub.Api.Middlewares;
// Hashing
using Microsoft.IdentityModel.Tokens;
using System.Text;
// Dashboard
using EnterpriseHub.Application.Dashboard.Ports;
using EnterpriseHub.Infrastructure.Querying;
// DB
using EnterpriseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EnterpriseHub.Application;
using EnterpriseHub.Application.Common.Interfaces;
using EnterpriseHub.Api.Auth;
// Serilog
using Serilog;
// API versioning
using Asp.Versioning;
// Rate limiting
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using EnterpriseHub.Application.Common.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";

        var problem = """
        {
          "title": "Too Many Requests",
          "status": 429,
          "detail": "Rate limit exceeded."
        }
        """;

        await context.HttpContext.Response.WriteAsync(problem, token);
    };
});
builder.Services.AddControllers();
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EnterpriseHub API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
// ✅ DB (centralisée)
builder.Services.AddDatabase(builder.Configuration);
// Application services
builder.Services.AddScoped<AuthService>();
// Ports -> Infrastructure impl
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<IDashboardReadRepository, DashboardReadRepository>();
builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IEnterpriseHubDbContext>(sp =>
    sp.GetRequiredService<EnterpriseHubDbContext>());
builder.Services.Configure<CookieConfig>(
    builder.Configuration.GetSection("AuthCookies"));

builder.Services.AddApplication();  

// JWT auth
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtIssuer)) throw new InvalidOperationException("Jwt:Issuer missing");
if (string.IsNullOrWhiteSpace(jwtAudience)) throw new InvalidOperationException("Jwt:Audience missing");
if (string.IsNullOrWhiteSpace(jwtKey)) throw new InvalidOperationException("Jwt:Key missing");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("DevOnly", policy => policy.RequireRole("Dev"));
    options.AddPolicy("CanManageUsers", policy =>
    policy.RequireClaim("perm", "users:write"));
});

var app = builder.Build();
if (builder.Configuration.GetValue<bool>("Database:AutoMigrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EnterpriseHubDbContext>();
    db.Database.Migrate();
}
// Swagger + auto-migrate uniquement en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EnterpriseHubDbContext>();
    db.Database.Migrate();
}
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/", () => Results.Ok("✅ EnterpriseHub API is running ✅"));
    // app.MapGet("/__throw/conflict", async context => throw new EnterpriseHub.Application.Common.Exceptions.ConflictAppException("boom"));
    // app.MapGet("/__throw/forbidden", async context => throw new EnterpriseHub.Application.Common.Exceptions.ForbiddenAppException("nope"));
    app.MapGet("/__throw/forbidden", context => throw new ForbiddenAppException("nope"));
    app.MapGet("/__throw/conflict", context => throw new ConflictAppException("boom"));
}
// Pipeline commun (1 seule fois)
app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.UseRateLimiter();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


// app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));
app.MapGet("/ready", () => Results.Ok(new { status = "ready" }));
app.Run();
public partial class Program { }
