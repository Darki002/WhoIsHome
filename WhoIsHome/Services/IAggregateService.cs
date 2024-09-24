namespace WhoIsHome.Services;

public interface IAggregateService<T>
{
    Task<T> GetAsync(int id, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}