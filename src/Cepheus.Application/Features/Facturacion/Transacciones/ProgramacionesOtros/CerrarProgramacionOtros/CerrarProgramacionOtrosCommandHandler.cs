using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CerrarProgramacionOtros;

public sealed class CerrarProgramacionOtrosCommandHandler
    : IRequestHandler<CerrarProgramacionOtrosCommand, ProgramacionOtrosResponse>
{
    private readonly IUnitOfWork _uow;

    public CerrarProgramacionOtrosCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProgramacionOtrosResponse> Handle(CerrarProgramacionOtrosCommand request, CancellationToken ct)
    {
        var code = ProgramacionOtrosRules.NormalizeCode(request.Code);

        var p = await ProgramacionOtrosReader.LoadForUpdateAsync(_uow, code, ct);

        if (p.Status != EstadoProgramacion.Pendiente)
        {
            throw new InvalidOperationException($"La programación {p.Code} está {p.Status}; solo se cierra una Pendiente.");
        }

        var ot = await ProgramacionOtrosReader.LoadOrderAsync(_uow, p.OrdenTrabajoCode, ct);

        foreach (var line in p.Detalles)
        {
            line.Quantity = line.DeliveredQuantity;
        }

        p.Status = EstadoProgramacion.Cerrada;

        var others = await ProgramacionOtrosRules.OtherProgrammedAsync(_uow, ot.Code, p.Code, ct);
        ProgramacionOtrosRules.Recalc(ot, p, others);

        await ProgramacionOrdenTrabajo.SaveAsync(_uow, "el cierre de la programación", ct);

        return await ProgramacionOtrosReader.GetAsync(_uow, code, ct);
    }
}
