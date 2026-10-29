using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.GetLineasDisponiblesProgramacionConcreto;

/// <summary>Líneas CO de la OT con cantidad aprobada aún no programada (dw_ges_adicional_x_programa_pendiente).</summary>
public sealed record GetLineasDisponiblesProgramacionConcretoQuery(string OrdenTrabajoCode)
    : IRequest<List<LineaDisponibleResponse>>;

public sealed class GetLineasDisponiblesProgramacionConcretoQueryHandler
    : IRequestHandler<GetLineasDisponiblesProgramacionConcretoQuery, List<LineaDisponibleResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetLineasDisponiblesProgramacionConcretoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<LineaDisponibleResponse>> Handle(
        GetLineasDisponiblesProgramacionConcretoQuery request,
        CancellationToken ct)
    {
        var code = ProgramacionOrdenTrabajo.NormalizeCode(request.OrdenTrabajoCode);
        var ot = await ProgramacionOrdenTrabajo.LoadProgramableAsync(_uow, code, ct);

        return ot.Detalles
            .Where(d => d.Status != EstadoDetalleOrdenTrabajoVenta.Anulado
                        && d.NegocioCode == OrdenTrabajoVentaRules.NegocioConcreto
                        && ProgramacionOrdenTrabajo.Available(d) > 0m)
            .OrderBy(d => d.Item)
            .Select(d => new LineaDisponibleResponse
            {
                Item = d.Item,
                ProductoTipoCode = d.ProductoTipoCode,
                ProductoCode = d.ProductoCode,
                ProductoName = d.Producto.Name,
                UnitCode = d.Producto.UnitCode,
                OrderQuantity = d.Quantity,
                ApprovedQuantity = d.ApprovedQuantity,
                ProgrammedQuantity = d.ProgrammedQuantity,
                AvailableQuantity = ProgramacionOrdenTrabajo.Available(d)
            })
            .ToList();
    }
}
