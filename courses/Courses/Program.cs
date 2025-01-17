using Courses.DbContexts;
using Courses.Services.Implementations;
using Courses.Services.Interfaces;
using Courses.Utils;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Extensions.DependencyInjection;
using IdempotentAPI.MinimalAPI.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.MultiBus;
using Microsoft.EntityFrameworkCore;
using IdempotencyOptions = IdempotentAPI.Core.IdempotencyOptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFastEndpoints();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddIdempotentMinimalAPI(new IdempotencyOptions
{
    HeaderKeyName = "x-idempotency-key",
    ExpiresInMilliseconds = TimeSpan.FromHours(1).TotalMilliseconds,
    CacheOnlySuccessResponses = true,
    IsIdempotencyOptional = false
});
// Register an implementation of the IDistributedCache.
// For this example, we are using a Memory Cache.
builder.Services.AddDistributedMemoryCache();

// Register the IdempotentAPI.Cache.DistributedCache.
builder.Services.AddIdempotentAPIUsingDistributedCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<CoursesDbContext>();
    if(context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseFastEndpoints();

app.Run();