using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra ISender (Mediator) y escanea el/los ensamblado(s) indicados
    /// buscando toda clase concreta que implemente IRequestHandler&lt;,&gt;
    /// (2 parámetros de tipo — incluye también los que llegan por herencia
    /// desde IRequestHandler&lt;TRequest&gt; de 1 solo parámetro, ej. LogoutCommandHandler),
    /// registrándola contra su interfaz cerrada correspondiente.
    /// Los IPipelineBehavior&lt;,&gt; (ej. ValidationBehavior) NO se registran acá:
    /// eso sigue siendo responsabilidad explícita de Program.cs, igual que antes.
    /// </summary>
    public static IServiceCollection AddMediatR(
        this IServiceCollection services, Action<MediatRServiceConfiguration> configure)
    {
        var config = new MediatRServiceConfiguration();
        configure(config);

        services.AddScoped<ISender, Mediator>();

        foreach (var assembly in config.AssembliesToRegister)
        {
            RegisterClosedGenericImplementations(services, assembly, typeof(IRequestHandler<,>));
        }

        return services;
    }

    private static void RegisterClosedGenericImplementations(
        IServiceCollection services, Assembly assembly, Type openGenericInterface)
    {
        var concreteTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var type in concreteTypes)
        {
            var matchingInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface);

            foreach (var closedInterface in matchingInterfaces)
            {
                services.AddTransient(closedInterface, type);
            }
        }
    }
}
