using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.GetEquipoByCode
{
    public class GetEquipoByCodeQueryHandler : IRequestHandler<GetEquipoByCodeQuery, EquipoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetEquipoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EquipoResponse> Handle(GetEquipoByCodeQuery request, CancellationToken cancellationToken)
        {
            var equipo = await _uow.Mantenimiento.Maestros.Equipos.Query()
                .AsNoTracking()
                .Where(e => e.Code == request.Code)
                .Select(e => new EquipoResponse
                {
                    Code = e.Code,
                    Name = e.Name,
                    Nivel = e.Nivel,
                    SubCentroCostoCode = e.SubCentroCostoCode,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    RowVersion = e.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (equipo is null)
            {
                throw new KeyNotFoundException($"Equipo {request.Code} no encontrado.");
            }

            return equipo;
        }
    }
}
