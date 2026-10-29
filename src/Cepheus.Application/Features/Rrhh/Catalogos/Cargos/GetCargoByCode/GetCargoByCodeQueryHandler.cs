using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.GetCargoByCode
{
    public class GetCargoByCodeQueryHandler : IRequestHandler<GetCargoByCodeQuery, CargoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCargoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CargoResponse> Handle(GetCargoByCodeQuery request, CancellationToken cancellationToken)
        {
            var cargo = await _uow.Rrhh.Catalogos.Cargos.Query()
                .AsNoTracking()
                .Where(c => c.Code == request.Code)
                .Select(c => new CargoResponse
                {
                    Code = c.Code,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (cargo is null)
            {
                throw new KeyNotFoundException($"Cargo {request.Code} no encontrado.");
            }

            return cargo;
        }
    }
}