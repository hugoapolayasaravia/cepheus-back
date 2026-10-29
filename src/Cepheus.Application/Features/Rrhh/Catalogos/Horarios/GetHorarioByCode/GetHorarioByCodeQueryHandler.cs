using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.GetHorarioByCode
{
    public class GetHorarioByCodeQueryHandler : IRequestHandler<GetHorarioByCodeQuery, HorarioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetHorarioByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<HorarioResponse> Handle(GetHorarioByCodeQuery request, CancellationToken cancellationToken)
        {
            var horario = await _uow.Rrhh.Catalogos.Horarios.Query()
                .AsNoTracking()
                .Where(h => h.Code == request.Code)
                .Select(h => new HorarioResponse
                {
                    Code = h.Code,
                    Name = h.Name,
                    IsActive = h.IsActive,
                    CreatedAt = h.CreatedAt,
                    UpdatedAt = h.UpdatedAt,
                    RowVersion = h.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (horario is null)
            {
                throw new KeyNotFoundException($"Horario {request.Code} no encontrado.");
            }

            return horario;
        }
    }
}