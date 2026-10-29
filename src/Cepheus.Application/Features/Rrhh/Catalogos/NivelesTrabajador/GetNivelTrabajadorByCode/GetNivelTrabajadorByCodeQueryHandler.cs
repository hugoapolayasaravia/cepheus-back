using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.GetNivelTrabajadorByCode
{
    public class GetNivelTrabajadorByCodeQueryHandler
        : IRequestHandler<GetNivelTrabajadorByCodeQuery, NivelTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNivelTrabajadorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelTrabajadorResponse> Handle(GetNivelTrabajadorByCodeQuery request, CancellationToken cancellationToken)
        {
            var nivelTrabajador = await _uow.Rrhh.Catalogos.NivelesTrabajador.Query()
                .AsNoTracking()
                .Where(n => n.Code == request.Code)
                .Select(n => new NivelTrabajadorResponse
                {
                    Code = n.Code,
                    Name = n.Name,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    RowVersion = n.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (nivelTrabajador is null)
            {
                throw new KeyNotFoundException($"Nivel {request.Code} no encontrado.");
            }

            return nivelTrabajador;
        }
    }
}