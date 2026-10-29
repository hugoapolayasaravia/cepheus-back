using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.CreateVerboActividad
{
    public class CreateVerboActividadCommandHandler
        : IRequestHandler<CreateVerboActividadCommand, VerboActividadResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateVerboActividadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VerboActividadResponse> Handle(
            CreateVerboActividadCommand request,
            CancellationToken cancellationToken)
        {
            var verboActividad = new VerboActividad
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Maestros.VerbosActividad.AddAsync(verboActividad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(verboActividad);
        }

        internal static VerboActividadResponse Map(VerboActividad verboActividad) => new()
        {
            Code = verboActividad.Code,
            Name = verboActividad.Name,
            IsActive = verboActividad.IsActive,
            CreatedAt = verboActividad.CreatedAt,
            UpdatedAt = verboActividad.UpdatedAt,
            RowVersion = verboActividad.RowVersion
        };
    }
}
