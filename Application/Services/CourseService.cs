using Application.Dtos.CourseDto;
using Application.Dtos.GroupDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CourseService(IUnitOfWork uow) : ICourseService
{
    // ───── Basic CRUD ─────

    public async Task<Result<CourseResponseDto>> CreateAsync(CourseCreateDto dto, CancellationToken ct = default)
    {
        var nameExists = await uow.Courses.NameExistsAsync(dto.Name, ct);
        if (nameExists)
            return Result<CourseResponseDto>.Fail("Course with this name already exists", ErrorType.Conflict);

        var course = new Course
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IconUrl = dto.IconUrl,
            DurationWeeks = dto.DurationWeeks,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await uow.Courses.AddAsync(course, ct);
        await uow.SaveChangesAsync(ct);

        return Result<CourseResponseDto>.Ok(MapToResponseDto(course));
    }

    public async Task<Result<CourseResponseDto>> UpdateAsync(int id, CourseUpdateDto dto, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetByIdAsync(id, ct);
        if (course is null)
            return Result<CourseResponseDto>.Fail("Course not found", ErrorType.NotFound);

        if (dto.Name != course.Name)
        {
            var nameExists = await uow.Courses.NameExistsAsync(dto.Name, ct);
            if (nameExists)
                return Result<CourseResponseDto>.Fail("Course with this name already exists", ErrorType.Conflict);
            course.Name = dto.Name;
        }

        course.Description = dto.Description;
        course.Price = dto.Price;
        course.IconUrl = dto.IconUrl;
        course.DurationWeeks = dto.DurationWeeks;
        course.IsActive = dto.IsActive;

        uow.Courses.Update(course);
        await uow.SaveChangesAsync(ct);

        return Result<CourseResponseDto>.Ok(MapToResponseDto(course));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetWithGroupsAsync(id, ct);
        if (course is null)
            return Result<bool>.Fail("Course not found", ErrorType.NotFound);

        if (course.Groups.Any(g => g.Status == GroupStatus.Active))
            return Result<bool>.Fail("Cannot delete course with active groups", ErrorType.Validation);

        uow.Courses.Delete(course);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<CourseResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetByIdAsync(id, ct);
        if (course is null)
            return Result<CourseResponseDto>.Fail("Course not found", ErrorType.NotFound);

        return Result<CourseResponseDto>.Ok(MapToResponseDto(course));
    }

    // ───── Queries ─────

    public async Task<Result<IEnumerable<CourseResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var courses = await uow.Courses.GetAllAsync(ct);
        return Result<IEnumerable<CourseResponseDto>>.Ok(courses.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<CourseResponseDto>>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var courses = await uow.Courses.GetAllActiveAsync(ct);
        return Result<IEnumerable<CourseResponseDto>>.Ok(courses.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<CourseResponseDto>>> GetByPriceRangeAsync(decimal min, decimal max, CancellationToken ct = default)
    {
        var courses = await uow.Courses.GetByPriceRangeAsync(min, max, ct);
        return Result<IEnumerable<CourseResponseDto>>.Ok(courses.Select(MapToResponseDto));
    }

    // ───── Detailed ─────

    public async Task<Result<CourseWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetWithGroupsAsync(id, ct);
        if (course is null)
            return Result<CourseWithGroupsDto>.Fail("Course not found", ErrorType.NotFound);

        return Result<CourseWithGroupsDto>.Ok(MapToWithGroupsDto(course));
    }

    public async Task<Result<CourseWithGroupsDto>> GetWithActiveGroupsAsync(int id, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetWithGroupsAsync(id, ct);
        if (course is null)
            return Result<CourseWithGroupsDto>.Fail("Course not found", ErrorType.NotFound);

        return Result<CourseWithGroupsDto>.Ok(MapToWithGroupsDto(course, onlyActive: true));
    }

    // ───── Admin Actions ─────

    public async Task<Result<CourseResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetByIdAsync(id, ct);
        if (course is null)
            return Result<CourseResponseDto>.Fail("Course not found", ErrorType.NotFound);

        course.IsActive = isActive;
        uow.Courses.Update(course);
        await uow.SaveChangesAsync(ct);

        return Result<CourseResponseDto>.Ok(MapToResponseDto(course));
    }

    public async Task<Result<bool>> NameExistsAsync(string name, CancellationToken ct = default)
    {
        var exists = await uow.Courses.NameExistsAsync(name, ct);
        return Result<bool>.Ok(exists);
    }

    public async Task<Result<int>> GetGroupCountAsync(int id, CancellationToken ct = default)
    {
        var count = await uow.Courses.GetGroupCountAsync(id, ct);
        return Result<int>.Ok(count);
    }

    // ───── Lookup ─────

    public async Task<Result<IEnumerable<CourseShortDto>>> GetLookupAsync(CancellationToken ct = default)
    {
        var courses = await uow.Courses.GetAllAsync(ct);
        return Result<IEnumerable<CourseShortDto>>.Ok(courses.Select(MapToShortDto));
    }

    public async Task<Result<IEnumerable<CourseShortDto>>> GetActiveLookupAsync(CancellationToken ct = default)
    {
        var courses = await uow.Courses.GetAllActiveAsync(ct);
        return Result<IEnumerable<CourseShortDto>>.Ok(courses.Select(MapToShortDto));
    }

    // ───── Mappers ─────

    private static CourseResponseDto MapToResponseDto(Course c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        Price = c.Price,
        IconUrl = c.IconUrl,
        DurationWeeks = c.DurationWeeks,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    };

    private static CourseShortDto MapToShortDto(Course c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        DurationWeeks = c.DurationWeeks
    };

    private static CourseWithGroupsDto MapToWithGroupsDto(Course c, bool onlyActive = false)
    {
        var groups = onlyActive
            ? c.Groups.Where(g => g.Status == GroupStatus.Active)
            : c.Groups;

        return new CourseWithGroupsDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Price = c.Price,
            IconUrl = c.IconUrl,
            DurationWeeks = c.DurationWeeks,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            Groups = groups.Select(g => new GroupShortDto
            {
                Id = g.Id,
                Name = g.Name,
                CourseName = c.Name,
                MentorName = g.Mentor?.User != null
                    ? $"{g.Mentor.User.FirstName} {g.Mentor.User.LastName}"
                    : "",
                Status = g.Status.ToString(),
                StudentCount = g.GroupStudents.Count(gs => gs.IsActive),
                StartDate = g.StartDate
            })
        };
    }
}