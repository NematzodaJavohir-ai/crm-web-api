using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.GroupDto;

public class GroupUpdateDto
{
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    public string? Name { get; set; }

    public int? MentorId { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Range(1, 50, ErrorMessage = "Max students must be between 1 and 50")]
    public int? MaxStudents { get; set; }

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [EnumDataType(typeof(GroupStatus))]
    public GroupStatus? Status { get; set; }
}