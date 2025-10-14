using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.RepositoryPattern;
using FSP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FSP.Infrastructure.Implementations.RepositoryPattern;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction? _transaction;

    private IGenericRepo<League>? _league;
    private IGenericRepo<Club>? _club;
    private IGenericRepo<Player>? _player;
    private IGenericRepo<Goalkeeping>? _goalkeeping;
    private IGenericRepo<Shooting>? _shooting;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    // Lazy Initializations
    public IGenericRepo<League> Leagues => this._league ??= new GenericRepo<League>(this._dbContext);
    public IGenericRepo<Club> Clubs => this._club ??= new GenericRepo<Club>(this._dbContext);
    public IGenericRepo<Player> Players => this._player ??= new GenericRepo<Player>(this._dbContext);
    public IGenericRepo<Goalkeeping> Goalkeepings => this._goalkeeping ?? new GenericRepo<Goalkeeping>(this._dbContext);
    public IGenericRepo<Shooting> Shootings => this._shooting ?? new GenericRepo<Shooting>(this._dbContext);

    // Transactions
    public async Task BeginTransactionAsync()
    {
        if (this._transaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");
        this._transaction = await this._dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (this._transaction == null)
            throw new InvalidOperationException("No transaction to commit.");
        try
        {
            await this.SaveChangesAsync();
            await this._transaction.CommitAsync();
        }
        catch
        {
            await this.RollbackAsync();
            throw;
        }
        finally
        {
            this.Dispose();
            this._transaction = null;
        }
    }

    public void Dispose()
    {
        this._transaction?.Dispose();
        this. _transaction = null;
        this._dbContext.Dispose();
    }

    public async Task RollbackAsync()
    {
        if (this._transaction == null)
            throw new InvalidOperationException("No transaction to rollback.");

        await this._transaction.RollbackAsync();
        this._transaction.Dispose();
        this._transaction = null;
    }

    public async Task SaveChangesAsync()
    {
        await this._dbContext.SaveChangesAsync();
    }
}
