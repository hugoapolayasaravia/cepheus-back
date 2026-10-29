using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.CreateProgramacionOtros;

public sealed class CreateProgramacionOtrosCommandHandler
    : IRequestHandler<CreateProgramacionOtrosCommand, ProgramacionOtrosResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateProgramacionOtrosCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProgramacionOtrosResponse> Handle(CreateProgramacionOtrosCommand request, CancellationToken ct)
    {
        var otCode = ProgramacionOrdenTrabajo.NormalizeCode(request.OrdenTrabajoCode);
        var ot = await ProgramacionOrdenTrabajo.LoadProgramableAsync(_uow, otCode, ct);

        var p = new ProgramacionOtros
        {
            Code = await ProgramacionOtrosRules.NextCodeAsync(_uow, ct),
            OrdenTrabajoCode = ot.Code,
            Status = EstadoProgramacion.Pendiente
        };

        await ProgramacionOtrosRules.ApplyHeaderAsync(_uow, p, request, ct);

        foreach (var r in request.Detalles)
        {
            p.Detalles.Add(ProgramacionOtrosRules.NewLine(p, r));
        }

        var others = await ProgramacionOtrosRules.OtherProgrammedAsync(_uow, ot.Code, p.Code, ct);

        ProgramacionOtrosRules.Validate(ot, p, others);
        ProgramacionOtrosRules.Recalc(ot, p, others);

        await _uow.Facturacion.Transacciones.ProgramacionesOtros.AddAsync(p, ct);
        await ProgramacionOrdenTrabajo.SaveAsync(_uow, "la programación de Otros", ct);

        return await ProgramacionOtrosReader.GetAsync(_uow, p.Code, ct);
    }
}
