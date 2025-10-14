using FSP.Domain.Entities.Core;

namespace FSP.Domain.Interfaces.RepositoryPattern;

public interface IUnitOfWork
{
    IGenericRepo<League> Leagues { get; }
    IGenericRepo<Club> Clubs { get; }
    IGenericRepo<Player> Players { get; }
    IGenericRepo<Goalkeeping> Goalkeepings { get; }
    IGenericRepo<Shooting> Shootings { get; }


    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task SaveChangesAsync();
}
