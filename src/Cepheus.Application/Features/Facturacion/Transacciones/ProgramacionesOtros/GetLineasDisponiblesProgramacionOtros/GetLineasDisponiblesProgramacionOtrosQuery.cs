using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.GetLineasDisponiblesProgramacionOtros;

/// <summary>Líneas no Concreto de la OT con cantidad aprobada aún no programada (dw_ges_adicional_x_programa_pendiente_o).</summary>
public sealed record GetLineasDisponiblesProgramacionOtrosQuery(string OrdenTrabajoCode)
    : IRequest<List<LineaDisponibleOtrosResponse>>;

public sealed class GetLineasDisponiblesProgramacionOtrosQueryHandler
    : IRequestHandler<GetLineasDisponiblesProgramacionOtrosQuery, List<LineaDisponibleOtrosResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetLineasDisponiblesProgramacionOtrosQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<LineaDisponibleOtrosResponse>> Handle(
        GetLineasDisponiblesProgramacionOtrosQuery request,
        CancellationToken ct)
    {
        var code = ProgramacionOrdenTrabajo.NormalizeCode(request.OrdenTrabajoCode);
        var ot = await ProgramacionOrdenTrabajo.LoadProgramableAsync(_uow, code, ct);

        return ot.Detalles
            .Where(d => d.Status != EstadoDetalleOrdenTrabajoVenta.Anulado
                        && ProgramacionOtrosRules.IsOtros(d)
                        && ProgramacionOrdenTrabajo.Available(d) > 0m)
            .OrderBy(d => d.Item)
            .Select(d => new LineaDisponibleOtrosResponse
            {
                Item = d.Item,
                NegocioCode = d.NegocioCode,
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
