using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.CreateEspecialidad
{
    public class CreateEspecialidadCommandHandler
        : IRequestHandler<CreateEspecialidadCommand, EspecialidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateEspecialidadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EspecialidadResponse> Handle(
            CreateEspecialidadCommand request,
            CancellationToken cancellationToken)
        {
            var especialidad = new Especialidad
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Catalogos.Especialidades.AddAsync(especialidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(especialidad);
        }

        internal static EspecialidadResponse Map(Especialidad especialidad) => new()
        {
            Code = especialidad.Code,
            Name = especialidad.Name,
            IsActive = especialidad.IsActive,
            CreatedAt = especialidad.CreatedAt,
            UpdatedAt = especialidad.UpdatedAt,
            RowVersion = especialidad.RowVersion
        };
    }
}
