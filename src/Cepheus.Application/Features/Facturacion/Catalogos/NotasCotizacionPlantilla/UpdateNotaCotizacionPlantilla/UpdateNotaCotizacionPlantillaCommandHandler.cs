using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.UpdateNotaCotizacionPlantilla
{
    public class UpdateNotaCotizacionPlantillaCommandHandler
        : IRequestHandler<UpdateNotaCotizacionPlantillaCommand, NotaCotizacionPlantillaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNotaCotizacionPlantillaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCotizacionPlantillaResponse> Handle(UpdateNotaCotizacionPlantillaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.NegocioCode == request.NegocioCode && t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Nota de cotización {request.NegocioCode}-{request.Code} no encontrada.");
            }

            var nota = new NotaCotizacionPlantilla
            {
                NegocioCode = request.NegocioCode,
                Code = request.Code,
                Description = request.Description.Trim(),
                Option = request.Option,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.Update(nota);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nota fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new NotaCotizacionPlantillaResponse
            {
                NegocioCode = nota.NegocioCode,
                Code = nota.Code,
                Description = nota.Description,
                Option = nota.Option,
                IsActive = nota.IsActive,
                CreatedAt = nota.CreatedAt,
                UpdatedAt = nota.UpdatedAt,
                RowVersion = nota.RowVersion
            };
        }
    }
}
