// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionPendientes/GetImportacionPendientesQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionPendientes
{
    public class GetImportacionPendientesQueryHandler
        : IRequestHandler<GetImportacionPendientesQuery, List<ImportacionPendienteResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetImportacionPendientesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ImportacionPendienteResponse>> Handle(GetImportacionPendientesQuery request, CancellationToken ct)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.ImportacionCode.Trim().ToUpperInvariant();
            var todos = string.IsNullOrWhiteSpace(request.ProveedorCode)
                        || string.Equals(request.ProveedorCode.Trim(), "T", StringComparison.OrdinalIgnoreCase);
            var proveedor = todos ? null : request.ProveedorCode!.Trim().ToUpperInvariant();

            var existe = await _uow.Logistica.Transacciones.Importaciones.Query()
                .AsNoTracking().AnyAsync(i => i.PlantaCode == planta && i.Code == code, ct);
            if (!existe)
                throw new KeyNotFoundException($"Importación {planta}/{code} no encontrada.");

            var filas = await _uow.Logistica.Transacciones.ImportacionDetalles.Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta && d.ImportacionCode == code
                            && d.Estado == EstadoImportacion.Pendiente
                            && (proveedor == null || d.ProveedorCode == proveedor))
                .OrderBy(d => d.ProveedorCode).ThenBy(d => d.ArticuloCode)
                .Select(d => new
                {
                    d.ProveedorCode, d.ArticuloCode,
                    ArticuloName = d.Articulo.Name,
                    d.Articulo.UnidadMedidaCode,
                    d.Cantidad, d.ValorDet
                })
                .ToListAsync(ct);

            return filas.Select(f => new ImportacionPendienteResponse
            {
                ProveedorCode = f.ProveedorCode,
                ArticuloCode = f.ArticuloCode,
                ArticuloName = f.ArticuloName,
                UnidadMedidaCode = f.UnidadMedidaCode,
                Cantidad = f.Cantidad,
                Precio = f.Cantidad == 0 ? 0 : Math.Round(Math.Round(f.ValorDet, 6, MidpointRounding.AwayFromZero) / f.Cantidad, 6, MidpointRounding.AwayFromZero),
                Total = f.ValorDet
            }).ToList();
        }
    }
}
