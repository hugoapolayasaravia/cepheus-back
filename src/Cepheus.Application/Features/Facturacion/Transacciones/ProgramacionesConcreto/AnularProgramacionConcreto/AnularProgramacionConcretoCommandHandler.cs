using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.AnularProgramacionConcreto;

public sealed class AnularProgramacionConcretoCommandHandler
    : IRequestHandler<AnularProgramacionConcretoCommand, ProgramacionConcretoResponse>
{
    private readonly IUnitOfWork _uow;

    public AnularProgramacionConcretoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProgramacionConcretoResponse> Handle(AnularProgramacionConcretoCommand request, CancellationToken ct)
    {
        var code = ProgramacionConcretoRules.NormalizeCode(request.Code);

        var p = await ProgramacionConcretoReader.LoadForUpdateAsync(_uow, code, ct);
        await ProgramacionConcretoRules.EnsureModifiableAsync(_uow, p, "anular", ct);

        var ot = await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query()
            .Include(o => o.Detalles)
            .FirstOrDefaultAsync(o => o.Code == p.OrdenTrabajoCode, ct)
            ?? throw new KeyNotFoundException($"La Orden de Trabajo {p.OrdenTrabajoCode} no existe.");

        p.Status = EstadoProgramacion.Anulada;

        var others = await ProgramacionConcretoRules.OtherProgrammedAsync(_uow, ot.Code, p.Code, ct);
        ProgramacionConcretoRules.Recalc(ot, p, others);

        await ProgramacionOrdenTrabajo.SaveAsync(_uow, "la anulación de la programación", ct);

        return await ProgramacionConcretoReader.GetAsync(_uow, code, ct);
    }
}
