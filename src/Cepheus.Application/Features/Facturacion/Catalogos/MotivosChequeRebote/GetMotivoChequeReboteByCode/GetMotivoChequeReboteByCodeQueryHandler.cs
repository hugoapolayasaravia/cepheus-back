using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.GetMotivoChequeReboteByCode
{
    public class GetMotivoChequeReboteByCodeQueryHandler
        : IRequestHandler<GetMotivoChequeReboteByCodeQuery, MotivoChequeReboteResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivoChequeReboteByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoChequeReboteResponse> Handle(
            GetMotivoChequeReboteByCodeQuery request, CancellationToken cancellationToken)
        {
            var motivo = await MotivoChequeReboteProjection.LoadAsync(_uow, request.Code.Trim(), cancellationToken);

            return motivo ?? throw new KeyNotFoundException($"Motivo de rebote de cheque {request.Code} no encontrado.");
        }
    }
}
