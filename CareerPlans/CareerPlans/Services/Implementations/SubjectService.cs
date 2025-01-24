using CareerPlans.DbContexts;
using CareerPlans.Entities;
using CareerPlans.Features.Subjects.Create;
using CareerPlans.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareerPlans.Services.Implementations;

public class SubjectService(CareerPlanDbContext context) : ISubjectService
{
    public async Task<Guid> AddSubjectAsync(CreateSubjectRequest req)
    {
        var subject = new Subject
        {
            Name = req.Name,
            Value = req.Value
        };
        context.Subjects.Add(subject);
        await context.SaveChangesAsync();
        return subject.Id;
    }

    public async Task<Subject?> GetById(Guid id)
    {
        return await context.Subjects.FirstOrDefaultAsync(x=>x.Id==id);
    }
}