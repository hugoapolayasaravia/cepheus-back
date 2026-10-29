using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.UpdateTipoCompra
{
    public class UpdateTipoCompraCommandHandler : IRequestHandler<UpdateTipoCompraCommand, TipoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoCompraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCompraResponse> Handle(UpdateTipoCompraCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.TiposCompra.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de compra {request.Code} no encontrado.");
            }

            var tipoCompra = new TipoCompra
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.TiposCompra.Update(tipoCompra);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de compra fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoCompraResponse
            {
                Code = tipoCompra.Code,
                Name = tipoCompra.Name,
                IsActive = tipoCompra.IsActive,
                CreatedAt = tipoCompra.CreatedAt,
                UpdatedAt = tipoCompra.UpdatedAt,
                CreatedBy = tipoCompra.CreatedBy,
                UpdatedBy = tipoCompra.UpdatedBy,
                RowVersion = tipoCompra.RowVersion
            };
        }
    }
}