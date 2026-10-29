using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.CreateHorario
{
    public class CreateHorarioCommandHandler : IRequestHandler<CreateHorarioCommand, HorarioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateHorarioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<HorarioResponse> Handle(CreateHorarioCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.Horarios.Query().Select(h => h.Code), length: 3, entityLabel: "Horarios", cancellationToken);

            var horario = new Horario
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.Horarios.AddAsync(horario, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(horario);
        }

        internal static HorarioResponse Map(Horario horario) => new()
        {
            Code = horario.Code,
            Name = horario.Name,
            IsActive = horario.IsActive,
            CreatedAt = horario.CreatedAt,
            UpdatedAt = horario.UpdatedAt,
            RowVersion = horario.RowVersion
        };
    }
}