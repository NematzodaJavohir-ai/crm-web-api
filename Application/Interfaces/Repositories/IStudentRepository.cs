using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct = default);
    Task<Student?> GetStudentByIdAsync(int id, CancellationToken ct = default);
    Task<int> AddStudentAsync(Student student, CancellationToken ct = default);
    Task<bool> UpdateStudent(Student student);
    Task<bool> DeleteStudent(Student student);
    
}
