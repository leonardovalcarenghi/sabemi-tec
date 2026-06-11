using Microsoft.Extensions.Logging;
using Sabemi.Application.Abstractions;
using Sabemi.Infra.Persistence.Contexts;
namespace Sabemi.Infra.Persistence;

internal class UnitOfWork(ApplicationDbContext context, ILogger logger) : IUnitOfWork, IDisposable
{
    private bool _disposed = false;

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar as alterações no banco de dados.");
            return false;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing) context.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
