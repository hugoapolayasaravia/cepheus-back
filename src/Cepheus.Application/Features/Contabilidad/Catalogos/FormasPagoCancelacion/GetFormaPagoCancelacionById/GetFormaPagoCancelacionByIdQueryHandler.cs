using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.CreateFormaPagoCancelacion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.GetFormaPagoCancelacionById
{
    public class GetFormaPagoCancelacionByIdQueryHandler : IRequestHandler<GetFormaPagoCancelacionByIdQuery, FormaPagoCancelacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetFormaPagoCancelacionByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoCancelacionResponse> Handle(GetFormaPagoCancelacionByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Catalogos.FormasPagoCancelacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Forma de pago de cancelación {request.Code} no encontrada.");
            }

            return CreateFormaPagoCancelacionCommandHandler.Map(entity);
        }
    }
}
