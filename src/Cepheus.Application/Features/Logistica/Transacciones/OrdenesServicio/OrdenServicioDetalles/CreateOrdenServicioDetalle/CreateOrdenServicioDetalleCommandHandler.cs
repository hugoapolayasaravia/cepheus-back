using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.CreateOrdenServicioDetalle;

public sealed class CreateOrdenServicioDetalleCommandHandler
    : IRequestHandler<CreateOrdenServicioDetalleCommand, OrdenServicioResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateOrdenServicioDetalleCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenServicioResponse> Handle(CreateOrdenServicioDetalleCommand request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.PlantaCode);
        var code = OrdenServicioRules.Normalize(request.OrdenServicioCode);

        var os = await OrdenServicioDetalleOperations.LoadEditableAsync(_uow, planta, code, ct);

        var detalle = await OrdenServicioRules.BuildDetalleAsync(_uow, planta, request.Detalle, ct);
        detalle.OrdenServicioCode = code;
        detalle.ItemNumber = os.Detalles.Count == 0 ? 1 : os.Detalles.Max(d => d.ItemNumber) + 1;

        os.Detalles.Add(detalle);
        await _uow.Logistica.Transacciones.OrdenServicioDetalles.AddAsync(detalle, ct);

        var comprobante = await _uow.Comunes.ComprobantesPago.Query().AsNoTracking()
            .FirstAsync(c => c.Code == os.ComprobantePagoCode, ct);
        await OrdenServicioTotalsCalculator.RecalculateAsync(_uow, os, comprobante, ct);

        await OrdenServicioDetalleOperations.SyncValeAsync(_uow, os, ct);

        await OrdenServicioDetalleOperations.SaveAsync(_uow, ct);

        return await OrdenServicioReader.GetAsync(_uow, planta, code, ct);
    }
}
