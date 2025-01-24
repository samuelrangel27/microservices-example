using CareerPlans.Entities;
using CareerPlans.Features.Subjects.Create;

namespace CareerPlans.Services.Interfaces;

public interface ISubjectService
{
    Task<Guid> AddSubjectAsync(CreateSubjectRequest subject);
    Task<Subject?> GetById(Guid id);
}