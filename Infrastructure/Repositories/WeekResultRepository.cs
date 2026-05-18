using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class WeekResultRepository(DataContext context) : IWeekResultRepository
{
    public async Task AddAsync(WeekResult weekResult, CancellationToken ct = default)
        => await context.WeekResults.AddAsync(weekResult, ct);

    public void Update(WeekResult weekResult)
        => context.WeekResults.Update(weekResult);

    public void Delete(WeekResult weekResult)
        => context.WeekResults.Remove(weekResult);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.WeekResults.AnyAsync(wr => wr.Id == id, ct);

    public async Task<WeekResult?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Include(wr => wr.Student)
            .ThenInclude(s => s.User)
            .Include(wr => wr.Group)
            .ThenInclude(g => g.Course)
            .Include(wr => wr.Group)
            .ThenInclude(g => g.Mentor)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(wr => wr.Id == id, ct);

    public async Task<WeekResult?> GetByStudentGroupAndWeekAsync(int studentId, int groupId, int weekNumber,
        CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Include(wr => wr.Student)
            .ThenInclude(s => s.User)
            .Include(wr => wr.Group)
            .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(wr => wr.StudentId == studentId
                                       && wr.GroupId == groupId
                                       && wr.WeekNumber == weekNumber, ct);

    public async Task<IEnumerable<WeekResult>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Where(wr => wr.StudentId == studentId)
            .Include(wr => wr.Group)
            .ThenInclude(g => g.Course)
            .Include(wr => wr.Group)
            .ThenInclude(g => g.Mentor)
            .ThenInclude(m => m.User)
            .OrderBy(wr => wr.GroupId)
            .ThenBy(wr => wr.WeekNumber)
            .ToListAsync(ct);

    public async Task<IEnumerable<WeekResult>> GetByGroupAndWeekAsync(int groupId, int weekNumber,
        CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Where(wr => wr.GroupId == groupId && wr.WeekNumber == weekNumber)
            .Include(wr => wr.Student)
            .ThenInclude(s => s.User)
            .Include(wr => wr.Group)
            .ThenInclude(g => g.Course)
            .OrderByDescending(wr => wr.TotalScore)
            .ToListAsync(ct);

    public async Task<IEnumerable<WeekResult>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Where(wr => wr.GroupId == groupId)
            .Include(wr => wr.Student)
            .ThenInclude(s => s.User)
            .Include(wr => wr.Group)
            .ThenInclude(g => g.Course)
            .OrderBy(wr => wr.WeekNumber)
            .ThenByDescending(wr => wr.TotalScore)
            .ToListAsync(ct);

    public async Task<int> GetTotalScoreByStudentAndGroupAsync(int studentId, int groupId,
        CancellationToken ct = default)
        => await context.WeekResults
            .Where(wr => wr.StudentId == studentId && wr.GroupId == groupId)
            .SumAsync(wr => wr.TotalScore, ct);

    public async Task<double> GetAverageTotalScoreByGroupAsync(int groupId, CancellationToken ct = default)
        => await context.WeekResults
            .Where(wr => wr.GroupId == groupId)
            .AverageAsync(wr => (double?)wr.TotalScore, ct) ?? 0;

    public async Task RecalculateTotalAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default)
    {
        var weekResult = await context.WeekResults
            .FirstOrDefaultAsync(wr => wr.StudentId == studentId
                                       && wr.GroupId == groupId
                                       && wr.WeekNumber == weekNumber, ct);

        if (weekResult is null) return;

        var presentCount = await context.Attendances
            .CountAsync(a => a.StudentId == studentId
                             && a.Lesson.GroupId == groupId
                             && a.Lesson.WeekNumber == weekNumber
                             && a.IsPresent, ct);

        weekResult.AttendanceScore = presentCount;
        weekResult.TotalScore = presentCount + weekResult.BonusScore + weekResult.ExamScore;
        weekResult.UpdatedAt = DateTime.UtcNow;

        context.WeekResults.Update(weekResult);
    }
}