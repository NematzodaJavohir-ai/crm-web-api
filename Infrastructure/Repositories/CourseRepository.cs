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

    public async Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => await context.Courses.AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct);

    public async Task<bool> HasActiveGroupsAsync(int id, CancellationToken ct = default)
        => await context.Groups.AnyAsync(g => g.CourseId == id && g.Status == Domain.Enums.GroupStatus.Active, ct);

    public async Task<int> GetGroupCountAsync(int id, CancellationToken ct = default)
        => await context.Groups.CountAsync(g => g.CourseId == id, ct);

    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Course?> GetWithGroupsAsync(int id, CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .Include(c => c.Groups.Where(g => g.Status == Domain.Enums.GroupStatus.Active))
                .ThenInclude(g => g.Mentor)
                    .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<Course>> GetAllActiveAsync(CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<Course>> GetByPriceRangeAsync(decimal min, decimal max, CancellationToken ct = default)
        => await context.Courses
            .AsNoTracking()
            .Where(c => c.Price >= min && c.Price <= max)
            .OrderBy(c => c.Price)
            .ToListAsync(ct);
}