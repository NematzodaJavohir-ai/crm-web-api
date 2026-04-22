using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IVerificationCodeRepository
{
    Task<int> AddVerifyCode(VerificationCode code,CancellationToken cancellationToken);
    Task<VerificationCode?>GetLatestVerifyCode(int userid,CancellationToken cancellationToken);

    Task<bool> DeleteCode(VerificationCode code);
    Task<bool> DeleteAllCodesByUserId(int userid,CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
