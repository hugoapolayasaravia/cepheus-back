namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Abstrae el acceso al usuario autenticado actual (leído de los claims del JWT
    /// vía HttpContext en la implementación de Infrastructure). Se usa para:
    /// - Auditoría automática (CreatedBy/UpdatedBy) en ApplicationDbContext.SaveChangesAsync.
    /// - Lógica de negocio que necesite saber "quién" está haciendo la operación
    ///   (ej. no permitir que un usuario se desactive a sí mismo).
    /// </summary>
    public interface ICurrentUserService
    {
        int? UserId { get; }

        string? Login { get; }

        string? Email { get; }

        /// <summary>
        /// "FirstName LastName" armado desde los claims del JWT. Se usa para
        /// CreatedBy/UpdatedBy en la auditoría automática (ApplicationDbContext.SaveChangesAsync).
        /// Null si no hay usuario autenticado (ej. bootstrap de Register).
        /// </summary>
        string? FullName { get; }

        /// <summary>
        /// Rol que el cliente declara como activo (encabezado X-Cepheus-Role). Es solo una
        /// petición: se valida contra los roles reales del usuario (ver ActiveRole.Pick).
        /// </summary>
        int? RequestedRoleId { get; }
    }



}
