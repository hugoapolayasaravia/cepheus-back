using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.GetTipoPensionByCode
{
    public class GetTipoPensionByCodeQueryHandler : IRequestHandler<GetTipoPensionByCodeQuery, TipoPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoPensionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPensionResponse> Handle(GetTipoPensionByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoPension = await _uow.Rrhh.Catalogos.TiposPension.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoPensionResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoPension is null)
            {
                throw new KeyNotFoundException($"Tipo de pensión {request.Code} no encontrado.");
            }

            return tipoPension;
        }
    }
}