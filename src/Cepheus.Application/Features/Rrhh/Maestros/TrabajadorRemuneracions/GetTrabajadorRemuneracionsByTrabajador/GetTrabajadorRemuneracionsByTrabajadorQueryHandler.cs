using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.GetTrabajadorRemuneracionsByTrabajador
{
    public class GetTrabajadorRemuneracionsByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorRemuneracionsByTrabajadorQuery, TrabajadorRemuneracionResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorRemuneracionsByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorRemuneracionResponse?> Handle(GetTrabajadorRemuneracionsByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorRemuneracions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorRemuneracionResponse Map(TrabajadorRemuneracion e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            SueldoBasico = e.SueldoBasico,
            MonedaCode = e.MonedaCode,
            ModoPagoCode = e.ModoPagoCode,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
