using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.CreateEquipo
{
    public class CreateEquipoCommandHandler : IRequestHandler<CreateEquipoCommand, EquipoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateEquipoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EquipoResponse> Handle(CreateEquipoCommand request, CancellationToken cancellationToken)
        {
            var equipo = new Equipo
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Nivel = request.Nivel,
                SubCentroCostoCode = string.IsNullOrWhiteSpace(request.SubCentroCostoCode)
                    ? null
                    : request.SubCentroCostoCode.Trim().ToUpperInvariant(),
                IsActive = true
            };

            await _uow.Mantenimiento.Maestros.Equipos.AddAsync(equipo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(equipo);
        }

        internal static EquipoResponse Map(Equipo equipo) => new()
        {
            Code = equipo.Code,
            Name = equipo.Name,
            Nivel = equipo.Nivel,
            SubCentroCostoCode = equipo.SubCentroCostoCode,
            IsActive = equipo.IsActive,
            CreatedAt = equipo.CreatedAt,
            UpdatedAt = equipo.UpdatedAt,
            RowVersion = equipo.RowVersion
        };
    }
}
