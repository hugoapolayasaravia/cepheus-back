using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.UpdateTransportista
{
    public class UpdateTransportistaCommandHandler : IRequestHandler<UpdateTransportistaCommand, TransportistaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTransportistaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TransportistaResponse> Handle(UpdateTransportistaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Maestros.Transportistas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Transportista {request.Code} no encontrado.");
            }

            var transportista = new TransportistaVenta
            {
                Code = request.Code,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                Name = request.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim().ToUpperInvariant(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                MtcInternalCode = string.IsNullOrWhiteSpace(request.MtcInternalCode) ? null : request.MtcInternalCode.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Transportistas.Update(transportista);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El transportista fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTransportista.CreateTransportistaCommandHandler.Map(transportista);
        }
    }
}
