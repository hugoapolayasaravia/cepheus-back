using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.GetFleteByCode
{
    public class GetFleteByCodeQueryHandler : IRequestHandler<GetFleteByCodeQuery, FleteResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetFleteByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FleteResponse> Handle(GetFleteByCodeQuery request, CancellationToken cancellationToken)
        {
            var flete = await _uow.Facturacion.Catalogos.Fletes.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new FleteResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    Amount = t.Amount,
                    IsDefault = t.IsDefault,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (flete is null)
            {
                throw new KeyNotFoundException($"Flete {request.Code} no encontrado.");
            }

            return flete;
        }
    }
}
