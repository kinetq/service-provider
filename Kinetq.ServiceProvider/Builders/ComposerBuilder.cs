using Kinetq.ServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kinetq.ServiceProvider.Builders;

public abstract class ComposerBuilder<TResult>
{
    private readonly ISessionManager _sessionManager;
    protected IDbContextTransaction Transaction { get; set; }

    protected ComposerBuilder(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public ComposerBuilder<TResult> WithTransactionFor(string sessionName)
    {
        var session = _sessionManager.GetSessionFrom(sessionName);
        Transaction = session.Database.BeginTransaction();

        return this;
    }

    protected abstract Task<TResult> Arrange();

    public Task<TResult> Execute()
    {
        return Arrange();
    }
}