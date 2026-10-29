using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.CreateTipoBien;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.GetTipoBienById
{
    public class GetTipoBienByIdQueryHandler : IRequestHandler<GetTipoBienByIdQuery, TipoBienResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoBienByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoBienResponse> Handle(GetTipoBienByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.TiposBien.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de bien {request.Code} no encontrado.");
            }

            return CreateTipoBienCommandHandler.Map(entity);
        }
    }
}
