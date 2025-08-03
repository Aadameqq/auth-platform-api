using System.Security.Cryptography;
using Core.Domain;
using Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EF;

public class EfConfirmationsRepository(DatabaseContext ctx) : ConfirmationsRepository
{
    public async Task Create(Confirmation confirmation)
    {
        await ctx.Confirmations.AddAsync(confirmation);
    }

    public Task<Confirmation?> FindById(Guid id)
    {
        return ctx.Confirmations.FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<Confirmation?> FindByAccount(Account account, ConfirmableAction action)
    {
        return ctx.Confirmations.FirstOrDefaultAsync(c =>
            c.OwnerId == account.Id && c.Action == action
        );
    }

    public Task Delete(Confirmation confirmation)
    {
        ctx.Confirmations.Remove(confirmation);
        return Task.CompletedTask;
    }

    public Task Update(Confirmation confirmation)
    {
        ctx.Confirmations.Update(confirmation);
        return Task.CompletedTask;
    }

    public string GenerateCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[4];

        rng.GetBytes(randomNumber);

        var generatedNumber = BitConverter.ToInt32(randomNumber, 0) & 0x7FFFFFFF;

        return (generatedNumber % 1000000).ToString("D6");
    }

    public Task<Confirmation?> FindByCode(string code, ConfirmableAction action)
    {
        return ctx.Confirmations.FirstOrDefaultAsync(c => c.Code == code && c.Action == action);
    }
}
