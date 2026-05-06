using Application.Interfaces.Services;
using Application.Email;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Jobs;

public class WeeklyReportJob(
    DataContext context,
    IEmailService emailService)
{
    public async Task SendWeeklyReportsAsync()
    {
        // все активные группы
        var groups = await context.Groups
            .Where(g => g.Status == Domain.Enums.GroupStatus.Active)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Student)
                    .ThenInclude(s => s.User)
            .ToListAsync();

        var currentWeek = GetCurrentWeekNumber();

        foreach (var group in groups)
        {
            foreach (var groupStudent in group.GroupStudents)
            {
                var student = groupStudent.Student;
                var user = student.User;

                // берём результат за текущую неделю
                var weekResult = await context.WeekResults
                    .FirstOrDefaultAsync(wr =>
                        wr.StudentId == student.Id &&
                        wr.GroupId == group.Id &&
                        wr.WeekNumber == currentWeek);

                if (weekResult is null) continue;

                // берём посещаемость за неделю
                var attendances = await context.Attendances
                    .Where(a =>
                        a.StudentId == student.Id &&
                        a.Lesson.GroupId == group.Id &&
                        a.Lesson.WeekNumber == currentWeek)
                    .Include(a => a.Lesson)
                    .ToListAsync();

                var body = BuildEmailBody(
                    firstName: user.FirstName,
                    groupName: group.Name,
                    weekNumber: currentWeek,
                    attendances: attendances,
                    weekResult: weekResult
                );

                await emailService.SendEmail(
                    to: user.Email,
                    subject: $"📊 Week {currentWeek} Results — {group.Name}",
                    body: body
                );
            }
        }
    }

    private static int GetCurrentWeekNumber()
    {
        var now = DateTime.UtcNow;
        return System.Globalization.ISOWeek.GetWeekOfYear(now);
    }

    private static string BuildEmailBody(
        string firstName,
        string groupName,
        int weekNumber,
        List<Domain.Entities.Attendance> attendances,
        Domain.Entities.WeekResult weekResult)
    {
        var rows = string.Join("", attendances.Select(a => $"""
            <tr>
                <td>{a.Lesson.LessonDate:dd.MM.yyyy}</td>
                <td>{(a.IsPresent ? "✅" : "❌")}</td>
                <td>{a.Score}</td>
                <td>{(a.HomeworkDone ? "✅" : "❌")}</td>
                <td>{a.HomeworkScore}</td>
            </tr>
        """));

        return $"""
        <html>
        <body style="font-family: Arial, sans-serif; padding: 20px;">
            <h2>Hello, {firstName}! 👋</h2>
            <p>Here are your results for <strong>Week {weekNumber}</strong> in <strong>{groupName}</strong>:</p>

            <table border="1" cellpadding="8" cellspacing="0" style="border-collapse: collapse; width: 100%;">
                <thead style="background-color: #4CAF50; color: white;">
                    <tr>
                        <th>Date</th>
                        <th>Present</th>
                        <th>Score</th>
                        <th>Homework</th>
                        <th>HW Score</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>

            <br/>
            <table border="1" cellpadding="8" cellspacing="0" style="border-collapse: collapse; width: 50%;">
                <tr><td><strong>Attendance Score</strong></td><td>{weekResult.AttendanceScore}</td></tr>
                <tr><td><strong>Bonus Score</strong></td><td>{weekResult.BonusScore}</td></tr>
                <tr><td><strong>Exam Score</strong></td><td>{weekResult.ExamScore}</td></tr>
                <tr style="background-color: #f0f0f0;">
                    <td><strong>Total Score</strong></td>
                    <td><strong>{weekResult.TotalScore}</strong></td>
                </tr>
            </table>

            {(weekResult.MentorComment is not null ? $"<p>💬 <strong>Mentor comment:</strong> {weekResult.MentorComment}</p>" : "")}

            <br/>
            <p style="color: gray;">Keep it up! 💪</p>
        </body>
        </html>
        """;
    }
}