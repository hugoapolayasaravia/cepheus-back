using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.CreateActividad
{
    public class CreateActividadCommandHandler : IRequestHandler<CreateActividadCommand, ActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateActividadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ActividadResponse> Handle(CreateActividadCommand request, CancellationToken cancellationToken)
        {
            var verboCode = request.VerboActividadCode.Trim().ToUpperInvariant();
            var objetoCode = request.ObjetoActividadCode.Trim().ToUpperInvariant();

            var actividad = new Actividad
            {
                Code = verboCode + objetoCode,
                VerboActividadCode = verboCode,
                ObjetoActividadCode = objetoCode,
                IsActive = true
            };

            await _uow.Mantenimiento.Maestros.Actividades.AddAsync(actividad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return await MapAsync(_uow, actividad, cancellationToken);
        }

        internal static async Task<ActividadResponse> MapAsync(IUnitOfWork uow, Actividad actividad, CancellationToken cancellationToken)
        {
            var verboName = await uow.Mantenimiento.Maestros.VerbosActividad.Query()
                .Where(v => v.Code == actividad.VerboActividadCode)
                .Select(v => v.Name)
                .FirstAsync(cancellationToken);

            var objetoName = await uow.Mantenimiento.Maestros.ObjetosActividad.Query()
                .Where(o => o.Code == actividad.ObjetoActividadCode)
                .Select(o => o.Name)
                .FirstAsync(cancellationToken);

            return new ActividadResponse
            {
                Code = actividad.Code,
                VerboActividadCode = actividad.VerboActividadCode,
                VerboActividadName = verboName,
                ObjetoActividadCode = actividad.ObjetoActividadCode,
                ObjetoActividadName = objetoName,
                IsActive = actividad.IsActive,
                CreatedAt = actividad.CreatedAt,
                UpdatedAt = actividad.UpdatedAt,
                RowVersion = actividad.RowVersion
            };
        }
    }
}
