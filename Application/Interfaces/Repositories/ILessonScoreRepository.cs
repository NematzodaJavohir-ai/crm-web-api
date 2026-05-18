using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILessonScoreRepository
{
    Task AddAsync(LessonScore lessonScore, CancellationToken ct = default);
    void Update(LessonScore lessonScore);
    void Delete(LessonScore lessonScore);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    

    Task<LessonScore?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LessonScore?> GetByHomeworkAndStudentAsync(int homeworkId, int studentId, CancellationToken ct = default);
    Task<IEnumerable<LessonScore>> GetByHomeworkIdAsync(int homeworkId, CancellationToken ct = default);
    Task<IEnumerable<LessonScore>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);
    Task<double> GetAverageScoreByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default);
    Task<int> GetTotalScoreByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default);
}