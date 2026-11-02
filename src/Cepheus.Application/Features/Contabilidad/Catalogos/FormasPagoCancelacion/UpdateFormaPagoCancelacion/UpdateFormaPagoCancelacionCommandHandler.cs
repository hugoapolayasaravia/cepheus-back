using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.Common;
using Cepheus.Domain.Contabilidad.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.UpdateFormaPagoCancelacion
{
    public class UpdateFormaPagoCancelacionCommandHandler : IRequestHandler<UpdateFormaPagoCancelacionCommand, FormaPagoCancelacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFormaPagoCancelacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoCancelacionResponse> Handle(UpdateFormaPagoCancelacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Contabilidad.Catalogos.FormasPagoCancelacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Forma de pago de cancelación {request.Code} no encontrada.");
            }

            var entity = new FormaPagoCancelacion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Contabilidad.Catalogos.FormasPagoCancelacion.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateFormaPagoCancelacion.CreateFormaPagoCancelacionCommandHandler.Map(entity);
        }
    }
}
