using Microsoft.EntityFrameworkCore.Storage;

namespace DevKit.MediatR.Pipelines;

public interface IDevKitDbContext
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
