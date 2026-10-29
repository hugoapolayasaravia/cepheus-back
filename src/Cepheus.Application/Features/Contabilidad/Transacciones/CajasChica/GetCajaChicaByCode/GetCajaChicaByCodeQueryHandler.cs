using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.GetCajaChicaByCode
{
    public class GetCajaChicaByCodeQueryHandler : IRequestHandler<GetCajaChicaByCodeQuery, CajaChicaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCajaChicaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(GetCajaChicaByCodeQuery request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var caja = await CajaChicaMapper.LoadAsync(_uow, code, cancellationToken);

            if (caja is null)
            {
                throw new KeyNotFoundException($"Caja chica {code} no encontrada.");
            }

            return caja;
        }
    }
}
