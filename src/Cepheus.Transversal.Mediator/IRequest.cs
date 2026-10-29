namespace MediatR;

public interface IBaseRequest
{
}

public interface IRequest<out TResponse> : IBaseRequest
{
}

/// <summary>
/// Request sin valor de retorno. Equivale a IRequest&lt;Unit&gt; — mismo
/// mecanismo que usa MediatR (ver IRequestHandler&lt;TRequest&gt; para cómo
/// se adapta Task a Task&lt;Unit&gt; sin que el handler tenga que saberlo).
/// </summary>
public interface IRequest : IRequest<Unit>
{
}
