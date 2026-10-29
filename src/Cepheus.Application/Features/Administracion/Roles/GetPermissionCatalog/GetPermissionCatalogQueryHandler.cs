using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.GetPermissionCatalog
{
    public class GetPermissionCatalogQueryHandler : IRequestHandler<GetPermissionCatalogQuery, List<CatalogoModuloResponse>>
    {
        // Orden de los permisos dentro de un programa; los que no estén aquí van después, por código.
        private static readonly string[] OrdenPermisos = { "VIEW", "CREATE", "UPDATE", "DELETE", "EXPORT_EXCEL", "EXPORT_PDF" };

        private readonly IUnitOfWork _uow;

        public GetPermissionCatalogQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<CatalogoModuloResponse>> Handle(GetPermissionCatalogQuery request, CancellationToken cancellationToken)
        {
            var filas = await _uow.Administracion.Permissions.Query()
                .AsNoTracking()
                .Where(p => p.IsActive
                    && p.Programa.IsActive
                    && p.Programa.Submodulo.IsActive
                    && p.Programa.Submodulo.Modulo.IsActive)
                .Select(p => new
                {
                    PermisoId = p.Id,
                    PermisoCode = p.Code,
                    PermisoName = p.Name,

                    ModuloId = p.Programa.Submodulo.Modulo.Id,
                    ModuloCode = p.Programa.Submodulo.Modulo.Code,
                    ModuloName = p.Programa.Submodulo.Modulo.Name,
                    ModuloOrden = p.Programa.Submodulo.Modulo.DisplayOrder,

                    SubmoduloId = p.Programa.Submodulo.Id,
                    SubmoduloCode = p.Programa.Submodulo.Code,
                    SubmoduloName = p.Programa.Submodulo.Name,
                    SubmoduloOrden = p.Programa.Submodulo.DisplayOrder,

                    ProgramaId = p.Programa.Id,
                    ProgramaCode = p.Programa.Code,
                    ProgramaName = p.Programa.Name,
                    ProgramaOrden = p.Programa.DisplayOrder
                })
                .ToListAsync(cancellationToken);

            return filas
                .GroupBy(f => new { f.ModuloId, f.ModuloCode, f.ModuloName, f.ModuloOrden })
                .OrderBy(m => m.Key.ModuloOrden).ThenBy(m => m.Key.ModuloName)
                .Select(m => new CatalogoModuloResponse
                {
                    Id = m.Key.ModuloId,
                    Code = m.Key.ModuloCode,
                    Name = m.Key.ModuloName,
                    Submodulos = m
                        .GroupBy(f => new { f.SubmoduloId, f.SubmoduloCode, f.SubmoduloName, f.SubmoduloOrden })
                        .OrderBy(s => s.Key.SubmoduloOrden).ThenBy(s => s.Key.SubmoduloName)
                        .Select(s => new CatalogoSubmoduloResponse
                        {
                            Id = s.Key.SubmoduloId,
                            Code = s.Key.SubmoduloCode,
                            Name = s.Key.SubmoduloName,
                            Programas = s
                                .GroupBy(f => new { f.ProgramaId, f.ProgramaCode, f.ProgramaName, f.ProgramaOrden })
                                .OrderBy(g => g.Key.ProgramaOrden).ThenBy(g => g.Key.ProgramaName)
                                .Select(g => new CatalogoProgramaResponse
                                {
                                    Id = g.Key.ProgramaId,
                                    Code = g.Key.ProgramaCode,
                                    Name = g.Key.ProgramaName,
                                    Permisos = g
                                        .Select(f => new CatalogoPermisoResponse
                                        {
                                            Id = f.PermisoId,
                                            Code = f.PermisoCode,
                                            Name = f.PermisoName
                                        })
                                        .OrderBy(x => PosicionPermiso(x.Code)).ThenBy(x => x.Code)
                                        .ToList()
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();
        }

        private static int PosicionPermiso(string codigo)
        {
            var posicion = Array.IndexOf(OrdenPermisos, codigo);
            return posicion >= 0 ? posicion : OrdenPermisos.Length;
        }
    }
}
