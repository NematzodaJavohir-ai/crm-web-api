using Application.Interfaces.Services;
using Application.Email;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Jobs;

public class WeeklyReportJob
{
    private readonly DataContext _context;
    private readonly IEmailService _emailService;

    public WeeklyReportJob(DataContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task SendWeeklyReportsAsync()
    {
        var currentWeek = GetCurrentWeekNumber();

        // 1. Загружаем группы и активных студентов
        var groups = await _context.Groups
            .Where(g => g.Status == GroupStatus.Active)
            .Include(g => g.Course)
            .Include(g => g.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Student)
                    .ThenInclude(s => s.User)
            .ToListAsync();

        // 2. Оптимизация: Загружаем все результаты и посещаемость одним запросом
        var allWeekResults = await _context.WeekResults
            .Where(wr => wr.WeekNumber == currentWeek)
            .ToListAsync();

        var allAttendances = await _context.Attendances
            .Where(a => a.Lesson.WeekNumber == currentWeek)
            .Include(a => a.Lesson)
            .ToListAsync();

        foreach (var group in groups)
        {
            foreach (var groupStudent in group.GroupStudents)
            {
                try
                {
                    var student = groupStudent.Student;
                    var user = student.User;

                    if (string.IsNullOrEmpty(user.Email)) continue;

                    // Поиск данных в локальных списках (без запросов к БД в цикле)
                    var weekResult = allWeekResults.FirstOrDefault(wr =>
                        wr.StudentId == student.Id &&
                        wr.GroupId == group.Id);

                    if (weekResult is null) continue;

                    var studentAttendances = allAttendances
                        .Where(a => a.StudentId == student.Id && a.Lesson.GroupId == group.Id)
                        .OrderBy(a => a.Lesson.LessonDate)
                        .ToList();

                    var body = BuildEmailBody(
                        firstName: user.FirstName,
                        groupName: group.Name,
                        weekNumber: currentWeek,
                        attendances: studentAttendances,
                        weekResult: weekResult
                    );

                    await _emailService.SendEmail(
                        to: user.Email,
                        subject: $"📊 Week {currentWeek} Results — {group.Name}",
                        body: body
                    );
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, чтобы один сбой не ломал всю рассылку
                    // _logger.LogError(ex, "Failed to send email to {Email}", groupStudent.Student.User.Email);
                    continue;
                }
            }
        }
    }

    private static int GetCurrentWeekNumber()
    {
        return ISOWeek.GetWeekOfYear(DateTime.UtcNow);
    }

    private static string BuildEmailBody(
        string firstName,
        string groupName,
        int weekNumber,
        List<Attendance> attendances,
        WeekResult weekResult)
    {
        var rows = string.Join("", attendances.Select(a => $"""
            <tr>
                <td style="padding: 10px;">{a.Lesson.LessonDate:dd.MM.yyyy}</td>
                <td style="text-align: center; padding: 10px;">{(a.IsPresent ? "✅" : "❌")}</td>
                <td style="text-align: center; padding: 10px;">{(a.MentorNote ?? "-")}</td>
            </tr>
        """));

        var commentHtml = !string.IsNullOrWhiteSpace(weekResult.MentorComment)
            ? $"""
            <div class="comment-box">
                <strong>💬 Mentor comment:</strong><br/>{weekResult.MentorComment}
            </div>
            """
            : "";

        // Используем двойные скобки {{ }} для CSS, чтобы C# не путал их с интерполяцией
        return $"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8" />
            
        </head>
        <body>
            <div class="container">
                <h2>👋 Hello, {firstName}!</h2>
                <p class="subtitle">Here are your results for <strong>Week {weekNumber}</strong> in <strong>{groupName}</strong></p>

                <h3>📅 Attendance</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th style="text-align: center;">Present</th>
                            <th style="text-align: center;">Note</th>
                        </tr>
                    </thead>
                    <tbody>
                        {rows}
                    </tbody>
                </table>

                <h3>📊 Scores</h3>
                <table class="summary-table">
                    <tr><td>Attendance Score</td><td>{weekResult.AttendanceScore}</td></tr>
                    <tr><td>Bonus Score</td><td>{weekResult.BonusScore}</td></tr>
                    <tr><td>Exam Score</td><td>{weekResult.ExamScore}</td></tr>
                    <tr class="total-row"><td>Total Score</td><td>{weekResult.TotalScore}</td></tr>
                </table>

                {commentHtml}

                <div class="footer">
                    Keep it up! 💪<br/>
                    <span style="font-size: 12px;">This report was generated automatically</span>
                </div>
            </div>
        </body>
        </html>
        """;
    }
}