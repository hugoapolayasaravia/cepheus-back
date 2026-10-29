using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.UpdateClasificacionCliente
{
    public class UpdateClasificacionClienteCommandHandler : IRequestHandler<UpdateClasificacionClienteCommand, ClasificacionClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateClasificacionClienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClasificacionClienteResponse> Handle(UpdateClasificacionClienteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.ClasificacionesCliente.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Clasificación de cliente {request.Code} no encontrada.");
            }

            var clasificacionCliente = new ClasificacionCliente
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.ClasificacionesCliente.Update(clasificacionCliente);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La clasificación de cliente fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ClasificacionClienteResponse
            {
                Code = clasificacionCliente.Code,
                Name = clasificacionCliente.Name,
                IsActive = clasificacionCliente.IsActive,
                CreatedAt = clasificacionCliente.CreatedAt,
                UpdatedAt = clasificacionCliente.UpdatedAt,
                RowVersion = clasificacionCliente.RowVersion
            };
        }
    }
}
