using Core.Domain;
using Core.Ports;
using Infrastructure.Other;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EF;

public class EfAccountsRepository(DatabaseContext ctx) : AccountsRepository
{
    public Task<Account?> FindByEmail(string email)
    {
        return ctx.Accounts.FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<Account?> FindById(Guid id)
    {
        return ctx.Accounts.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Account> FindByIdOrFail(Guid id)
    {
        var found = await FindById(id);
        if (found is null)
        {
            throw new EntitySearchFailure(); // TODO:
        }

        return found;
    }

    public async Task Create(Account account)
    {
        await ctx.Accounts.AddAsync(account);
    }

    public Task Update(Account account)
    {
        ctx.Accounts.Update(account);
        return Task.CompletedTask;
    }
}
