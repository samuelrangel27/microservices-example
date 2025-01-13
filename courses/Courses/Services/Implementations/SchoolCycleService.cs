using Courses.DbContexts;
using Courses.Domain.Entitites;
using Courses.Features.Cycle.Create;
using Courses.Services.Interfaces;
using Courses.Utils;

namespace Courses.Services.Implementations;

public class SchoolCycleService : ISchoolCycleService
    {
        private readonly CoursesDbContext context;

        public SchoolCycleService(CoursesDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<Cycle>> add(CycleCreateRequest cycle)
        {
            var existingCycle = context.Cycles
                .FirstOrDefault(x => x.Status == CycleStatus.New);
            if(existingCycle != null)
                return Result<Cycle>.Fail("There is already a cycle with status New");

            if(cycle.EndDate < cycle.StartDate)
                return Result<Cycle>.Fail("End date must be greater than start date");

            if(cycle.EndDate.Month - cycle.StartDate.Month < 5)
                return Result<Cycle>.Fail("The duration of the cycle must be at least 5 months");

            var sameYearCycles = context.Cycles
                .Where(x => x.StartDate.Year == cycle.StartDate.Year)
                .ToArray();
            
            if(sameYearCycles.Length == 2)
                return Result<Cycle>.Fail("There can only be 2 cycles every year");

            if(sameYearCycles.FirstOrDefault()?.EndDate < cycle.StartDate)
                return Result<Cycle>.Fail("Start date must be greater than previous cycle's end date");

            var newCycle = new Cycle()
            {
                StartDate = cycle.StartDate,
                EndDate = cycle.EndDate,
                Status = CycleStatus.New
            };
            await context.Cycles.AddAsync(newCycle);
            await context.SaveChangesAsync();
            return Result<Cycle>.Ok(MsgConstants.SUCCESS,newCycle);
        }

        public async Task<Result<Cycle>> open()
        {
            var openCycle = context.Cycles.FirstOrDefault(x => x.Status == CycleStatus.Open);
            if(openCycle != null)
                return Result<Cycle>
                    .Fail($"School cycle with start date {openCycle.StartDate.ToString("dd/MM/yyyy")} is still open");

            var cycle = context.Cycles
                .FirstOrDefault(x => x.Status == CycleStatus.New);
            if(cycle == null)
                return Result<Cycle>.Fail("There is no school cycle with status New");
            
            cycle.Status = CycleStatus.Open;
            context.Update(cycle);
            await context.SaveChangesAsync();
            
            return Result<Cycle>.Ok(MsgConstants.SUCCESS, cycle);
        }
      }