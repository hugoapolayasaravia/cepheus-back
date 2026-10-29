using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.GetTrabajadorPensionsByTrabajador
{
    public class GetTrabajadorPensionsByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorPensionsByTrabajadorQuery, TrabajadorPensionResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorPensionsByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorPensionResponse?> Handle(GetTrabajadorPensionsByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorPensions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorPensionResponse Map(TrabajadorPension e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TipoAfiliacionCode = e.TipoAfiliacionCode,
            AfpCode = e.AfpCode,
            FechaAfiliacion = e.FechaAfiliacion,
            NumeroAfp = e.NumeroAfp,
            RegimenPensionarioCode = e.RegimenPensionarioCode,
            TipoPensionCode = e.TipoPensionCode,
            NumeroCarnetSsp = e.NumeroCarnetSsp,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
