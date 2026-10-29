using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.CreateProgramacionConcreto;

public sealed class CreateProgramacionConcretoCommandHandler
    : IRequestHandler<CreateProgramacionConcretoCommand, ProgramacionConcretoResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateProgramacionConcretoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProgramacionConcretoResponse> Handle(CreateProgramacionConcretoCommand request, CancellationToken ct)
    {
        var otCode = ProgramacionOrdenTrabajo.NormalizeCode(request.OrdenTrabajoCode);
        var ot = await ProgramacionOrdenTrabajo.LoadProgramableAsync(_uow, otCode, ct);

        var p = new ProgramacionConcreto
        {
            Code = await ProgramacionConcretoRules.NextCodeAsync(_uow, ct),
            OrdenTrabajoCode = ot.Code,
            Status = EstadoProgramacion.Pendiente
        };

        await ProgramacionConcretoRules.ApplyHeaderAsync(_uow, p, request, ct);

        foreach (var r in request.Detalles)
        {
            p.Detalles.Add(ProgramacionConcretoRules.NewLine(p, r));
        }

        var others = await ProgramacionConcretoRules.OtherProgrammedAsync(_uow, ot.Code, p.Code, ct);

        ProgramacionConcretoRules.Validate(ot, p, others);
        ProgramacionConcretoRules.Recalc(ot, p, others);

        await _uow.Facturacion.Transacciones.ProgramacionesConcreto.AddAsync(p, ct);
        await ProgramacionOrdenTrabajo.SaveAsync(_uow, "la programación de Concreto", ct);

        return await ProgramacionConcretoReader.GetAsync(_uow, p.Code, ct);
    }
}
