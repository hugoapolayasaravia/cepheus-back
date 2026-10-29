using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.GetFallaByCode
{
    public class GetFallaByCodeQueryHandler
        : IRequestHandler<GetFallaByCodeQuery, FallaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetFallaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FallaResponse> Handle(
            GetFallaByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var falla = await _uow.Mantenimiento.Catalogos.Fallas.Query()
                .AsNoTracking()
                .Where(p => p.Code == request.Code)
                .Select(p => new FallaResponse
                {
                    Code = p.Code,
                    Name = p.Name,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CreatedBy = p.CreatedBy,
                    UpdatedBy = p.UpdatedBy,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (falla is null)
            {
                throw new KeyNotFoundException($"Falla {request.Code} no encontrada.");
            }

            return falla;
        }
    }
}
