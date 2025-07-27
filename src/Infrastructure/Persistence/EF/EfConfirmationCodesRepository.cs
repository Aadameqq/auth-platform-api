using System.Security.Cryptography;
using Core.Domain;
using Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EF;

public class EfConfirmationCodesRepository(DatabaseContext ctx) : ConfirmationCodesRepository
{
    public async Task Create(ConfirmationCode confirmationCode)
    {
        await ctx.ConfirmationCodes.AddAsync(confirmationCode);
    }

    public async Task<ConfirmationCode?> FindByCode(string code, ConfirmableAction action)
    {
        return await ctx.ConfirmationCodes.FirstOrDefaultAsync(c =>
            c.Code == code && c.Action == action
        );
    }

    public async Task<ConfirmationCode?> FindByAccount(Account account, ConfirmableAction action)
    {
        return await ctx.ConfirmationCodes.FirstOrDefaultAsync(c =>
            c.OwnerId == account.Id && c.Action == action
        );
    }

    public async Task Delete(ConfirmationCode confirmationCode)
    {
        throw new NotImplementedException();
    }

    public string GenerateCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[4];

        rng.GetBytes(randomNumber);

        var generatedNumber = BitConverter.ToInt32(randomNumber, 0) & 0x7FFFFFFF;

        return (generatedNumber % 1000000).ToString("D6");
    }
}
