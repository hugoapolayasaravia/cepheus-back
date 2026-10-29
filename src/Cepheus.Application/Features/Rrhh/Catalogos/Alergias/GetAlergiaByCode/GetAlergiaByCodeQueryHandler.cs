using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.GetAlergiaByCode
{
    public class GetAlergiaByCodeQueryHandler : IRequestHandler<GetAlergiaByCodeQuery, AlergiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAlergiaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AlergiaResponse> Handle(GetAlergiaByCodeQuery request, CancellationToken cancellationToken)
        {
            var alergia = await _uow.Rrhh.Catalogos.Alergias.Query()
                .AsNoTracking()
                .Where(a => a.Code == request.Code)
                .Select(a => new AlergiaResponse
                {
                    Code = a.Code,
                    Name = a.Name,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    RowVersion = a.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (alergia is null)
            {
                throw new KeyNotFoundException($"Alergia {request.Code} no encontrada.");
            }

            return alergia;
        }
    }
}