using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.CreateEspecialidad
{
    public class CreateEspecialidadTrabajadorCommandHandler : IRequestHandler<CreateEspecialidadTrabajadorCommand, EspecialidadTrabajadorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateEspecialidadTrabajadorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EspecialidadTrabajadorResponse> Handle(CreateEspecialidadTrabajadorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.EspecialidadesTrabajador.Query().Select(e => e.Code), length: 3, entityLabel: "Especialidades", cancellationToken);

            var especialidad = new EspecialidadTrabajador
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.EspecialidadesTrabajador.AddAsync(especialidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(especialidad);
        }

        internal static EspecialidadTrabajadorResponse Map(EspecialidadTrabajador especialidad) => new()
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