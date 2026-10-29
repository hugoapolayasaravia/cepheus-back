using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.GetActividadByCode
{
    public class GetActividadByCodeQueryHandler : IRequestHandler<GetActividadByCodeQuery, ActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetActividadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ActividadResponse> Handle(GetActividadByCodeQuery request, CancellationToken cancellationToken)
        {
            var actividad = await _uow.Mantenimiento.Maestros.Actividades.Query()
                .AsNoTracking()
                .Where(a => a.Code == request.Code)
                .Select(a => new ActividadResponse
                {
                    Code = a.Code,
                    VerboActividadCode = a.VerboActividadCode,
                    VerboActividadName = a.VerboActividad.Name,
                    ObjetoActividadCode = a.ObjetoActividadCode,
                    ObjetoActividadName = a.ObjetoActividad.Name,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    RowVersion = a.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (actividad is null)
            {
                throw new KeyNotFoundException($"Actividad {request.Code} no encontrada.");
            }

            return actividad;
        }
    }
}
