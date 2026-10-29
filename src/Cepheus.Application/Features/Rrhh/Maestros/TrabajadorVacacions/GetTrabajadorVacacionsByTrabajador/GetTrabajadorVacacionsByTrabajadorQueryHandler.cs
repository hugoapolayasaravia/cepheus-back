using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.GetTrabajadorVacacionsByTrabajador
{
    public class GetTrabajadorVacacionsByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorVacacionsByTrabajadorQuery, List<TrabajadorVacacionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorVacacionsByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorVacacionResponse>> Handle(GetTrabajadorVacacionsByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.TrabajadorVacacions.Query()
                .AsNoTracking()
                .Where(e => e.TrabajadorCode == request.TrabajadorCode)
                .OrderBy(e => e.Id)
                .Select(e => Map(e))
                .ToListAsync(cancellationToken);
        }

        private static TrabajadorVacacionResponse Map(TrabajadorVacacion e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            FechaVacaciones = e.FechaVacaciones,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
