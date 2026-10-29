using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.GetOportunidadByCode
{
    public class GetOportunidadByCodeQueryHandler
        : IRequestHandler<GetOportunidadByCodeQuery, OportunidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetOportunidadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OportunidadResponse> Handle(
            GetOportunidadByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var oportunidad = await _uow.Mantenimiento.Catalogos.Oportunidades.Query()
                .AsNoTracking()
                .Where(o => o.Code == request.Code)
                .Select(o => new OportunidadResponse
                {
                    Code = o.Code,
                    Name = o.Name,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RowVersion = o.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (oportunidad is null)
            {
                throw new KeyNotFoundException($"Oportunidad {request.Code} no encontrada.");
            }

            return oportunidad;
        }
    }
}
