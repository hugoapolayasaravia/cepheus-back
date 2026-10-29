using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.GetTipoValeByCode
{
    public class GetTipoValeByCodeQueryHandler : IRequestHandler<GetTipoValeByCodeQuery, TipoValeResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoValeByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoValeResponse> Handle(GetTipoValeByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoVale = await _uow.Logistica.Catalogos.TiposVale.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoValeResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy,
                    UpdatedBy = t.UpdatedBy,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoVale is null)
            {
                throw new KeyNotFoundException($"Tipo de vale {request.Code} no encontrado.");
            }

            return tipoVale;
        }
    }
}