using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.GetPermissionCatalog
{
    /// <summary>
    /// Catálogo completo de permisos asignables (Módulo -> Submódulo -> Programa -> Permiso), solo con
    /// lo activo. Lo usa la pantalla de Roles para armar el árbol de casillas en una sola llamada.
    /// </summary>
    public record GetPermissionCatalogQuery() : IRequest<List<CatalogoModuloResponse>>;
}
