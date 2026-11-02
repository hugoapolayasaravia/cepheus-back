using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaByKey
{
    public class GetLetraVentaByKeyQueryHandler : IRequestHandler<GetLetraVentaByKeyQuery, LetraVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetLetraVentaByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LetraVentaResponse> Handle(GetLetraVentaByKeyQuery request, CancellationToken cancellationToken)
        {
            var planta = LetraVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            return await LetraVentaMapper.LoadAsync(_uow, planta, number, cancellationToken)
                   ?? throw new KeyNotFoundException($"Letra {LetraVentaRules.FormatKey(planta, number)} no encontrada.");
        }
    }
}
