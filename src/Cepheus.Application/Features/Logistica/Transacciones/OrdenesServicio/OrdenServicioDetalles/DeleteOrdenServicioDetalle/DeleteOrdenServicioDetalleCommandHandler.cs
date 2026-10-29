using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.DeleteOrdenServicioDetalle;

public sealed class DeleteOrdenServicioDetalleCommandHandler
    : IRequestHandler<DeleteOrdenServicioDetalleCommand, OrdenServicioResponse>
{
    private readonly IUnitOfWork _uow;

    public DeleteOrdenServicioDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenServicioResponse> Handle(DeleteOrdenServicioDetalleCommand request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.PlantaCode);
        var code = OrdenServicioRules.Normalize(request.OrdenServicioCode);

        var os = await OrdenServicioDetalleOperations.LoadEditableAsync(_uow, planta, code, ct);

        var linea = os.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber)
            ?? throw new KeyNotFoundException(
                $"La línea {request.ItemNumber} no existe en la Orden de Servicio {planta}/{code}.");

        if (os.Detalles.Count == 1)
            throw new InvalidOperationException("La Orden de Servicio debe conservar al menos una línea de detalle.");

        os.Detalles.Remove(linea);
        _uow.Logistica.Transacciones.OrdenServicioDetalles.Remove(linea);

        var comprobante = await _uow.Comunes.ComprobantesPago.Query().AsNoTracking()
            .FirstAsync(c => c.Code == os.ComprobantePagoCode, ct);
        await OrdenServicioTotalsCalculator.RecalculateAsync(_uow, os, comprobante, ct);

        await OrdenServicioDetalleOperations.SyncValeAsync(_uow, os, ct);

        await OrdenServicioDetalleOperations.SaveAsync(_uow, ct);

        return await OrdenServicioReader.GetAsync(_uow, planta, code, ct);
    }
}
