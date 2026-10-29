using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.GetNotaDebitoVentaByKey
{
    public class GetNotaDebitoVentaByKeyQueryHandler
        : IRequestHandler<GetNotaDebitoVentaByKeyQuery, NotaDebitoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNotaDebitoVentaByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaDebitoVentaResponse> Handle(
            GetNotaDebitoVentaByKeyQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaDebitoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var nota = await NotaDebitoVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException(
                    $"Nota de débito {NotaDebitoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
            }

            return nota;
        }
    }
}
