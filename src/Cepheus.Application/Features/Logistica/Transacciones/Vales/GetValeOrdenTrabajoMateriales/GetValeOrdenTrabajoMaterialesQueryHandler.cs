using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeOrdenTrabajoMateriales;

public sealed class GetValeOrdenTrabajoMaterialesQueryHandler
    : IRequestHandler<GetValeOrdenTrabajoMaterialesQuery, ValeOrdenTrabajoMaterialesResponse>
{
    private readonly IUnitOfWork _uow;

    public GetValeOrdenTrabajoMaterialesQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ValeOrdenTrabajoMaterialesResponse> Handle(
        GetValeOrdenTrabajoMaterialesQuery request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var otCode = ValeRules.Normalize(request.OrdenTrabajoCode);

        var ot = await ValeCabecera.LoadOrdenTrabajoEnEjecucionAsync(_uow, planta, otCode, ct);

        var query = _uow.Mantenimiento.Transacciones.OTRMateriales
            .Query()
            .AsNoTracking()
            .Include(m => m.Articulo)
            .Where(m => m.PlantaCode == planta && m.OrdenTrabajoCode == otCode);

        if (!request.Todos)
        {
            query = query.Where(m => m.EstadoCode == ValeRules.OtMaterialPendiente);
        }

        var materiales = await query
            .OrderBy(m => m.ArticuloCode)
            .ThenBy(m => m.FechaProceso)
            .ToListAsync(ct);

        return new ValeOrdenTrabajoMaterialesResponse
        {
            PlantaCode = ot.PlantaCode,
            OrdenTrabajoCode = ot.Code,
            Description = ot.Description,
            ResponsableCode = ot.ResponsableCode,
            SubCentroCostoCode = ot.SubCentroCostoCode,
            SubCentroEjecutorCode = ot.SubCentroEjecutorCode,
            EquipoCode = ot.EquipoCode,
            Materiales = materiales
                .Select(m => new ValeMaterialOtResponse
                {
                    ArticuloCode = m.ArticuloCode,
                    ArticuloName = m.Articulo.Name,
                    UnidadMedidaCode = m.Articulo.UnidadMedidaCode,
                    FechaProceso = m.FechaProceso,
                    Cantidad = m.Cantidad,
                    CostoUnitario = m.CostoUnitario,
                    CostoTotal = m.CostoTotal,
                    EstadoCode = m.EstadoCode
                })
                .ToList()
        };
    }
}
