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

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.CreateGuiaRemisionVenta;

public sealed class CreateGuiaRemisionVentaCommandHandler
    : IRequestHandler<CreateGuiaRemisionVentaCommand, GuiaRemisionVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateGuiaRemisionVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<GuiaRemisionVentaResponse> Handle(CreateGuiaRemisionVentaCommand request, CancellationToken ct)
    {
        var motive = GuiaRemisionVentaRules.ParseMotive(request.Motive);
        var today = DateTime.Today;
        var hasOrder = !string.IsNullOrWhiteSpace(request.OrdenTrabajoCode);

        if (motive == MotivoGuiaRemisionVenta.Devolucion && !hasOrder)
        {
            throw new InvalidOperationException("La devolución debe indicar la Orden de Trabajo.");
        }

        if (motive == MotivoGuiaRemisionVenta.Traslado && hasOrder)
        {
            throw new InvalidOperationException("El traslado no lleva Orden de Trabajo.");
        }

        if (motive == MotivoGuiaRemisionVenta.Reposicion)
        {
            if (string.IsNullOrWhiteSpace(request.ReplacesNumber)
                || !OrdenTrabajoVentaDocumentos.IsValid(GuiaRemisionVentaRules.NormalizeNumber(request.ReplacesNumber)))
            {
                throw new InvalidOperationException("La reposición debe indicar el número (SSS-NNNNNN) de la guía que se repone.");
            }
        }

        if (motive == MotivoGuiaRemisionVenta.Devolucion)
        {
            if (string.IsNullOrWhiteSpace(request.DevolutionMotiveCode))
            {
                throw new InvalidOperationException("La devolución debe indicar el motivo de devolución.");
            }

            if (!GuiaRemisionVentaRules.IsTwoDigits(request.DevolutionActionCode))
            {
                throw new InvalidOperationException("La devolución debe indicar la acción (2 dígitos; 01 devuelve al saldo de la OT).");
            }
        }

        await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, today, ct);

        var g = new GuiaRemisionVenta
        {
            Motive = motive,
            Status = EstadoGuiaRemisionVenta.Pendiente,
            IssueDate = today
        };

        OrdenTrabajoVenta? ot = null;
        GuiaProgramacionInfo? prog = null;
        string plantCode;
        Cepheus.Domain.Facturacion.Maestros.Obra obra;

        if (hasOrder)
        {
            var otCode = ProgramacionOrdenTrabajo.NormalizeCode(request.OrdenTrabajoCode!);

            if (motive == MotivoGuiaRemisionVenta.Devolucion)
            {
                ot = await GuiaRemisionVentaRules.LoadOrderAsync(_uow, otCode, ct);
                GuiaRemisionVentaRules.EnsureReturnable(ot);

                if (string.IsNullOrWhiteSpace(request.PlantCode))
                {
                    throw new InvalidOperationException("Indique la planta de la devolución.");
                }

                plantCode = OrdenTrabajoVentaRules.NormalizeFixed(request.PlantCode, 2);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.ProgramacionType) || string.IsNullOrWhiteSpace(request.ProgramacionCode))
                {
                    throw new InvalidOperationException("Indique el tipo y el código de la programación.");
                }

                ot = await ProgramacionOrdenTrabajo.LoadProgramableAsync(_uow, otCode, ct);

                prog = await GuiaRemisionVentaProgramaciones.LoadAsync(
                    _uow, GuiaRemisionVentaRules.ParseProgramacionType(request.ProgramacionType), request.ProgramacionCode, ct);

                if (prog.OrdenTrabajoCode != ot.Code)
                {
                    throw new InvalidOperationException($"La programación {prog.Code} no es de la Orden de Trabajo {ot.Code}.");
                }

                if (prog.Status != EstadoProgramacion.Pendiente)
                {
                    throw new InvalidOperationException($"La programación {prog.Code} está {prog.Status}; no admite guías.");
                }

                g.ProgramacionType = prog.Type;
                g.ProgramacionCode = prog.Code;
                plantCode = prog.PlantCode;
            }

            g.OrdenTrabajoCode = ot.Code;
            g.ClienteCode = ot.ClienteCode;
            g.ObraCode = ot.ObraCode;
            g.VendedorCode = ot.VendedorCode;
            g.CurrencyCode = ot.CurrencyCode;
            obra = await GuiaRemisionVentaRules.LoadObraAsync(_uow, ot.ClienteCode, ot.ObraCode, ct);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(request.ProgramacionCode))
            {
                throw new InvalidOperationException("Una guía sin Orden de Trabajo no lleva programación.");
            }

            if (string.IsNullOrWhiteSpace(request.PlantCode)
                || string.IsNullOrWhiteSpace(request.ClienteCode)
                || string.IsNullOrWhiteSpace(request.ObraCode))
            {
                throw new InvalidOperationException("Sin Orden de Trabajo indique planta, cliente y obra.");
            }

            plantCode = OrdenTrabajoVentaRules.NormalizeFixed(request.PlantCode, 2);
            obra = await GuiaRemisionVentaRules.LoadObraAsync(
                _uow,
                OrdenTrabajoVentaRules.NormalizeFixed(request.ClienteCode, 5),
                OrdenTrabajoVentaRules.NormalizeFixed(request.ObraCode, 3),
                ct);

            g.ClienteCode = obra.ClienteCode;
            g.ObraCode = obra.Code;
            g.VendedorCode = string.IsNullOrWhiteSpace(request.VendedorCode)
                ? obra.VendedorCode
                : OrdenTrabajoVentaRules.NormalizeFixed(request.VendedorCode, 4);
            g.CurrencyCode = GuiaRemisionVentaRules.LocalCurrency;
        }

        g.PlantCode = plantCode;
        g.ExchangeRate = await GuiaRemisionVentaRules.GetExchangeRateAsync(_uow, g.CurrencyCode, today, ct);

        if (motive == MotivoGuiaRemisionVenta.Reposicion)
        {
            var replaces = GuiaRemisionVentaRules.NormalizeNumber(request.ReplacesNumber!);

            var original = await _uow.Facturacion.Transacciones.GuiasRemisionVenta.Query().AsNoTracking()
                .Where(x => x.Number == replaces)
                .Select(x => new { x.ClienteCode, x.Status })
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException($"La guía {replaces} que se repone no existe.");

            if (original.Status == EstadoGuiaRemisionVenta.Anulado || original.ClienteCode != g.ClienteCode)
            {
                throw new InvalidOperationException($"La guía {replaces} no se puede reponer (anulada o de otro cliente).");
            }

            g.ReplacesNumber = replaces;
        }

        if (motive == MotivoGuiaRemisionVenta.Devolucion)
        {
            var mot = OrdenTrabajoVentaRules.NormalizeFixed(request.DevolutionMotiveCode!, 2);

            if (!await _uow.Comunes.MotivosDevolucion.Query().AsNoTracking().AnyAsync(m => m.Code == mot && m.IsActive, ct))
            {
                throw new KeyNotFoundException($"El motivo de devolución {mot} no existe o está inactivo.");
            }

            g.DevolutionMotiveCode = mot;
            g.DevolutionActionCode = request.DevolutionActionCode!.Trim();
        }

        var planta = await GuiaRemisionVentaRules.LoadPlantaAsync(_uow, plantCode, tracked: true, ct);
        g.Number = await GuiaRemisionVentaRules.TakeNumberAsync(_uow, planta, request.Number, ct);

        await GuiaRemisionVentaRules.ApplyFieldsAsync(_uow, g, request, obra, ct);
        GuiaRemisionVentaRules.ApplyDocument(g, ot);

        var lines = ot is not null
            ? GuiaRemisionVentaRules.BuildOrderLines(g, ot, prog, request.Detalles, new Dictionary<int, decimal>())
            : await GuiaRemisionVentaRules.BuildFreeLinesAsync(_uow, g, request.Detalles, ct);

        foreach (var l in lines)
        {
            g.Detalles.Add(l);
        }

        if (ot is not null)
        {
            await GuiaRemisionVentaSaldos.RecalcAsync(_uow, ot, g, ct);
        }

        await _uow.Facturacion.Transacciones.GuiasRemisionVenta.AddAsync(g, ct);
        await GuiaRemisionVentaRules.SaveAsync(_uow, "la guía de remisión", ct);

        return await GuiaRemisionVentaReader.GetAsync(_uow, g.Number, ct);
    }
}
