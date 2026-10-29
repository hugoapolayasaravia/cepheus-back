using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR;

/// <summary>
/// Implementación mínima del despachador. Por cada Send&lt;TResponse&gt;:
///   1. Resuelve IRequestHandler&lt;TRequestConcreto, TResponse&gt; desde el DI container.
///   2. Resuelve todos los IPipelineBehavior&lt;TRequestConcreto, TResponse&gt; registrados
///      (ej. ValidationBehavior) y arma la cadena alrededor del handler.
///   3. Ejecuta la cadena.
/// Cachea los MethodInfo resueltos por reflexión (por tipo) para no pagar ese
/// costo en cada request.
/// </summary>
public class Mediator : ISender
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandlerMethodCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> BehaviorMethodCache = new();

    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"No se encontró ningún IRequestHandler<{requestType.Name}, {responseType.Name}>. " +
                "Verificá que la clase *Handler exista y que su ensamblado esté registrado en AddMediatR.");

        var handleMethod = HandlerMethodCache.GetOrAdd(handlerType, static t => t.GetMethod("Handle")!);

        RequestHandlerDelegate<TResponse> pipeline = () =>
            (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviorHandleMethod = BehaviorMethodCache.GetOrAdd(behaviorType, static t => t.GetMethod("Handle")!);

        var behaviors = _serviceProvider.GetServices(behaviorType).Cast<object>().Reverse();

        foreach (var behavior in behaviors)
        {
            var next = pipeline;
            pipeline = () => (Task<TResponse>)behaviorHandleMethod.Invoke(
                behavior, new object[] { request, next, cancellationToken })!;
        }

        return pipeline();
    }
}
