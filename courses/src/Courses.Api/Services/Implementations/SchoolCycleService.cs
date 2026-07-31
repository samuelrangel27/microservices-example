    using Courses.DbContexts;
using Courses.Domain.Entitites;
using Courses.Events;
using Courses.Features.Cycle.Create;
using Courses.Services.Interfaces;
using Courses.Utils;
using MassTransit;

namespace Courses.Services.Implementations;

public class SchoolCycleService(CoursesDbContext context, 
    IPublishEndpoint publishEndpoint,
    ILogger<SchoolCycleService> logger) : ISchoolCycleService
    {

        public async Task<Result<Cycle>> CreateAsync(CycleCreateRequest cycle)
        {
            logger.LogInformation("Querying for existing cycles with New status");
            var existingCycle = context.Cycles
                .FirstOrDefault(x => x.Status == CycleStatus.New);
            if (existingCycle != null)
            {
                logger.LogError("There is already a cycle with status New");
                return Result<Cycle>.Fail("There is already a cycle with status New");
            }

            if(cycle.EndDate < cycle.StartDate)
            {
                logger.LogError("End date must be greater than start date");
                return Result<Cycle>.Fail("End date must be greater than start date");
            }

            if(cycle.EndDate.Month - cycle.StartDate.Month < 5)
                return Result<Cycle>.Fail("The duration of the cycle must be at least 5 months");

            var sameYearCycles = context.Cycles
                .Where(x => x.StartDate.Year == cycle.StartDate.Year)
                .ToArray();
            
            if(sameYearCycles.Length == 2)
            {
                logger.LogError("There can only be 2 cycles every year");
                return Result<Cycle>.Fail("There can only be 2 cycles every year");
            }

            if(sameYearCycles.FirstOrDefault()?.EndDate < cycle.StartDate)
            {
                logger.LogError("Start date must be greater than previous cycle's end date");
                return Result<Cycle>.Fail("Start date must be greater than previous cycle's end date");
            }

            var newCycle = new Cycle()
            {
                StartDate = cycle.StartDate,
                EndDate = cycle.EndDate,
                Status = CycleStatus.New
            };
            await context.Cycles.AddAsync(newCycle);
            await context.SaveChangesAsync();
            await publishEndpoint.Publish(new CycleAdded(newCycle.Id, newCycle.StartDate, newCycle.EndDate));
            return Result<Cycle>.Ok(MsgConstants.SUCCESS, newCycle);
        }

        public async Task<Result<Cycle>> OpenAsync()
        {
            logger.LogInformation("Querying for cycles with Open status");
            var openCycle = context.Cycles.FirstOrDefault(x => x.Status == CycleStatus.Open);
            if(openCycle != null)
            {
                logger.LogError("There is already a cycle with status Open");
                return Result<Cycle>
                    .Fail($"School cycle with start date {openCycle.StartDate.ToString("dd/MM/yyyy")} is still open");
            }
            logger.LogInformation("Querying for cycles with New status");
            var cycle = context.Cycles
                .FirstOrDefault(x => x.Status == CycleStatus.New);
            if(cycle == null)
            {
                logger.LogError("There is no cycle with status New");
                return Result<Cycle>.Fail("There is no school cycle with status New");
            }
            
            cycle.Status = CycleStatus.Open;
            context.Update(cycle);
            await context.SaveChangesAsync();
            
            return Result<Cycle>.Ok(MsgConstants.SUCCESS, cycle);
        }
      }