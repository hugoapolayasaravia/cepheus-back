using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.GetTipoSangreByCode
{
    public class GetTipoSangreByCodeQueryHandler : IRequestHandler<GetTipoSangreByCodeQuery, TipoSangreResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoSangreByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoSangreResponse> Handle(GetTipoSangreByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoSangre = await _uow.Rrhh.Catalogos.TiposSangre.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoSangreResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoSangre is null)
            {
                throw new KeyNotFoundException($"Tipo de sangre {request.Code} no encontrado.");
            }

            return tipoSangre;
        }
    }
}