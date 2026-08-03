namespace Courses.Features.Cycle.Open;

public class CycleOpenRequestBinder : IRequestBinder<CycleOpenRequest>
{
    public ValueTask<CycleOpenRequest> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        return new ValueTask<CycleOpenRequest>();
    }
}