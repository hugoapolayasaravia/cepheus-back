using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.UpdateTipoBien
{
    public class UpdateTipoBienCommandHandler : IRequestHandler<UpdateTipoBienCommand, TipoBienResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoBienCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoBienResponse> Handle(UpdateTipoBienCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.TiposBien.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de bien {request.Code} no encontrado.");
            }

            var entity = new TipoBien
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                DetractionRate = request.DetractionRate,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.TiposBien.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTipoBien.CreateTipoBienCommandHandler.Map(entity);
        }
    }
}
