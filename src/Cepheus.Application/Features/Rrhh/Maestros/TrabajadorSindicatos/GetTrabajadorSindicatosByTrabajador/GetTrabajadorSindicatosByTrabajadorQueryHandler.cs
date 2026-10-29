using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.GetTrabajadorSindicatosByTrabajador
{
    public class GetTrabajadorSindicatosByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorSindicatosByTrabajadorQuery, TrabajadorSindicatoResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorSindicatosByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSindicatoResponse?> Handle(GetTrabajadorSindicatosByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorSindicatos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorSindicatoResponse Map(TrabajadorSindicato e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            Afiliado = e.Afiliado,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
