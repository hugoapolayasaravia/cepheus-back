using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.GetTrabajadorSaludsByTrabajador
{
    public class GetTrabajadorSaludsByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorSaludsByTrabajadorQuery, TrabajadorSaludResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorSaludsByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSaludResponse?> Handle(GetTrabajadorSaludsByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorSaluds.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorSaludResponse Map(TrabajadorSalud e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TipoSangreCode = e.TipoSangreCode,
            AlergiaCode = e.AlergiaCode,
            Otros = e.Otros,
            FechaEvaluacionMedica = e.FechaEvaluacionMedica,
            Observaciones = e.Observaciones,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
