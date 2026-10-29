using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.GetRegimenPensionarioByCode
{
    public class GetRegimenPensionarioByCodeQueryHandler
        : IRequestHandler<GetRegimenPensionarioByCodeQuery, RegimenPensionarioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetRegimenPensionarioByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RegimenPensionarioResponse> Handle(GetRegimenPensionarioByCodeQuery request, CancellationToken cancellationToken)
        {
            var regimenPensionario = await _uow.Rrhh.Catalogos.RegimenesPensionarios.Query()
                .AsNoTracking()
                .Where(r => r.Code == request.Code)
                .Select(r => new RegimenPensionarioResponse
                {
                    Code = r.Code,
                    Name = r.Name,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    RowVersion = r.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (regimenPensionario is null)
            {
                throw new KeyNotFoundException($"Régimen pensionario {request.Code} no encontrado.");
            }

            return regimenPensionario;
        }
    }
}