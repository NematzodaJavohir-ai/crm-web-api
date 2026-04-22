using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IMentorRepository
{
    Task<IEnumerable<Mentor>> GetAllMentorsAsync(CancellationToken ct = default);
    Task<Mentor?> GetMentorByIdAsync(int id, CancellationToken ct = default);
    Task<int>AddMentorAsync(Mentor mentor, CancellationToken ct = default);
    Task<bool> UpdateMentor(Mentor mentor);
    Task<bool>DeleteMentor(Mentor mentor);
}
