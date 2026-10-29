using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.GetTipoTrabajadorByCode
{
    public class GetTipoTrabajadorByCodeQueryHandler : IRequestHandler<GetTipoTrabajadorByCodeQuery, TipoTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoTrabajadorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoTrabajadorResponse> Handle(GetTipoTrabajadorByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoTrabajador = await _uow.Rrhh.Catalogos.TiposTrabajador.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoTrabajadorResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoTrabajador is null)
            {
                throw new KeyNotFoundException($"Tipo de trabajador {request.Code} no encontrado.");
            }

            return tipoTrabajador;
        }
    }
}