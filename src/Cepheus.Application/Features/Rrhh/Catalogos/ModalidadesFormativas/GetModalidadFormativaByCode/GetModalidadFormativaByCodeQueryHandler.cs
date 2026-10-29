using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.GetModalidadFormativaByCode
{
    public class GetModalidadFormativaByCodeQueryHandler
        : IRequestHandler<GetModalidadFormativaByCodeQuery, ModalidadFormativaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetModalidadFormativaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModalidadFormativaResponse> Handle(GetModalidadFormativaByCodeQuery request, CancellationToken cancellationToken)
        {
            var modalidadFormativa = await _uow.Rrhh.Catalogos.ModalidadesFormativas.Query()
                .AsNoTracking()
                .Where(m => m.Code == request.Code)
                .Select(m => new ModalidadFormativaResponse
                {
                    Code = m.Code,
                    Name = m.Name,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    RowVersion = m.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (modalidadFormativa is null)
            {
                throw new KeyNotFoundException($"Modalidad formativa {request.Code} no encontrada.");
            }

            return modalidadFormativa;
        }
    }
}