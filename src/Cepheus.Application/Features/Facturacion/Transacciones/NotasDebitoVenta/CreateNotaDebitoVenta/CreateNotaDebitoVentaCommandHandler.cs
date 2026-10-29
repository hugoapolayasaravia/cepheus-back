using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.CreateNotaDebitoVenta
{
    public class CreateNotaDebitoVentaCommandHandler
        : IRequestHandler<CreateNotaDebitoVentaCommand, NotaDebitoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNotaDebitoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaDebitoVentaResponse> Handle(
            CreateNotaDebitoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaDebitoVentaRules.Normalize(request.PlantaCode);
            var clienteCode = request.ClienteCode.Trim();
            var obraCode = request.ObraCode.Trim();
            var monedaCode = NotaDebitoVentaRules.Normalize(request.MonedaCode);

            var obra = await NotaDebitoVentaRules.GetObraAsync(_uow, clienteCode, obraCode, cancellationToken);

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

            var (documentType, documentNumber) = await NotaDebitoVentaRules.ResolveReferenceAsync(
                _uow, clienteCode, request.DocumentTypeCode, request.DocumentNumber, cancellationToken);

            var exchangeRate = await DocumentoVentaRules.ResolveExchangeRateAsync(
                _uow, monedaCode, request.IssueDate, cancellationToken);
            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, request.IssueDate, cancellationToken);
            var productNames = await NotaDebitoVentaRules.LoadProductNamesAsync(_uow, request.Details, cancellationToken);

            var created = await NotaDebitoVentaRules.SaveNewAsync(_uow, plantaCode, number =>
            {
                var (details, subTotal) = NotaDebitoVentaRules.BuildDetails(plantaCode, number, request.Details, productNames);
                var totals = DocumentoVentaRules.ComputeTotals(subTotal, igvPercentage);

                var nota = new NotaDebitoVenta
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
                        : NotaDebitoVentaRules.Normalize(request.ExecutingPlantaCode),
                    SubTotal = totals.SubTotal,
                    IgvAmount = totals.IgvAmount,
                    TotalAmount = totals.TotalAmount,
                    PendingAmount = totals.TotalAmount,
                    Type = "N",
                    DocumentTypeCode = documentType,
                    DocumentNumber = documentNumber,
                    Status = EstadoNotaDebito.Pendiente,
                    IsPrinted = false,
                    Details = details
                };

                return Task.FromResult(nota);
            }, cancellationToken);

            _uow.ClearTracking();

            return (await NotaDebitoVentaMapper.LoadAsync(_uow, created.PlantaCode, created.Number, cancellationToken))!;
        }
    }
}
