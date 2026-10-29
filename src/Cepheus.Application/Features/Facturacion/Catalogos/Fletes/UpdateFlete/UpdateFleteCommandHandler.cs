using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.UpdateFlete
{
    public class UpdateFleteCommandHandler : IRequestHandler<UpdateFleteCommand, FleteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFleteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FleteResponse> Handle(UpdateFleteCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.Fletes.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Flete {request.Code} no encontrado.");
            }

            var flete = new Flete
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Amount = request.Amount,
                IsDefault = request.IsDefault,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.Fletes.Update(flete);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El flete fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new FleteResponse
            {
                Code = flete.Code,
                Name = flete.Name,
                Amount = flete.Amount,
                IsDefault = flete.IsDefault,
                IsActive = flete.IsActive,
                CreatedAt = flete.CreatedAt,
                UpdatedAt = flete.UpdatedAt,
                RowVersion = flete.RowVersion
            };
        }
    }
}
