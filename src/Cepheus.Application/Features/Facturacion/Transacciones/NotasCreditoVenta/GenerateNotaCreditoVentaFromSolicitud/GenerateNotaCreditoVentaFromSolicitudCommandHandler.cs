using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GenerateNotaCreditoVentaFromSolicitud
{
    public class GenerateNotaCreditoVentaFromSolicitudCommandHandler
        : IRequestHandler<GenerateNotaCreditoVentaFromSolicitudCommand, NotaCreditoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerateNotaCreditoVentaFromSolicitudCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCreditoVentaResponse> Handle(
            GenerateNotaCreditoVentaFromSolicitudCommand request, CancellationToken cancellationToken)
        {
            var year = request.SolicitudYear.Trim();
            var month = request.SolicitudMonth.Trim();
            var solicitudCode = SolicitudNotaCreditoRules.Normalize(request.SolicitudCode);
            var plantaCode = NotaCreditoVentaRules.Normalize(request.PlantaCode);
            var obraCode = request.ObraCode.Trim();

            // Datos previos (sin seguimiento) para resolver tipo de cambio e IGV de la fecha de emisión.
            var monedaCode = await _uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                .AsNoTracking()
                .Where(s => s.Code == solicitudCode && s.Year == year && s.Month == month)
                .Select(s => s.MonedaCode)
                .FirstOrDefaultAsync(cancellationToken);

            if (monedaCode is null)
            {
                throw new KeyNotFoundException(
                    $"Solicitud de nota de crédito {SolicitudNotaCreditoRules.FormatKey(year, month, solicitudCode)} no encontrada.");
            }

            var exchangeRate = await DocumentoVentaRules.ResolveExchangeRateAsync(
                _uow, monedaCode, request.IssueDate, cancellationToken);
            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, request.IssueDate, cancellationToken);

            var created = await NotaCreditoVentaRules.SaveNewAsync(_uow, plantaCode, async number =>
            {
                var solicitud = await _uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query()
                    .Include(s => s.Approvals)
                    .Include(s => s.Motivo)
                    .Include(s => s.Details).ThenInclude(d => d.Producto)
                    .FirstAsync(s => s.Code == solicitudCode && s.Year == year && s.Month == month, cancellationToken);

                if (solicitud.Status != EstadoSolicitudNotaCredito.Pendiente)
                {
                    throw new InvalidOperationException(
                        $"La solicitud está en estado {solicitud.Status}: solo se genera la nota de crédito de una solicitud Pendiente.");
                }

                var pending = solicitud.Approvals
                    .Where(a => a.Status != EstadoAprobacionSolicitud.Aprobado)
                    .OrderBy(a => a.Stage)
                    .FirstOrDefault();

                if (!solicitud.Approvals.Any() || pending is not null)
                {
                    throw new InvalidOperationException(
                        "La solicitud debe tener todas las etapas aprobadas" +
                        (pending is null ? "." : $" (pendiente: {pending.Stage})."));
                }

                if (string.IsNullOrWhiteSpace(solicitud.Motivo.MotivoDevolucionCode))
                {
                    throw new InvalidOperationException(
                        $"El motivo {solicitud.Motivo.Name} no tiene un motivo de devolución asociado (Catálogo de Motivos de Solicitud).");
                }

                var obra = await NotaCreditoVentaRules.GetObraAsync(_uow, solicitud.ClienteCode, obraCode, cancellationToken);

                var formaPago = string.IsNullOrWhiteSpace(request.FormaPagoVentaCode)
                    ? obra.FormaPagoVentaCode
                    : request.FormaPagoVentaCode.Trim();

                if (string.IsNullOrWhiteSpace(formaPago))
                {
                    throw new InvalidOperationException(
                        "Debe indicar la forma de pago: la obra no tiene una forma de pago por defecto.");
                }

                var details = solicitud.Details
                    .OrderBy(d => d.Line)
                    .Select((d, index) => new NotaCreditoVentaDetalle
                    {
                        PlantaCode = plantaCode,
                        Number = number,
                        Line = (index + 1).ToString().PadLeft(3, '0'),
                        GuideNumber = null,
                        TipoProductoCode = d.TipoProductoCode,
                        ProductoCode = d.ProductoCode,
                        Description = d.Producto.Name,
                        Quantity = DocumentoVentaRules.Round2(d.Quantity),
                        DispatchedQuantity = 0m,
                        UnitPrice = d.UnitPrice,
                        Total = d.Total,
                        LineKey = d.LineKey,
                        OrderCode = d.OrderCode
                    })
                    .ToList();

                var totals = DocumentoVentaRules.ComputeTotals(details.Sum(d => d.Total), igvPercentage);

                // La solicitud pasa a Facturado en la misma operación que graba la nota.
                solicitud.Status = EstadoSolicitudNotaCredito.Facturado;
                solicitud.IsEditable = false;

                return new NotaCreditoVenta
                {
                    PlantaCode = plantaCode,
                    Number = number,
                    IssueDate = request.IssueDate,
                    ClienteCode = solicitud.ClienteCode,
                    ObraCode = obraCode,
                    VendedorCode = string.IsNullOrWhiteSpace(request.VendedorCode) ? null : request.VendedorCode.Trim(),
                    DeliveryAddress = string.IsNullOrWhiteSpace(request.DeliveryAddress)
                        ? (string.IsNullOrWhiteSpace(obra.DeliveryAddress) ? obra.Address : obra.DeliveryAddress)
                        : request.DeliveryAddress.Trim(),
                    DeliveryUbigeoCode = string.IsNullOrWhiteSpace(request.DeliveryUbigeoCode)
                        ? obra.DeliveryUbigeoCode
                        : request.DeliveryUbigeoCode.Trim(),
                    FormaPagoVentaCode = formaPago,
                    MonedaCode = solicitud.MonedaCode,
                    ExchangeRate = exchangeRate,
                    Description = string.IsNullOrWhiteSpace(request.Description)
                        ? solicitud.Observation
                        : request.Description.Trim(),
                    ExecutingPlantaCode = string.IsNullOrWhiteSpace(request.ExecutingPlantaCode)
                        ? plantaCode
                        : NotaCreditoVentaRules.Normalize(request.ExecutingPlantaCode),
                    SubTotal = totals.SubTotal,
                    IgvAmount = totals.IgvAmount,
                    TotalAmount = totals.TotalAmount,
                    PendingAmount = totals.TotalAmount,
                    Type = string.IsNullOrWhiteSpace(request.Type) ? "N" : request.Type.Trim().ToUpperInvariant(),
                    DocumentTypeCode = solicitud.DocumentTypeCode,
                    DocumentNumber = solicitud.DocumentNumber,
                    MotivoDevolucionCode = solicitud.Motivo.MotivoDevolucionCode!,
                    SolicitudCode = solicitud.Code,
                    SolicitudYear = solicitud.Year,
                    SolicitudMonth = solicitud.Month,
                    CashDocument = string.IsNullOrWhiteSpace(request.CashDocument) ? null : request.CashDocument.Trim(),
                    Status = EstadoNotaCredito.Pendiente,
                    IsPrinted = false,
                    Details = details
                };
            }, cancellationToken);

            _uow.ClearTracking();

            return (await NotaCreditoVentaMapper.LoadAsync(_uow, created.PlantaCode, created.Number, cancellationToken))!;
        }
    }
}
