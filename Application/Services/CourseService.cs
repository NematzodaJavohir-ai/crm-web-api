using Application.Dtos.CourseDto;
using Application.Dtos.GroupDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CourseService(IUnitOfWork uow) : ICourseService
{
    public async Task<Result<CourseResponseDto>> CreateAsync(CourseCreateDto dto, CancellationToken ct = default)
    {
        var nameExists = await uow.Courses.NameExistsAsync(dto.Name, ct);
        if (nameExists)
            return Result<CourseResponseDto>.Fail("Course with this name already exists", ErrorType.Conflict);

        var course = new Course
        {
            Name = dto.Name,
            Description = dto.Description,
            DurationWeeks = dto.DurationWeeks,
            IsActive = true,
            IconUrl = dto.IconUrl,
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

        if (dto.Name is not null && dto.Name != course.Name)
        {
            var nameExists = await uow.Courses.NameExistsAsync(dto.Name, ct);
            if (nameExists)
                return Result<CourseResponseDto>.Fail("Course with this name already exists", ErrorType.Conflict);

            course.Name = dto.Name;
        }

        if (dto.Description is not null) course.Description = dto.Description;
        if (dto.DurationWeeks.HasValue) course.DurationWeeks = dto.DurationWeeks.Value;
        if (dto.IsActive.HasValue) course.IsActive = dto.IsActive.Value;

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

    public async Task<Result<CourseResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetByIdAsync(id, ct);
        if (course is null)
            return Result<CourseResponseDto>.Fail("Course not found", ErrorType.NotFound);

        if (course.IsActive == isActive)
            return Result<CourseResponseDto>.Fail($"Course is already {(isActive ? "active" : "inactive")}", ErrorType.Validation);

        course.IsActive = isActive;

        uow.Courses.Update(course);
        await uow.SaveChangesAsync(ct);

        return Result<CourseResponseDto>.Ok(MapToResponseDto(course));
    }

    public async Task<Result<IEnumerable<CourseResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var courses = await uow.Courses.GetAllAsync(ct);
        var result = courses.Select(MapToResponseDto);

        return Result<IEnumerable<CourseResponseDto>>.Ok(result);
    }

    public async Task<Result<IEnumerable<CourseResponseDto>>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var courses = await uow.Courses.GetAllActiveAsync(ct);
        var result = courses.Select(MapToResponseDto);

        return Result<IEnumerable<CourseResponseDto>>.Ok(result);
    }

    public async Task<Result<CourseResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetByIdAsync(id, ct);
        if (course is null)
            return Result<CourseResponseDto>.Fail("Course not found", ErrorType.NotFound);

        return Result<CourseResponseDto>.Ok(MapToResponseDto(course));
    }

    public async Task<Result<CourseWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default)
    {
        var course = await uow.Courses.GetWithGroupsAsync(id, ct);
        if (course is null)
            return Result<CourseWithGroupsDto>.Fail("Course not found", ErrorType.NotFound);

        var dto = new CourseWithGroupsDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            DurationWeeks = course.DurationWeeks,
            IsActive = course.IsActive,
            CreatedAt = course.CreatedAt,
            Groups = course.Groups.Select(g => new GroupShortDto
            {
                Id = g.Id,
                Name = g.Name,
                CourseName = course.Name,
                MentorName = $"{g.Mentor.User.FirstName} {g.Mentor.User.LastName}",
                Status = g.Status
            })
        };

        return Result<CourseWithGroupsDto>.Ok(dto);
    }

    private static CourseResponseDto MapToResponseDto(Course course) => new()
    {
        Id = course.Id,
        Name = course.Name,
        Description = course.Description,
         IconUrl = course.IconUrl, 
        DurationWeeks = course.DurationWeeks,
        IsActive = course.IsActive,
        CreatedAt = course.CreatedAt
        
    };
}