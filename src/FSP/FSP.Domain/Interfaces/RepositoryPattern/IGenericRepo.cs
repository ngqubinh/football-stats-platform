using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace FSP.Domain.Interfaces.RepositoryPattern;

public interface IGenericRepo<T> where T : class
{
    Task<T?> GetByIdAsync(object id);
    Task<T?> GetByIdAsync(object id, bool asNoTracking = false);
    Task<T?> GetByAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdWithIncludesAsync(
        Guid id,
        Expression<Func<T, Guid>> keySelector,
        params Expression<Func<T, object>>[] includes
    );
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync( Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IQueryable<T>>? include = null);
    Task<T?> GetByAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<T>> GetAllWithIncludesAsync(
        params Expression<Func<T, object>>[] includes
    );
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task AddAsync(T entity);
    bool Update(T entity);
    Task<bool> Delete(object id);
}
