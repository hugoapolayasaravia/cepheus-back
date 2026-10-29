using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.GetTrabajadorFiscalsByTrabajador
{
    public class GetTrabajadorFiscalsByTrabajadorQueryHandler : IRequestHandler<GetTrabajadorFiscalsByTrabajadorQuery, TrabajadorFiscalResponse?>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorFiscalsByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorFiscalResponse?> Handle(GetTrabajadorFiscalsByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorFiscals.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TrabajadorCode == request.TrabajadorCode, cancellationToken);

            return entidad is null ? null : Map(entidad);
        }

        private static TrabajadorFiscalResponse Map(TrabajadorFiscal e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            ConInmTrabajador = e.ConInmTrabajador,
            Domiciliado = e.Domiciliado,
            OtrosIngresosQuinta = e.OtrosIngresosQuinta,
            RentaQuintaExonerada = e.RentaQuintaExonerada,
            MadreResFamiliar = e.MadreResFamiliar,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
