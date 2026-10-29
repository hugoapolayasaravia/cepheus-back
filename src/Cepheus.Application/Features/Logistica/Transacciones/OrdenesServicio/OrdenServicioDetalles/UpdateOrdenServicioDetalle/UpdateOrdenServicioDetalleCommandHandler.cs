using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.UpdateOrdenServicioDetalle;

public sealed class UpdateOrdenServicioDetalleCommandHandler
    : IRequestHandler<UpdateOrdenServicioDetalleCommand, OrdenServicioResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateOrdenServicioDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenServicioResponse> Handle(UpdateOrdenServicioDetalleCommand request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.PlantaCode);
        var code = OrdenServicioRules.Normalize(request.OrdenServicioCode);

        var os = await OrdenServicioDetalleOperations.LoadEditableAsync(_uow, planta, code, ct);

        var existente = os.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber)
            ?? throw new KeyNotFoundException(
                $"La línea {request.ItemNumber} no existe en la Orden de Servicio {planta}/{code}.");

        var articuloEnviado = OrdenServicioRules.Normalize(request.Detalle.ArticuloCode);
        if (articuloEnviado != existente.ArticuloCode)
            throw new InvalidOperationException(
                "No se puede cambiar el artículo de una línea. Elimine la línea y agregue una nueva.");

        var nuevo = await OrdenServicioRules.BuildDetalleAsync(_uow, planta, request.Detalle, ct);
        OrdenServicioRules.CopyEditableFields(nuevo, existente);

        var comprobante = await _uow.Comunes.ComprobantesPago.Query().AsNoTracking()
            .FirstAsync(c => c.Code == os.ComprobantePagoCode, ct);
        await OrdenServicioTotalsCalculator.RecalculateAsync(_uow, os, comprobante, ct);

        await OrdenServicioDetalleOperations.SyncValeAsync(_uow, os, ct);

        await OrdenServicioDetalleOperations.SaveAsync(_uow, ct);

        return await OrdenServicioReader.GetAsync(_uow, planta, code, ct);
    }
}
