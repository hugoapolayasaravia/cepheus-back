using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.GetNivelEducativoByCode
{
    public class GetNivelEducativoByCodeQueryHandler : IRequestHandler<GetNivelEducativoByCodeQuery, NivelEducativoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNivelEducativoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelEducativoResponse> Handle(GetNivelEducativoByCodeQuery request, CancellationToken cancellationToken)
        {
            var nivelEducativo = await _uow.Rrhh.Catalogos.NivelesEducativos.Query()
                .AsNoTracking()
                .Where(n => n.Code == request.Code)
                .Select(n => new NivelEducativoResponse
                {
                    Code = n.Code,
                    Name = n.Name,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    RowVersion = n.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (nivelEducativo is null)
            {
                throw new KeyNotFoundException($"Nivel educativo {request.Code} no encontrado.");
            }

            return nivelEducativo;
        }
    }
}