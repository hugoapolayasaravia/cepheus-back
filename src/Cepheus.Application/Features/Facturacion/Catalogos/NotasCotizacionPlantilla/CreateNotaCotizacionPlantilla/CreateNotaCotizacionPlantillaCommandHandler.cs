using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.CreateNotaCotizacionPlantilla
{
    public class CreateNotaCotizacionPlantillaCommandHandler
        : IRequestHandler<CreateNotaCotizacionPlantillaCommand, NotaCotizacionPlantillaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNotaCotizacionPlantillaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCotizacionPlantillaResponse> Handle(CreateNotaCotizacionPlantillaCommand request, CancellationToken cancellationToken)
        {
            var existingCodes = _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.Query()
                .Where(t => t.NegocioCode == request.NegocioCode)
                .Select(t => t.Code);

            var code = await SequentialCodeGenerator.NextAsync(
                existingCodes, length: 2, entityLabel: $"Notas de Cotización del negocio {request.NegocioCode}", cancellationToken);

            var nota = new NotaCotizacionPlantilla
            {
                NegocioCode = request.NegocioCode,
                Code = code,
                Description = request.Description.Trim(),
                Option = request.Option,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.AddAsync(nota, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(nota);
        }

        internal static NotaCotizacionPlantillaResponse Map(NotaCotizacionPlantilla nota) => new()
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
