using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.UpdateProgramacionOtros;

public sealed class UpdateProgramacionOtrosCommandHandler
    : IRequestHandler<UpdateProgramacionOtrosCommand, ProgramacionOtrosResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateProgramacionOtrosCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProgramacionOtrosResponse> Handle(UpdateProgramacionOtrosCommand request, CancellationToken ct)
    {
        var code = ProgramacionOtrosRules.NormalizeCode(request.Code);
        var d = request.Data;

        var p = await ProgramacionOtrosReader.LoadForUpdateAsync(_uow, code, ct);
        await ProgramacionOtrosRules.EnsureModifiableAsync(_uow, p, "modificar", ct);

        var ot = await ProgramacionOrdenTrabajo.LoadProgramableAsync(_uow, p.OrdenTrabajoCode, ct);

        await ProgramacionOtrosRules.ApplyHeaderAsync(_uow, p, d, ct);

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
                p.Detalles.Add(ProgramacionOtrosRules.NewLine(p, r));
                continue;
            }

            line.Quantity = r.Quantity;
        }

        var others = await ProgramacionOtrosRules.OtherProgrammedAsync(_uow, ot.Code, p.Code, ct);

        ProgramacionOtrosRules.Validate(ot, p, others);
        ProgramacionOtrosRules.Recalc(ot, p, others);

        await ProgramacionOrdenTrabajo.SaveAsync(_uow, "la programación de Otros", ct);

        return await ProgramacionOtrosReader.GetAsync(_uow, code, ct);
    }
}
