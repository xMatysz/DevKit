using DevKit.MediatR.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace DevKit.MediatR;

public static class MediatRExtensions
{
    public static void AddValidationBehaviour(this MediatRServiceConfiguration cfg, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(cfg);

        cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>), lifetime);
    }

    public static void AddUnitOfWork(this MediatRServiceConfiguration cfg, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(cfg);

        cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>), lifetime);
    }
}
