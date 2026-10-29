using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.CreateCobrador;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.GetCobradorByCode
{
    public class GetCobradorByCodeQueryHandler : IRequestHandler<GetCobradorByCodeQuery, CobradorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCobradorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CobradorResponse> Handle(GetCobradorByCodeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Maestros.Cobradores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Cobrador {request.Code} no encontrado.");
            }

            return CreateCobradorCommandHandler.Map(entity);
        }
    }
}
