using System.Linq.Expressions;
using FSP.Domain.Interfaces.RepositoryPattern;
using FSP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FSP.Infrastructure.Implementations.RepositoryPattern;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepo(ApplicationDbContext dbContext)
    {
        this._dbContext = dbContext;
        this._dbSet = dbContext.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await this._dbSet.AddAsync(entity);
    }

    public async Task<bool> Delete(object id)
    {
        T? entity = this._dbSet.Find(id);
        if (entity == null) return false;
        this._dbSet.Remove(entity);
        await this._dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await this._dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null
    )
    {
        IQueryable<T> query = _dbSet;

        if (include != null)
            query = include(query);

        if (predicate != null)
            query = query.Where(predicate);

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await this._dbSet.FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(object id, bool asNoTracking = false)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        var entityType = _dbContext.Model.FindEntityType(typeof(T))
            ?? throw new InvalidOperationException($"Type {typeof(T).Name} is not part of the EF model.");

        var primaryKey = entityType.FindPrimaryKey()
            ?? throw new InvalidOperationException($"Entity {typeof(T).Name} has no primary key defined.");

        var keyName = primaryKey.Properties
                                .Select(p => p.Name)
                                .Single();

        return await query.FirstOrDefaultAsync(e =>
            EF.Property<object>(e, keyName).Equals(id));
    }

    public async Task<T?> GetByAsync(Expression<Func<T, bool>> predicate)
    {
        return await this._dbSet.FirstOrDefaultAsync(predicate);
    }
    public async Task<T?> GetByAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<T> query = this._dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        if (include is not null)
            query = include(query);

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public bool Update(T entity)
    {
        this._dbSet.Attach(entity);
        this._dbContext.Entry(entity).State = EntityState.Modified;
        return true;
    }

    public async Task<T?> GetByIdWithIncludesAsync(
        Guid id,
        Expression<Func<T, Guid>> keySelector,
        params Expression<Func<T, object>>[] includes
    )
    {
        IQueryable<T> query = this._dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        var parameter = keySelector.Parameters[0];
        var body = Expression.Equal(keySelector.Body, Expression.Constant(id));
        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

        return await query.FirstOrDefaultAsync(lambda);
    }

    public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = this._dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> query = _dbSet;
        if (predicate != null)
            query = query.Where(predicate);
        return await query.CountAsync();
    }
}

