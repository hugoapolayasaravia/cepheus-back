using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GetCobranzaDudosa
{
    public class GetCobranzaDudosaQueryHandler : IRequestHandler<GetCobranzaDudosaQuery, CobranzaDudosaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCobranzaDudosaQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<CobranzaDudosaResponse> Handle(GetCobranzaDudosaQuery request, CancellationToken cancellationToken)
            => CobranzaDudosaMapper.LoadAsync(
                _uow, CobranzaDudosaRules.NormalizeType(request.TypeCode), request.PlantaCode, request.Number,
                request.ClienteCode, request.BancoCode, cancellationToken);
    }
}
