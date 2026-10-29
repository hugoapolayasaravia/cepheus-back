using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.GetTrabajadorLaboralsByTrabajador
{
    public class GetTrabajadorLaboralsByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorLaboralsByTrabajadorQuery, TrabajadorLaboralResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorLaboralsByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorLaboralResponse?> Handle(GetTrabajadorLaboralsByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorLaborals.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorLaboralResponse Map(TrabajadorLaboral e) => new()
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
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
