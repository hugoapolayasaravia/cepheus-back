using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.UpdateChofer
{
    public class UpdateChoferCommandHandler : IRequestHandler<UpdateChoferCommand, ChoferResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateChoferCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChoferResponse> Handle(UpdateChoferCommand request, CancellationToken cancellationToken)
        {
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Facturacion.Maestros.Choferes.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.TransportistaCode == transportistaCode && c.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Chofer {transportistaCode}/{code} no encontrado.");
            }

            var chofer = new ChoferVenta
            {
                TransportistaCode = transportistaCode,
                Code = code,
                FullName = request.FullName.Trim(),
                DriverLicenseNumber = request.DriverLicenseNumber.Trim().ToUpperInvariant(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Choferes.Update(chofer);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El chofer fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateChofer.CreateChoferCommandHandler.Map(chofer);
        }
    }
}
