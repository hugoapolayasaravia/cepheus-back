using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.UpdateTrabajadorLaboral
{
    public class UpdateTrabajadorLaboralCommandHandler : IRequestHandler<UpdateTrabajadorLaboralCommand, TrabajadorLaboralResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorLaboralCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorLaboralResponse> Handle(UpdateTrabajadorLaboralCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorLaborals.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorLaboral {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorLaboral
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                FechaIngreso = request.FechaIngreso,
                FechaCese = request.FechaCese,
                TipoTrabajadorCode = request.TipoTrabajadorCode,
                CategoriaTrabajadorCode = request.CategoriaTrabajadorCode,
                EstadoTrabajadorCode = request.EstadoTrabajadorCode,
                AreaCode = request.AreaCode,
                OcupacionCode = request.OcupacionCode,
                SubOcupacionCode = request.SubOcupacionCode,
                OficinaCode = request.OficinaCode,
                PlantaCode = request.PlantaCode,
                CargoCode = request.CargoCode,
                NivelCode = request.NivelCode,
                RegimenLaboralCode = request.RegimenLaboralCode,
                ProveedorCode = request.ProveedorCode,
                Permanente = request.Permanente,
                Pensionista = request.Pensionista,
                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorLaborals.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorLaboral.CreateTrabajadorLaboralCommandHandler.Map(entidad);
        }
    }
}
