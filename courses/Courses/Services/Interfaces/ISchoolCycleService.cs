using Courses.Domain.Entitites;
using Courses.Features.Cycle.Create;
using Courses.Utils;

namespace Courses.Services.Interfaces;

public interface ISchoolCycleService
{
    Task<Result<Cycle>> add(CycleCreateRequest cycle);
    Task<Result<Cycle>> open();
}