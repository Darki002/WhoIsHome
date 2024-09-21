namespace WhoIsHome.Services;

public interface IService<T>
{
    Task<T> GetAsync(int id, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}