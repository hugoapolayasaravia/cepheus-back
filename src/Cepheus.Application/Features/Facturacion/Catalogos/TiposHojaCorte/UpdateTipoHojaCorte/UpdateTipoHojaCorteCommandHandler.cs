using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.UpdateTipoHojaCorte
{
    public class UpdateTipoHojaCorteCommandHandler : IRequestHandler<UpdateTipoHojaCorteCommand, TipoHojaCorteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoHojaCorteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoHojaCorteResponse> Handle(UpdateTipoHojaCorteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.TiposHojaCorte.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de hoja de corte {request.Code} no encontrado.");
            }

            var tipoHojaCorte = new TipoHojaCorte
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.TiposHojaCorte.Update(tipoHojaCorte);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de hoja de corte fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoHojaCorteResponse
            {
                Code = tipoHojaCorte.Code,
                Name = tipoHojaCorte.Name,
                IsActive = tipoHojaCorte.IsActive,
                CreatedAt = tipoHojaCorte.CreatedAt,
                UpdatedAt = tipoHojaCorte.UpdatedAt,
                RowVersion = tipoHojaCorte.RowVersion
            };
        }
    }
}
