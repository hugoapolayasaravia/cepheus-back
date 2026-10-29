using System.Reflection;

namespace MediatR;

public class MediatRServiceConfiguration
{
    internal List<Assembly> AssembliesToRegister { get; } = new();

    public MediatRServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);
        return this;
    }
}
