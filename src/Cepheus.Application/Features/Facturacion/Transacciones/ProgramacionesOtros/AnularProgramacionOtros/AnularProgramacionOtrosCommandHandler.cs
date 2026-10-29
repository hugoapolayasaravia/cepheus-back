using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.AnularProgramacionOtros;

public sealed class AnularProgramacionOtrosCommandHandler
    : IRequestHandler<AnularProgramacionOtrosCommand, ProgramacionOtrosResponse>
{
    private readonly IUnitOfWork _uow;

    public AnularProgramacionOtrosCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProgramacionOtrosResponse> Handle(AnularProgramacionOtrosCommand request, CancellationToken ct)
    {
        var code = ProgramacionOtrosRules.NormalizeCode(request.Code);

        var p = await ProgramacionOtrosReader.LoadForUpdateAsync(_uow, code, ct);
        await ProgramacionOtrosRules.EnsureModifiableAsync(_uow, p, "anular", ct);

        var ot = await ProgramacionOtrosReader.LoadOrderAsync(_uow, p.OrdenTrabajoCode, ct);

        p.Status = EstadoProgramacion.Anulada;

        var others = await ProgramacionOtrosRules.OtherProgrammedAsync(_uow, ot.Code, p.Code, ct);
        ProgramacionOtrosRules.Recalc(ot, p, others);

        await ProgramacionOrdenTrabajo.SaveAsync(_uow, "la anulación de la programación", ct);

        return await ProgramacionOtrosReader.GetAsync(_uow, code, ct);
    }
}
