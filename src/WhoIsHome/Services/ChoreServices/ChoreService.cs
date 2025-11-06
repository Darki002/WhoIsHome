using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.External.Models;

namespace WhoIsHome.Services.ChoreServices;

public class ChoreService(IDbContextFactory<WhoIsHomeContext> contextFactory) : IChoreService
{
    public async Task<ChoreModel> GetAsync(int id, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await db.Chores
            .Include(c => c.AssignedUser)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        
        if (result is null) throw new NotFoundException($"No OneTimeEvent found with the id {id}.");
        return result;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.Chores.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        if (result is null) throw new NotFoundException($"No OneTimeEvent found with the id {id}.");

        context.Chores.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChoreModel> CreateAsync(string title, string description, ushort repetition, int? assignedUserId, CancellationToken cancellationToken)
    {
        var chore = new ChoreModel
        {
            Title = title,
            Description = description,
            Repetition = repetition,
            AssignedUserId = assignedUserId
        };

        ValidateChore(chore);
        
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.Chores.AddAsync(chore, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<ChoreModel> UpdateAsync(int id, string title, string description, ushort repetition, int? assignedUserId,
        CancellationToken cancellationToken)
    {
        var chore = await GetAsync(id, cancellationToken);
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        chore.Title = title;
        chore.Description = description;
        chore.Repetition = repetition;
        chore.AssignedUserId = assignedUserId;

        ValidateChore(chore);

        var result = context.Chores.Update(chore);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    private static void ValidateChore(ChoreModel model)
    {
        if (model.Title.Length > 50) throw new InvalidModelException("Title must be less then or equal to 50 characters long");
        if (model.Description.Length > 200) throw new InvalidModelException("Title must be less then or equal to 200 characters long");
    }
}