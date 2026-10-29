using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.GetTrabajadorContratosByTrabajador
{
    public class GetTrabajadorContratosByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorContratosByTrabajadorQuery, List<TrabajadorContratoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorContratosByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorContratoResponse>> Handle(GetTrabajadorContratosByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.TrabajadorContratos.Query()
                .AsNoTracking()
                .Where(e => e.TrabajadorCode == request.TrabajadorCode)
                .OrderBy(e => e.Id)
                .Select(e => Map(e))
                .ToListAsync(cancellationToken);
        }

        private static TrabajadorContratoResponse Map(TrabajadorContrato e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TipoContratoCode = e.TipoContratoCode,
            TipoExtensionCode = e.TipoExtensionCode,
            FechaInicio = e.FechaInicio,
            FechaFin = e.FechaFin,
            FechaTermino = e.FechaTermino,
            Renovado = e.Renovado,
            TipoDuracion = e.TipoDuracion,
            CantidadDuracion = e.CantidadDuracion,
            Activo = e.Activo,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
