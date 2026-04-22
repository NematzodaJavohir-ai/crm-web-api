using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllCoursesAsync(CancellationToken ct = default);
    Task<Course?> GetCourseByIdAsync(int id, CancellationToken ct = default);
    Task<int> AddCourseAsync(Course course, CancellationToken ct = default);
    Task<bool> UpdateCourse(Course course);
    Task<bool> DeleteCourse(Course course);
    
}
