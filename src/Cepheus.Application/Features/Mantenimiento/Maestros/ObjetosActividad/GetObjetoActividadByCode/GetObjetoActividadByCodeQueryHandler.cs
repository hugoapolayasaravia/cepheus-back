using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.GetObjetoActividadByCode
{
    public class GetObjetoActividadByCodeQueryHandler
        : IRequestHandler<GetObjetoActividadByCodeQuery, ObjetoActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetObjetoActividadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ObjetoActividadResponse> Handle(
            GetObjetoActividadByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var objetoActividad = await _uow.Mantenimiento.Maestros.ObjetosActividad.Query()
                .AsNoTracking()
                .Where(o => o.Code == request.Code)
                .Select(o => new ObjetoActividadResponse
                {
                    Code = o.Code,
                    Name = o.Name,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    RowVersion = o.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (objetoActividad is null)
            {
                throw new KeyNotFoundException($"Objeto de actividad {request.Code} no encontrado.");
            }

            return objetoActividad;
        }
    }
}
