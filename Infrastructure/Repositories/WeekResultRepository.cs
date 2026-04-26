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
            .FirstOrDefaultAsync(wr => wr.Id == id, ct);

    public async Task<WeekResult?> GetByStudentGroupWeekAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .FirstOrDefaultAsync(wr => wr.StudentId == studentId
                                    && wr.GroupId == groupId
                                    && wr.WeekNumber == weekNumber, ct);

    public async Task<IEnumerable<WeekResult>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Where(wr => wr.StudentId == studentId)
            .OrderBy(wr => wr.WeekNumber)
            .ToListAsync(ct);

    public async Task<IEnumerable<WeekResult>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Where(wr => wr.GroupId == groupId)
            .Include(wr => wr.Student)
                .ThenInclude(s => s.User)
            .OrderBy(wr => wr.WeekNumber)
            .ToListAsync(ct);

    public async Task<IEnumerable<WeekResult>> GetByGroupAndWeekAsync(int groupId, int weekNumber, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Where(wr => wr.GroupId == groupId && wr.WeekNumber == weekNumber)
            .Include(wr => wr.Student)
                .ThenInclude(s => s.User)
            .OrderByDescending(wr => wr.TotalScore)
            .ToListAsync(ct);

    public async Task<WeekResult?> GetBestWeekAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.WeekResults
            .AsNoTracking()
            .Where(wr => wr.StudentId == studentId && wr.GroupId == groupId)
            .OrderByDescending(wr => wr.TotalScore)
            .FirstOrDefaultAsync(ct);

    public async Task<double> GetAverageScoreAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.WeekResults
            .Where(wr => wr.StudentId == studentId && wr.GroupId == groupId)
            .AverageAsync(wr => (double?)wr.TotalScore, ct) ?? 0;

    public async Task RecalculateTotalAsync(int studentId, int groupId, int weekNumber, CancellationToken ct = default)
    {
        var weekResult = await context.WeekResults
            .FirstOrDefaultAsync(wr => wr.StudentId == studentId
                                    && wr.GroupId == groupId
                                    && wr.WeekNumber == weekNumber, ct);

        if (weekResult is null) return;

        var attendanceScore = await context.Attendances
            .Where(a => a.StudentId == studentId
                     && a.Lesson.GroupId == groupId
                     && a.Lesson.WeekNumber == weekNumber)
            .SumAsync(a => a.Score, ct);

        weekResult.AttendanceScore = attendanceScore;
        weekResult.TotalScore = attendanceScore + weekResult.BonusScore + weekResult.ExamScore;
        weekResult.UpdatedAt = DateTime.UtcNow;

        context.WeekResults.Update(weekResult);
    }
}
