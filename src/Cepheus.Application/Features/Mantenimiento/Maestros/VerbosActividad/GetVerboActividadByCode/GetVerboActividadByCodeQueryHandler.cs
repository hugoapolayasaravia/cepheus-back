using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.GetVerboActividadByCode
{
    public class GetVerboActividadByCodeQueryHandler
        : IRequestHandler<GetVerboActividadByCodeQuery, VerboActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetVerboActividadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VerboActividadResponse> Handle(
            GetVerboActividadByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var verboActividad = await _uow.Mantenimiento.Maestros.VerbosActividad.Query()
                .AsNoTracking()
                .Where(v => v.Code == request.Code)
                .Select(v => new VerboActividadResponse
                {
                    Code = v.Code,
                    Name = v.Name,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    RowVersion = v.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (verboActividad is null)
            {
                throw new KeyNotFoundException($"Verbo de actividad {request.Code} no encontrado.");
            }

            return verboActividad;
        }
    }
}
