using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository(DataContext context) : ICourseRepository
{
    public async Task AddAsync(Course course, CancellationToken ct = default)
        => await context.Courses.AddAsync(course, ct);

    public void Update(Course course)
        => context.Courses.Update(course);

    public void Delete(Course course)
        => context.Courses.Remove(course);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Courses.AnyAsync(c => c.Id == id, ct);

    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Course?> GetWithGroupsAsync(int id, CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .Include(c => c.Groups)
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IEnumerable<Course>> GetAllActiveAsync(CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync(ct);

    public async Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => await context.Courses.AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct);
}