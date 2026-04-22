using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VerificationCodeRepository(DataContext context) : IVerificationCodeRepository
{
    public async Task<int>AddVerifyCode(VerificationCode code,CancellationToken cancellationToken)
    {
       await context.VerificationCodes.AddAsync(code,cancellationToken);
       return code.Id;
    }

    public async Task<VerificationCode?> GetLatestVerifyCode(int userid, CancellationToken cancellationToken)
    {
        return await context.VerificationCodes.AsNoTracking()
                                               .Where(vf => vf.UserId == userid)
                                               .OrderByDescending(vf => vf.CreatedAt)
                                               .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<bool> DeleteCode(VerificationCode code)
    {
        context.VerificationCodes.Remove(code);
        return await Task.FromResult(true);
    }

    public async Task<bool>DeleteAllCodesByUserId(int userid, CancellationToken cancellationToken)
    {
        var codes = await context.VerificationCodes.AsNoTracking()
                                        .Where(us => us.Id==userid)
                                       .ToListAsync(cancellationToken);

          if(!codes.Any())return false; 

          context.VerificationCodes.RemoveRange(codes);
          return true;                          
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}

