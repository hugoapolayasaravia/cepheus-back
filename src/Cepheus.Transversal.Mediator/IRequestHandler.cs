namespace MediatR;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Variante para requests SIN valor de retorno (TRequest : IRequest, no IRequest&lt;T&gt;).
/// El handler concreto solo implementa "Task Handle(...)" (sin genérico) — la
/// adaptación a Task&lt;Unit&gt; que exige IRequestHandler&lt;TRequest, Unit&gt;
/// ocurre acá adentro, vía default interface implementation (C# 8+), sin que
/// el código del handler necesite saber que existe Unit.
/// </summary>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit>
{
    new Task Handle(TRequest request, CancellationToken cancellationToken);

    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken);
        return Unit.Value;
    }
}
