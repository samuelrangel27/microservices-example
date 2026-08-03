using System.Text.Json;
using System.Text.Json.Serialization;
using Courses.DbContexts;
using Courses.Services.Implementations;
using Courses.Services.Interfaces;
using Courses.Utils;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Cache.FusionCache.Extensions.DependencyInjection;
using IdempotentAPI.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.MultiBus;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using IdempotencyOptions = IdempotentAPI.Core.IdempotencyOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CoursesDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Courses"),
        x => x.MigrationsAssembly(typeof(CoursesDbContext).Assembly.FullName)));

builder.Services.AddScoped<ISchoolCycleService, SchoolCycleService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemsExceptionHandler>();

builder.Services.AddIdempotentMinimalAPI(new IdempotencyOptions
{
    HeaderKeyName = "x-idempotency-key",
    ExpiresInMilliseconds = TimeSpan.FromHours(1).TotalMilliseconds,
    CacheOnlySuccessResponses = true,
    IsIdempotencyOptional = false
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis-Cache");
});

builder.Services.AddFusionCacheNewtonsoftJsonSerializer();
builder.Services.AddIdempotentAPIUsingFusionCache();

// Register OpenTelemetry only when NOT in Development environment
if (!builder.Environment.IsDevelopment())
{
    var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://tempo:4317";

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing =>
        {
            tracing
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Courses.Api"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                });
        })
        .WithMetrics(metrics =>
        {
            metrics
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Courses.Api"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter();
        });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.MapPrometheusScrapingEndpoint();
}

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<CoursesDbContext>();
    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseFastEndpoints();

app.Run();