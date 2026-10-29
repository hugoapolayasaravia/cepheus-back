using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.UpdateOrdenServicioSalida;

public sealed class UpdateOrdenServicioSalidaCommandHandler
    : IRequestHandler<UpdateOrdenServicioSalidaCommand, OrdenServicioSalidaResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateOrdenServicioSalidaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenServicioSalidaResponse> Handle(UpdateOrdenServicioSalidaCommand request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();
        var code = request.Code.Trim().ToUpperInvariant();
        var trabajador = request.TrabajadorCode.Trim().ToUpperInvariant();

        var vale = await _uow.Logistica.Transacciones.OrdenesServicioSalida.Query()
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"El vale de salida {planta}/{code} no existe.");

        if (vale.Estado == EstadoOrdenServicioSalida.Anulado)
            throw new InvalidOperationException("El vale de salida está Anulado y no admite modificación.");

        if (!string.IsNullOrWhiteSpace(vale.AsientoContable))
            throw new InvalidOperationException("El vale de salida ya generó asiento contable. Imposible su modificación.");

        // La orden dueña del vale no debe estar cerrada.
        var estadoOrden = await _uow.Logistica.Transacciones.OrdenesServicio.Query()
            .AsNoTracking()
            .Where(x => x.PlantaCode == planta && x.ValeSalidaCode == code)
            .Select(x => (EstadoOrdenServicio?)x.Estado)
            .FirstOrDefaultAsync(ct);

        if (estadoOrden == EstadoOrdenServicio.Cerrado)
            throw new InvalidOperationException("La Orden de Servicio está Cerrada; no se puede cambiar el responsable.");

        if (!await _uow.Rrhh.Maestros.Trabajadores.Query().AsNoTracking().AnyAsync(t => t.Code == trabajador, ct))
            throw new InvalidOperationException("El trabajador responsable no existe.");

        vale.TrabajadorCode = trabajador;

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "El vale de salida fue modificado por otro usuario. Recargue los datos e intente nuevamente.");
        }

        return await OrdenServicioSalidaReader.GetAsync(_uow, planta, code, ct);
    }
}
