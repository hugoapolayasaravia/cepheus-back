using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.GetSexoByCode
{
    public class GetSexoByCodeQueryHandler : IRequestHandler<GetSexoByCodeQuery, SexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSexoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SexoResponse> Handle(GetSexoByCodeQuery request, CancellationToken cancellationToken)
        {
            var sexo = await _uow.Rrhh.Catalogos.Sexos.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SexoResponse
                {
                    Code = s.Code,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (sexo is null)
            {
                throw new KeyNotFoundException($"Sexo {request.Code} no encontrado.");
            }

            return sexo;
        }
    }
}