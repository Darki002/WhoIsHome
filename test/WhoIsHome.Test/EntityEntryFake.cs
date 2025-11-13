using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WhoIsHome.Test;

public class EntityEntryFake<T> : EntityEntry<T> where T : class
{
    public override T Entity { get; }

#pragma warning disable EF1001
    public EntityEntryFake(T entity) : base(null!)
#pragma warning restore EF1001
    {
        Entity = entity;
    }
}