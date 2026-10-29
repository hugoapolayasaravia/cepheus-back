using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.GetTituloByCode
{
    public class GetTituloByCodeQueryHandler : IRequestHandler<GetTituloByCodeQuery, TituloResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTituloByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TituloResponse> Handle(GetTituloByCodeQuery request, CancellationToken cancellationToken)
        {
            var titulo = await _uow.Rrhh.Catalogos.Titulos.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TituloResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (titulo is null)
            {
                throw new KeyNotFoundException($"Título {request.Code} no encontrado.");
            }

            return titulo;
        }
    }
}