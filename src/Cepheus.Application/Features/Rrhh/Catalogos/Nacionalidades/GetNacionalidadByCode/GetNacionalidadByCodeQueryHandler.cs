using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.GetNacionalidadByCode
{
    public class GetNacionalidadByCodeQueryHandler : IRequestHandler<GetNacionalidadByCodeQuery, NacionalidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNacionalidadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NacionalidadResponse> Handle(GetNacionalidadByCodeQuery request, CancellationToken cancellationToken)
        {
            var nacionalidad = await _uow.Rrhh.Catalogos.Nacionalidades.Query()
                .AsNoTracking()
                .Where(n => n.Code == request.Code)
                .Select(n => new NacionalidadResponse
                {
                    Code = n.Code,
                    Name = n.Name,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    RowVersion = n.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (nacionalidad is null)
            {
                throw new KeyNotFoundException($"Nacionalidad {request.Code} no encontrada.");
            }

            return nacionalidad;
        }
    }
}