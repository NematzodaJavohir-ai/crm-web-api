using System;
using BlazorApp.DTOs.AttendanceDto;


namespace BlazorApp.DTOs.LessonDto;

public class LessonWithAttendancesDto : LessonResponseDto
{
    public IEnumerable<AttendanceResponseDto> Attendances { get; set; } = new List<AttendanceResponseDto>();
}
