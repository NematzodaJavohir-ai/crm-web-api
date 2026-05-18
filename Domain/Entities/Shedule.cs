namespace Domain.Entities;

public class Shedule
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public Group Group { get; set; } = null!;
}