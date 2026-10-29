using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.CreateNotaCreditoVenta
{
    public class CreateNotaCreditoVentaCommandHandler
        : IRequestHandler<CreateNotaCreditoVentaCommand, NotaCreditoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNotaCreditoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCreditoVentaResponse> Handle(
            CreateNotaCreditoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaCreditoVentaRules.Normalize(request.PlantaCode);
            var clienteCode = request.ClienteCode.Trim();
            var obraCode = request.ObraCode.Trim();
            var monedaCode = NotaCreditoVentaRules.Normalize(request.MonedaCode);

            var obra = await NotaCreditoVentaRules.GetObraAsync(_uow, clienteCode, obraCode, cancellationToken);

            var deliveryAddress = string.IsNullOrWhiteSpace(request.DeliveryAddress)
                ? (string.IsNullOrWhiteSpace(obra.DeliveryAddress) ? obra.Address : obra.DeliveryAddress)
                : request.DeliveryAddress.Trim();

            var deliveryUbigeo = string.IsNullOrWhiteSpace(request.DeliveryUbigeoCode)
                ? obra.DeliveryUbigeoCode
                : request.DeliveryUbigeoCode.Trim();

            var formaPago = string.IsNullOrWhiteSpace(request.FormaPagoVentaCode)
                ? obra.FormaPagoVentaCode
                : request.FormaPagoVentaCode.Trim();

            if (string.IsNullOrWhiteSpace(formaPago))
            {
                throw new InvalidOperationException(
                    "Debe indicar la forma de pago: la obra no tiene una forma de pago por defecto.");
            }

            var exchangeRate = await DocumentoVentaRules.ResolveExchangeRateAsync(
                _uow, monedaCode, request.IssueDate, cancellationToken);
            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, request.IssueDate, cancellationToken);
            var productNames = await NotaCreditoVentaRules.LoadProductNamesAsync(_uow, request.Details, cancellationToken);

            var created = await NotaCreditoVentaRules.SaveNewAsync(_uow, plantaCode, number =>
            {
                var (details, subTotal) = NotaCreditoVentaRules.BuildDetails(plantaCode, number, request.Details, productNames);
                var totals = DocumentoVentaRules.ComputeTotals(subTotal, igvPercentage);

                var nota = new NotaCreditoVenta
                {
                    PlantaCode = plantaCode,
                    Number = number,
                    IssueDate = request.IssueDate,
                    ClienteCode = clienteCode,
                    ObraCode = obraCode,
                    VendedorCode = string.IsNullOrWhiteSpace(request.VendedorCode) ? null : request.VendedorCode.Trim(),
                    DeliveryAddress = deliveryAddress,
                    DeliveryUbigeoCode = deliveryUbigeo,
                    FormaPagoVentaCode = formaPago,
                    MonedaCode = monedaCode,
                    ExchangeRate = exchangeRate,
                    OrderCode = string.IsNullOrWhiteSpace(request.OrderCode) ? null : request.OrderCode.Trim(),
                    Description = request.Description.Trim(),
                    ExecutingPlantaCode = string.IsNullOrWhiteSpace(request.ExecutingPlantaCode)
                        ? plantaCode
                        : NotaCreditoVentaRules.Normalize(request.ExecutingPlantaCode),
                    SubTotal = totals.SubTotal,
                    IgvAmount = totals.IgvAmount,
                    TotalAmount = totals.TotalAmount,
                    PendingAmount = totals.TotalAmount,
                    Type = string.IsNullOrWhiteSpace(request.Type) ? "N" : request.Type.Trim().ToUpperInvariant(),
                    DocumentTypeCode = request.DocumentTypeCode.Trim(),
                    DocumentNumber = request.DocumentNumber.Trim(),
                    MotivoDevolucionCode = request.MotivoDevolucionCode.Trim(),
                    CashDocument = string.IsNullOrWhiteSpace(request.CashDocument) ? null : request.CashDocument.Trim(),
                    Status = EstadoNotaCredito.Pendiente,
                    IsPrinted = false,
                    Details = details
                };

                return Task.FromResult(nota);
            }, cancellationToken);

            _uow.ClearTracking();

            return (await NotaCreditoVentaMapper.LoadAsync(_uow, created.PlantaCode, created.Number, cancellationToken))!;
        }
    }
}
