using Courses.DbContexts;
using Courses.Services.Implementations;
using Courses.Services.Interfaces;
using Courses.Utils;
using Microsoft.EntityFrameworkCore;

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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseFastEndpoints();

app.Run();