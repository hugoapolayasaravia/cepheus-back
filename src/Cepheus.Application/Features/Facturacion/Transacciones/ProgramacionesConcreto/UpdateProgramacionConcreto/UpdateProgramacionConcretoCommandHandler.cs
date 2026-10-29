using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.UpdateProgramacionConcreto;

public sealed class UpdateProgramacionConcretoCommandHandler
    : IRequestHandler<UpdateProgramacionConcretoCommand, ProgramacionConcretoResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateProgramacionConcretoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProgramacionConcretoResponse> Handle(UpdateProgramacionConcretoCommand request, CancellationToken ct)
    {
        var code = ProgramacionConcretoRules.NormalizeCode(request.Code);
        var d = request.Data;

        var p = await ProgramacionConcretoReader.LoadForUpdateAsync(_uow, code, ct);
        await ProgramacionConcretoRules.EnsureModifiableAsync(_uow, p, "modificar", ct);

        var ot = await ProgramacionOrdenTrabajo.LoadProgramableAsync(_uow, p.OrdenTrabajoCode, ct);

        await ProgramacionConcretoRules.ApplyHeaderAsync(_uow, p, d, ct);

        var requested = d.Detalles.ToDictionary(x => x.Item);

        foreach (var existing in p.Detalles.Where(x => !requested.ContainsKey(x.Item)).ToList())
        {
            p.Detalles.Remove(existing);
        }

        foreach (var r in d.Detalles)
        {
            var line = p.Detalles.FirstOrDefault(x => x.Item == r.Item);

            if (line is null)
            {
                p.Detalles.Add(ProgramacionConcretoRules.NewLine(p, r));
                continue;
            }

            line.Quantity = r.Quantity;
            line.PumpCode = ProgramacionConcretoRules.NormalizePump(r.PumpCode);
            line.Support = r.Support;
        }

        var others = await ProgramacionConcretoRules.OtherProgrammedAsync(_uow, ot.Code, p.Code, ct);

        ProgramacionConcretoRules.Validate(ot, p, others);
        ProgramacionConcretoRules.Recalc(ot, p, others);

        await ProgramacionOrdenTrabajo.SaveAsync(_uow, "la programación de Concreto", ct);

        return await ProgramacionConcretoReader.GetAsync(_uow, code, ct);
    }
}
