using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.CreateTrabajadorLaboral
{
    public class CreateTrabajadorLaboralCommandHandler : IRequestHandler<CreateTrabajadorLaboralCommand, TrabajadorLaboralResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorLaboralCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorLaboralResponse> Handle(CreateTrabajadorLaboralCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorLaboral
            {
                TrabajadorCode = request.TrabajadorCode,
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
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorLaborals.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorLaboralResponse Map(TrabajadorLaboral e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            FechaIngreso = e.FechaIngreso,
            FechaCese = e.FechaCese,
            TipoTrabajadorCode = e.TipoTrabajadorCode,
            CategoriaTrabajadorCode = e.CategoriaTrabajadorCode,
            EstadoTrabajadorCode = e.EstadoTrabajadorCode,
            AreaCode = e.AreaCode,
            OcupacionCode = e.OcupacionCode,
            SubOcupacionCode = e.SubOcupacionCode,
            OficinaCode = e.OficinaCode,
            PlantaCode = e.PlantaCode,
            CargoCode = e.CargoCode,
            NivelCode = e.NivelCode,
            RegimenLaboralCode = e.RegimenLaboralCode,
            ProveedorCode = e.ProveedorCode,
            Permanente = e.Permanente,
            Pensionista = e.Pensionista,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
