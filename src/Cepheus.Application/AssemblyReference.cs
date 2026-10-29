namespace Cepheus.Application;

/// <summary>
/// Clase marcadora sin lógica: existe únicamente para que
/// RegisterServicesFromAssembly() y AddValidatorsFromAssembly()
/// puedan ubicar el ensamblado Cepheus.Application por reflexión
/// (typeof(AssemblyReference).Assembly) sin depender de un tipo
/// de negocio concreto que pueda desaparecer o moverse.
/// </summary>
public class AssemblyReference
{
}
