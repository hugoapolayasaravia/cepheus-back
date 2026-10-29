using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GetNotaCreditoVentaByKey
{
    public class GetNotaCreditoVentaByKeyQueryHandler
        : IRequestHandler<GetNotaCreditoVentaByKeyQuery, NotaCreditoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNotaCreditoVentaByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCreditoVentaResponse> Handle(
            GetNotaCreditoVentaByKeyQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaCreditoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var nota = await NotaCreditoVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException(
                    $"Nota de crédito {NotaCreditoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
            }

            return nota;
        }
    }
}
