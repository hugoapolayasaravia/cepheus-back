using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Negocios.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Negocios.GetNegocioByCode
{
    public class GetNegocioByCodeQueryHandler : IRequestHandler<GetNegocioByCodeQuery, NegocioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNegocioByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NegocioResponse> Handle(GetNegocioByCodeQuery request, CancellationToken cancellationToken)
        {
            var negocio = await _uow.Comunes.Negocios.Query()
                .AsNoTracking()
                .Where(n => n.Code == request.Code)
                .Select(n => new NegocioResponse
                {
                    Code = n.Code,
                    Name = n.Name,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    CreatedBy = n.CreatedBy,
                    UpdatedBy = n.UpdatedBy,
                    RowVersion = n.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (negocio is null)
            {
                throw new KeyNotFoundException($"Negocio {request.Code} no encontrado.");
            }

            return negocio;
        }
    }
}
