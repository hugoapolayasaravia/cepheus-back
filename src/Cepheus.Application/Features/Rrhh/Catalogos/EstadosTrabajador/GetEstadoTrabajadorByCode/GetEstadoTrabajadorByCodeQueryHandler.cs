using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.GetEstadoTrabajadorByCode
{
    public class GetEstadoTrabajadorByCodeQueryHandler : IRequestHandler<GetEstadoTrabajadorByCodeQuery, EstadoTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetEstadoTrabajadorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EstadoTrabajadorResponse> Handle(GetEstadoTrabajadorByCodeQuery request, CancellationToken cancellationToken)
        {
            var estadoTrabajador = await _uow.Rrhh.Catalogos.EstadosTrabajador.Query()
                .AsNoTracking()
                .Where(e => e.Code == request.Code)
                .Select(e => new EstadoTrabajadorResponse
                {
                    Code = e.Code,
                    Name = e.Name,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    RowVersion = e.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (estadoTrabajador is null)
            {
                throw new KeyNotFoundException($"Estado de trabajador {request.Code} no encontrado.");
            }

            return estadoTrabajador;
        }
    }
}