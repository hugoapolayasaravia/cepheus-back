using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.UpdateTipoCliente
{
    public class UpdateTipoClienteCommandHandler : IRequestHandler<UpdateTipoClienteCommand, TipoClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoClienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoClienteResponse> Handle(UpdateTipoClienteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.TiposCliente.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de cliente {request.Code} no encontrado.");
            }

            var tipoCliente = new TipoCliente
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.TiposCliente.Update(tipoCliente);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de cliente fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoClienteResponse
            {
                Code = tipoCliente.Code,
                Name = tipoCliente.Name,
                IsActive = tipoCliente.IsActive,
                CreatedAt = tipoCliente.CreatedAt,
                UpdatedAt = tipoCliente.UpdatedAt,
                RowVersion = tipoCliente.RowVersion
            };
        }
    }
}
