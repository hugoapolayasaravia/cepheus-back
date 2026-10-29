using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.UpdateResponsableObra
{
    public class UpdateResponsableObraCommandHandler : IRequestHandler<UpdateResponsableObraCommand, ResponsableObraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateResponsableObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ResponsableObraResponse> Handle(UpdateResponsableObraCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.ResponsablesObra.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.ClienteCode == request.ClienteCode &&
                    r.ObraCode == request.ObraCode &&
                    r.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException("Responsable de obra no encontrado para la combinación indicada.");
            }

            var responsable = new ResponsableObra
            {
                ClienteCode = request.ClienteCode,
                ObraCode = request.ObraCode,
                Code = request.Code,
                Name = request.Name.Trim(),
                Phone = request.Phone.Trim(),
                Position = request.Position.Trim(),

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.ResponsablesObra.Update(responsable);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El responsable fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ResponsableObraResponse
            {
                ClienteCode = responsable.ClienteCode,
                ObraCode = responsable.ObraCode,
                Code = responsable.Code,
                Name = responsable.Name,
                Phone = responsable.Phone,
                Position = responsable.Position,
                CreatedAt = responsable.CreatedAt,
                UpdatedAt = responsable.UpdatedAt,
                RowVersion = responsable.RowVersion
            };
        }
    }
}
