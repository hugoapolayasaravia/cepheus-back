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

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.GetGuiaProgramacion;

/// <summary>Cabecera por defecto y líneas con saldo para emitir una guía desde una programación.</summary>
public sealed record GetGuiaProgramacionQuery(string Type, string Code) : IRequest<GuiaProgramacionResponse>;

public sealed class GetGuiaProgramacionQueryHandler : IRequestHandler<GetGuiaProgramacionQuery, GuiaProgramacionResponse>
{
    private readonly IUnitOfWork _uow;

    public GetGuiaProgramacionQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<GuiaProgramacionResponse> Handle(GetGuiaProgramacionQuery request, CancellationToken ct)
    {
        var type = GuiaRemisionVentaRules.ParseProgramacionType(request.Type);
        var prog = await GuiaRemisionVentaProgramaciones.LoadAsync(_uow, type, request.Code, ct);

        var ot = await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
                     .Query().AsNoTracking()
                     .Include(o => o.Cliente)
                     .Include(o => o.Obra)
                     .Include(o => o.Aprobacion)
                     .Include(o => o.Detalles).ThenInclude(d => d.Producto)
                     .FirstOrDefaultAsync(o => o.Code == prog.OrdenTrabajoCode, ct)
                 ?? throw new KeyNotFoundException($"La Orden de Trabajo {prog.OrdenTrabajoCode} no existe.");

        var planta = await _uow.Comunes.Plantas.Query().AsNoTracking()
            .FirstOrDefaultAsync(p => p.Code == prog.PlantCode, ct);

        var a = ot.Aprobacion;
        var contado = a is { PaymentType: TipoPagoAprobacion.Contado };

        var lines = prog.Lines
            .OrderBy(l => l.Item)
            .Select(l =>
            {
                var o = ot.Detalles.First(d => d.Item == l.Item);

                return new GuiaProgramacionLineaResponse
                {
                    Item = l.Item,
                    ProductoTipoCode = o.ProductoTipoCode,
                    ProductoCode = o.ProductoCode,
                    ProductoName = o.Producto.Name,
                    UnitCode = o.Producto.UnitCode,
                    ProgrammedQuantity = l.Quantity,
                    DeliveredQuantity = l.Delivered,
                    PendingQuantity = Math.Max(0m, l.Quantity - l.Delivered),
                    ApprovedBalance = Math.Max(0m, o.ApprovedQuantity - o.DeliveredQuantity),
                    UnitPrice = o.UnitPrice
                };
            })
            .ToList();

        return new GuiaProgramacionResponse
        {
            ProgramacionType = prog.Type,
            ProgramacionCode = prog.Code,
            ProgramacionStatus = prog.Status,
            OrdenTrabajoCode = ot.Code,
            ClienteCode = ot.ClienteCode,
            ClienteName = ot.Cliente.Name,
            ObraCode = ot.ObraCode,
            ObraName = ot.Obra.Description,
            VendedorCode = ot.VendedorCode,
            PlantCode = prog.PlantCode,
            Address = string.IsNullOrWhiteSpace(ot.Obra.DeliveryAddress) ? ot.Obra.Address : ot.Obra.DeliveryAddress,
            UbigeoCode = string.IsNullOrWhiteSpace(ot.Obra.DeliveryUbigeoCode) ? ot.Obra.UbigeoCode : ot.Obra.DeliveryUbigeoCode,
            CurrencyCode = ot.CurrencyCode,
            ExchangeRate = await GuiaRemisionVentaRules.TryGetExchangeRateAsync(_uow, ot.CurrencyCode, DateTime.Today, ct),
            DocumentFlow = a?.DocumentFlow,
            DocumentTypeCode = a is null ? null : a.DocumentType == TipoDocumentoAprobacion.Factura
                ? GuiaRemisionVentaRules.DocFactura
                : GuiaRemisionVentaRules.DocBoleta,
            Document1Number = contado ? a!.Document1Number : null,
            Document2Number = contado ? a!.Document2Number : null,
            NextNumber = planta is null ? null : GuiaRemisionVentaRules.TryNext(planta.NumeroGve),
            Lineas = lines
        };
    }
}
