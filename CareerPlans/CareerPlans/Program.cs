using CareerPlans.DbContexts;
using CareerPlans.Services.Implementations;
using CareerPlans.Services.Interfaces;
using CareerPlans.Utils;
using IdempotentAPI.Cache.FusionCache.Extensions.DependencyInjection;
using IdempotentAPI.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddFastEndpoints();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CareerPlanDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("CareerPlan"),
        x => x.MigrationsAssembly(typeof(CareerPlanDbContext).Assembly.FullName)));
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ICareerService, CareerService>();
builder.Services.AddScoped<ICareerPlanService, CareerPlanService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemsExceptionHandler>();
builder.Services.AddOptions<RabbitMqTransportOptions>()
    .Configure(options =>
    {
        options.Host = builder.Configuration["RabbitMq:Host"];
        options.User = builder.Configuration["RabbitMq:User"];
        options.Pass = builder.Configuration["RabbitMq:Password"];
    });;
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq();
});

builder.Services.AddIdempotentMinimalAPI(new IdempotentAPI.Core.IdempotencyOptions
{
    HeaderKeyName = "x-idempotency-key",
    ExpiresInMilliseconds = TimeSpan.FromHours(1).TotalMilliseconds,
    CacheOnlySuccessResponses = true,
    IsIdempotencyOptional = true
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis-Cache");
});

// Configure FusionCache with System.Text.Json, NodaTime serializer and more.
builder.Services.AddFusionCacheNewtonsoftJsonSerializer();

builder.Services.AddIdempotentAPIUsingFusionCache();
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("CareerPlanService"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRedisInstrumentation()
            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

        tracing.AddOtlpExporter();
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<CareerPlanDbContext>();
    if(context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseFastEndpoints();

app.Run();