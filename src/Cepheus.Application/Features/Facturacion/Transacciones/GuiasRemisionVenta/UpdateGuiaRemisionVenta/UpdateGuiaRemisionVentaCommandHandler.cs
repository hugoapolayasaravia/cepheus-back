using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.UpdateGuiaRemisionVenta;

public sealed class UpdateGuiaRemisionVentaCommandHandler
    : IRequestHandler<UpdateGuiaRemisionVentaCommand, GuiaRemisionVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateGuiaRemisionVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<GuiaRemisionVentaResponse> Handle(UpdateGuiaRemisionVentaCommand request, CancellationToken ct)
    {
        var number = GuiaRemisionVentaRules.NormalizeNumber(request.Number);
        var g = await GuiaRemisionVentaReader.LoadForUpdateAsync(_uow, number, ct);

        if (g.Status != EstadoGuiaRemisionVenta.Pendiente)
        {
            throw new InvalidOperationException($"La guía {number} está {g.Status}; solo se modifica una guía Pendiente.");
        }

        if (g.IsPrinted)
        {
            throw new InvalidOperationException($"La guía {number} ya fue impresa; no se puede modificar.");
        }

        await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, g.IssueDate, ct);

        var obra = await GuiaRemisionVentaRules.LoadObraAsync(_uow, g.ClienteCode, g.ObraCode, ct);
        await GuiaRemisionVentaRules.ApplyFieldsAsync(_uow, g, request.Data, obra, ct);

        OrdenTrabajoVenta? ot = null;
        List<GuiaRemisionVentaDetalle> lines;

        if (g.OrdenTrabajoCode is not null)
        {
            ot = await GuiaRemisionVentaRules.LoadOrderAsync(_uow, g.OrdenTrabajoCode, ct);

            GuiaProgramacionInfo? prog = null;

            if (g.ProgramacionType is { } pType && g.ProgramacionCode is { } pCode)
            {
                prog = await GuiaRemisionVentaProgramaciones.LoadAsync(_uow, pType, pCode, ct);
            }

            var ownOld = g.Detalles
                .Where(d => d.OrdenTrabajoItem != null)
                .GroupBy(d => d.OrdenTrabajoItem!.Value)
                .ToDictionary(x => x.Key, x => x.Sum(d => d.Quantity));

            lines = GuiaRemisionVentaRules.BuildOrderLines(g, ot, prog, request.Data.Detalles, ownOld);
        }
        else
        {
            lines = await GuiaRemisionVentaRules.BuildFreeLinesAsync(_uow, g, request.Data.Detalles, ct);
        }

        GuiaRemisionVentaRules.ReplaceLines(g, lines);

        if (ot is not null)
        {
            await GuiaRemisionVentaSaldos.RecalcAsync(_uow, ot, g, ct);
        }

        await GuiaRemisionVentaRules.SaveAsync(_uow, "la modificación de la guía", ct);

        return await GuiaRemisionVentaReader.GetAsync(_uow, number, ct);
    }
}
