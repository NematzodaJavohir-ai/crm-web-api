using System;
using Application.Dtos.AttendanceDto;

namespace Application.Dtos.LessonDto;

public class LessonWithAttendancesDto : LessonResponseDto
{
    public IEnumerable<AttendanceResponseDto> Attendances { get; set; } = new List<AttendanceResponseDto>();
}
