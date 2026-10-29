using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.AnularComprobanteVenta;

public enum ModoAnulacionComprobanteVenta
{
    /// <summary>PB 'S': se anula el comprobante y sus guías (las cantidades vuelven a estar disponibles en la OT).</summary>
    AnularGuias = 1,

    /// <summary>PB 'N': se anula el comprobante y se emite otro con las mismas guías y líneas (nuevo número y fecha).</summary>
    Reemplazar = 2
}

public sealed class AnularComprobanteVentaRequest
{
    public ModoAnulacionComprobanteVenta Mode { get; set; } = ModoAnulacionComprobanteVenta.AnularGuias;
    public string Reason { get; set; } = default!;

    /// <summary>Solo Reemplazar: número del nuevo comprobante (por defecto el siguiente de la planta).</summary>
    public string? NewNumber { get; set; }

    /// <summary>Solo Reemplazar: fecha del nuevo comprobante (por defecto hoy).</summary>
    public DateTime? NewIssueDate { get; set; }
}

public sealed record AnularComprobanteVentaCommand(string TypeCode, string Number, AnularComprobanteVentaRequest Data)
    : IRequest<ComprobanteVentaResponse>;

public sealed class AnularComprobanteVentaCommandValidator : AbstractValidator<AnularComprobanteVentaCommand>
{
    public AnularComprobanteVentaCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Data.Mode).IsInEnum();
        RuleFor(x => x.Data.Reason).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Data.NewNumber).MaximumLength(10);
    }
}

public sealed class AnularComprobanteVentaCommandHandler
    : IRequestHandler<AnularComprobanteVentaCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public AnularComprobanteVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ComprobanteVentaResponse> Handle(AnularComprobanteVentaCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var number = ComprobanteVentaRules.NormalizeNumber(request.Number);
        var d = request.Data;
        var c = await ComprobanteVentaRules.LoadForUpdateAsync(_uow, type, number, ct);

        if (c.Status is not (EstadoComprobanteVenta.Pendiente or EstadoComprobanteVenta.Facturado))
        {
            throw new InvalidOperationException(
                $"El comprobante {ComprobanteVentaRules.TypeName(type)} {number} está {c.Status}; no se anula.");
        }

        if (!string.IsNullOrWhiteSpace(c.ProvisionEntry))
        {
            throw new InvalidOperationException(
                $"El comprobante {number} tiene asiento de provisión ({c.ProvisionEntry}); Contabilidad debe extornarlo antes de anular.");
        }

        await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, c.IssueDate, ct);

        var reason = OrdenTrabajoVentaRules.NormalizeText(d.Reason)!;
        var guias = await ComprobanteVentaGuias.LoadAsync(_uow, c, ct);

        OrdenTrabajoVenta? ot = c.OrdenTrabajoCode is null
            ? null
            : await GuiaRemisionVentaRules.LoadOrderAsync(_uow, c.OrdenTrabajoCode, ct);

        c.Status = EstadoComprobanteVenta.Anulado;
        c.CanceledAt = DateTime.Now;
        c.CanceledBy = _currentUser.FullName;
        c.CancelReason = reason;

        string resultNumber = number;

        if (d.Mode == ModoAnulacionComprobanteVenta.AnularGuias)
        {
            foreach (var g in guias)
            {
                ComprobanteVentaGuias.Cancel(g, _currentUser.FullName, $"Anulación del comprobante {number}: {reason}");
            }

            await ComprobanteVentaGuias.RecalcOrderAsync(_uow, ot, guias, ct);
        }
        else
        {
            var date = (d.NewIssueDate ?? DateTime.Today).Date;
            ComprobanteVentaRules.EnsureIssueDate(date);
            await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, date, ct);

            var planta = await GuiaRemisionVentaRules.LoadPlantaAsync(_uow, c.PlantCode, tracked: true, ct);
            var newNumber = await ComprobanteVentaRules.TakeNumberAsync(_uow, planta, type, null, d.NewNumber, ct);

            var n = new ComprobanteVenta
            {
                TypeCode = type,
                Number = newNumber,
                PlantCode = c.PlantCode,
                ExecutingPlantCode = c.ExecutingPlantCode,
                IssueDate = date,
                ClienteCode = c.ClienteCode,
                ObraCode = c.ObraCode,
                VendedorCode = c.VendedorCode,
                Address = c.Address,
                UbigeoCode = c.UbigeoCode,
                PaymentFormCode = c.PaymentFormCode,
                CurrencyCode = c.CurrencyCode,
                ExchangeRate = await GuiaRemisionVentaRules.GetExchangeRateAsync(_uow, c.CurrencyCode, date, ct),
                IgvPercentage = await ComprobanteVentaRules.GetIgvPercentageAsync(_uow, date, ct),
                OrdenTrabajoCode = c.OrdenTrabajoCode,
                Origin = c.Origin,
                DocumentFlow = c.DocumentFlow,
                IsFreeTransfer = c.IsFreeTransfer,
                Glosa = c.Glosa,
                Status = EstadoComprobanteVenta.Pendiente
            };

            foreach (var l in c.Detalles.OrderBy(x => x.Line))
            {
                n.Detalles.Add(new ComprobanteVentaDetalle
                {
                    TypeCode = type,
                    Number = newNumber,
                    Line = l.Line,
                    GuiaNumber = l.GuiaNumber,
                    GuiaLine = l.GuiaLine,
                    OrdenTrabajoCode = l.OrdenTrabajoCode,
                    OrdenTrabajoItem = l.OrdenTrabajoItem,
                    ProductoTipoCode = l.ProductoTipoCode,
                    ProductoCode = l.ProductoCode,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice
                });
            }

            ComprobanteVentaRules.Recalc(n);

            foreach (var g in guias)
            {
                g.Document1Number = newNumber;
            }

            // Créditos aprobó el número anterior para la OT: el aprobado pasa a ser el nuevo.
            if (ot?.Aprobacion is { } a)
            {
                if (a.Document1Number == number)
                {
                    a.Document1Number = newNumber;
                }

                if (a.Document2Number == number)
                {
                    a.Document2Number = newNumber;
                }
            }

            c.ReplacedByNumber = newNumber;
            resultNumber = newNumber;

            await _uow.Facturacion.Transacciones.ComprobantesVenta.AddAsync(n, ct);
        }

        await ComprobanteVentaRules.SaveAsync(_uow, "la anulación del comprobante", ct);

        return await ComprobanteVentaReader.GetAsync(_uow, type, resultNumber, ct);
    }
}
