using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.GetTrabajadorAntecedentesByTrabajador
{
    public class GetTrabajadorAntecedentesByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorAntecedentesByTrabajadorQuery, TrabajadorAntecedenteResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorAntecedentesByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorAntecedenteResponse?> Handle(GetTrabajadorAntecedentesByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorAntecedentes.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorAntecedenteResponse Map(TrabajadorAntecedente e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TieneAntecedentes = e.TieneAntecedentes,
            Descripcion = e.Descripcion,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
