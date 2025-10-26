using WhoIsHome.External.Models;

namespace WhoIsHome.Services.ChoreServices;

public interface IChoreService : IAggregateService<ChoreModel>
{
    Task<ChoreModel> CreateAsync(string title, string description, ushort repetition, int? assignedUserId,
        CancellationToken cancellationToken);

    Task<ChoreModel> UpdateAsync(int id, string title, string description, ushort repetition, int? assignedUserId,
        CancellationToken cancellationToken);
}