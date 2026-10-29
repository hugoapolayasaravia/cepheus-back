using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionByKey
{
    public class GetDetraccionByKeyQueryHandler : IRequestHandler<GetDetraccionByKeyQuery, DetraccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetDetraccionByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DetraccionResponse> Handle(GetDetraccionByKeyQuery request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);

            var response = await DetraccionMapper.LoadAsync(_uow, year, period, lot, cancellationToken);

            return response ?? throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
        }
    }
}
