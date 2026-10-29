using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.UpdateElementoConcreto
{
    public class UpdateElementoConcretoCommandHandler : IRequestHandler<UpdateElementoConcretoCommand, ElementoConcretoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateElementoConcretoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ElementoConcretoResponse> Handle(UpdateElementoConcretoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.ElementosConcreto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Elemento de concreto {request.Code} no encontrado.");
            }

            var elementoConcreto = new ElementoConcreto
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                UsesBalance = request.UsesBalance,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.ElementosConcreto.Update(elementoConcreto);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El elemento de concreto fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ElementoConcretoResponse
            {
                Code = elementoConcreto.Code,
                Name = elementoConcreto.Name,
                UsesBalance = elementoConcreto.UsesBalance,
                IsActive = elementoConcreto.IsActive,
                CreatedAt = elementoConcreto.CreatedAt,
                UpdatedAt = elementoConcreto.UpdatedAt,
                RowVersion = elementoConcreto.RowVersion
            };
        }
    }
}
