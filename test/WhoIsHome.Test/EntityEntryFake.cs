using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using WhoIsHome.External.Database;

namespace WhoIsHome.Test;

public class EntityEntryFake<TEntity> : EntityEntry<TEntity> where TEntity : class
{
    public override TEntity Entity { get; }

#pragma warning disable EF1001
    public EntityEntryFake(TEntity entity) : base(null!)
#pragma warning restore EF1001
    {
        Entity = entity;
    }
}

public static class ChangeTrackingExtansions
{
    public static void AddChangeTracking<TEntity>(
        this Mock<WhoIsHomeContext> mock,
        Expression<Func<WhoIsHomeContext, EntityEntry<TEntity>>> setup,
        Action<TEntity>? edit = null) where TEntity : class
    {
        mock.Setup(setup).Returns<TEntity>(t =>
        {
            var result = new EntityEntryFake<TEntity>(t);
            edit?.Invoke(t);
            return result;
        });
    }
    
    public static void AddChangeTrackingWithCt<TEntity>(
        this Mock<WhoIsHomeContext> mock,
        Expression<Func<WhoIsHomeContext, EntityEntry<TEntity>>> setup,
        Action<TEntity>? edit = null) where TEntity : class
    {
        mock.Setup(setup).Returns<TEntity, CancellationToken>((t, _) =>
        {
            var result = new EntityEntryFake<TEntity>(t);
            edit?.Invoke(t);
            return result;
        });
    }
    
    public static void AddChangeTracking<TEntity>(
        this Mock<WhoIsHomeContext> mock,
        Expression<Func<WhoIsHomeContext, ValueTask<EntityEntry<TEntity>>>> setup,
        Action<TEntity>? edit = null) where TEntity : class
    {
        mock.Setup(setup).Returns<TEntity>(t =>
        {
            var result = new EntityEntryFake<TEntity>(t);
            edit?.Invoke(t);
            return ValueTask.FromResult<EntityEntry<TEntity>>(result);
        });
    }
    
    public static void AddChangeTrackingWithCt<TEntity>(
        this Mock<WhoIsHomeContext> mock,
        Expression<Func<WhoIsHomeContext, ValueTask<EntityEntry<TEntity>>>> setup,
        Action<TEntity>? edit = null) where TEntity : class
    {
        mock.Setup(setup).Returns<TEntity, CancellationToken>((t, _) =>
        {
            var result = new EntityEntryFake<TEntity>(t);
            edit?.Invoke(t);
            return ValueTask.FromResult<EntityEntry<TEntity>>(result);
        });
    }
}