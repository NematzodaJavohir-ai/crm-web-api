using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonScoreRepository(DataContext context) : ILessonScoreRepository
{
    public async Task AddAsync(LessonScore lessonScore, CancellationToken ct = default)
        => await context.LessonScores.AddAsync(lessonScore, ct);

    public void Update(LessonScore lessonScore)
        => context.LessonScores.Update(lessonScore);

    public void Delete(LessonScore lessonScore)
        => context.LessonScores.Remove(lessonScore);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.LessonScores.AnyAsync(ls => ls.Id == id, ct);

    public async Task<LessonScore?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.LessonScores
            .AsNoTracking()
            .Include(ls => ls.Homework)
                .ThenInclude(h => h.Lesson)
            .Include(ls => ls.Student)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(ls => ls.Id == id, ct);

    public async Task<LessonScore?> GetByHomeworkAndStudentAsync(int homeworkId, int studentId, CancellationToken ct = default)
        => await context.LessonScores
            .AsNoTracking()
            .Include(ls => ls.Homework)
            .Include(ls => ls.Student)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(ls => ls.HomeworkId == homeworkId && ls.StudentId == studentId, ct);

    public async Task<IEnumerable<LessonScore>> GetByHomeworkIdAsync(int homeworkId, CancellationToken ct = default)
        => await context.LessonScores
            .AsNoTracking()
            .Where(ls => ls.HomeworkId == homeworkId)
            .Include(ls => ls.Student)
                .ThenInclude(s => s.User)
            .OrderByDescending(ls => ls.Score)
            .ToListAsync(ct);

    public async Task<IEnumerable<LessonScore>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.LessonScores
            .AsNoTracking()
            .Where(ls => ls.StudentId == studentId)
            .Include(ls => ls.Homework)
                .ThenInclude(h => h.Lesson)
            .OrderByDescending(ls => ls.SubmittedAt)
            .ToListAsync(ct);

    public async Task<double> GetAverageScoreByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.LessonScores
            .Where(ls => ls.StudentId == studentId && ls.Homework.Lesson.GroupId == groupId)
            .AverageAsync(ls => (double?)ls.Score, ct) ?? 0;

    public async Task<int> GetTotalScoreByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default)
        => await context.LessonScores
            .Where(ls => ls.StudentId == studentId && ls.Homework.Lesson.GroupId == groupId)
            .SumAsync(ls => ls.Score, ct);
}