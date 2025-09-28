using DevKit.MediatR.Pipelines;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevKit.MediatR;

public sealed class TestDbContext(DbContextOptions<TestDbContext> options) :
    DbContext(options),
    IDevKitDbContext
{
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => Database.BeginTransactionAsync(cancellationToken);
}
