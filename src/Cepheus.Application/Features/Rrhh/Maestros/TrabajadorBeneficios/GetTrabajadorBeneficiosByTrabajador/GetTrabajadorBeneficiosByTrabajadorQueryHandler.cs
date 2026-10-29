using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.GetTrabajadorBeneficiosByTrabajador
{
    public class GetTrabajadorBeneficiosByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorBeneficiosByTrabajadorQuery, TrabajadorBeneficioResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorBeneficiosByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorBeneficioResponse?> Handle(GetTrabajadorBeneficiosByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorBeneficios.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorBeneficioResponse Map(TrabajadorBeneficio e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            Cts = e.Cts,
            Gratificacion = e.Gratificacion,
            Vacaciones = e.Vacaciones,
            MovilidadAntesEntrada = e.MovilidadAntesEntrada,
            MovilidadDespuesSalida = e.MovilidadDespuesSalida,
            Refrigerio = e.Refrigerio,
            Cena = e.Cena,
            Vale = e.Vale,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
