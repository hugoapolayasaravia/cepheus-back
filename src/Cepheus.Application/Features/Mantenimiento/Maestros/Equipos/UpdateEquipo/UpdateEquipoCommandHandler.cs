using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.UpdateEquipo
{
    public class UpdateEquipoCommandHandler : IRequestHandler<UpdateEquipoCommand, EquipoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEquipoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EquipoResponse> Handle(UpdateEquipoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Maestros.Equipos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Equipo {request.Code} no encontrado.");
            }

            var equipo = new Equipo
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Nivel = request.Nivel,
                SubCentroCostoCode = string.IsNullOrWhiteSpace(request.SubCentroCostoCode)
                    ? null
                    : request.SubCentroCostoCode.Trim().ToUpperInvariant(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Maestros.Equipos.Update(equipo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El equipo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateEquipo.CreateEquipoCommandHandler.Map(equipo);
        }
    }
}
