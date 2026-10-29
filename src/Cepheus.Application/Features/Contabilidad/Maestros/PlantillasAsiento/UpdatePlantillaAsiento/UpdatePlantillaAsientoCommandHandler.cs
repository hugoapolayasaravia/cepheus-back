using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.UpdatePlantillaAsiento
{
    public class UpdatePlantillaAsientoCommandHandler
        : IRequestHandler<UpdatePlantillaAsientoCommand, PlantillaAsientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlantillaAsientoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlantillaAsientoResponse> Handle(
            UpdatePlantillaAsientoCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var entity = await _uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Plantilla de asiento {code} no encontrada.");
            }

            if (entity.IsActive)
            {
                await PlantillaAsientoRules.EnsureSingleActiveAsync(_uow, request.Process, code, cancellationToken);
            }

            entity.Name = request.Name.Trim();
            entity.Process = request.Process;
            entity.TipoAsientoCode = request.TipoAsientoCode.Trim().ToUpperInvariant();
            entity.PurchaseAccountCode = string.IsNullOrWhiteSpace(request.PurchaseAccountCode)
                ? null
                : request.PurchaseAccountCode.Trim().ToUpperInvariant();
            entity.ComprobantePagoCode = string.IsNullOrWhiteSpace(request.ComprobantePagoCode)
                ? null
                : request.ComprobantePagoCode.Trim().ToUpperInvariant();
            entity.DescriptionText = string.IsNullOrWhiteSpace(request.DescriptionText) ? null : request.DescriptionText.Trim();
            entity.RowVersion = request.RowVersion;

            // Sincroniza las líneas por Order: modifica las existentes, agrega las nuevas y elimina las que ya no vienen.
            var incoming = request.Lines.ToDictionary(l => l.Order);

            foreach (var existing in entity.Lines.ToList())
            {
                if (incoming.ContainsKey(existing.Order))
                {
                    continue;
                }

                entity.Lines.Remove(existing);
                _uow.Contabilidad.Maestros.PlantillaAsientoLineas.Remove(existing);
            }

            foreach (var line in request.Lines)
            {
                var target = entity.Lines.FirstOrDefault(l => l.Order == line.Order);
                if (target is null)
                {
                    target = new PlantillaAsientoLinea { TemplateCode = code, Order = line.Order };
                    entity.Lines.Add(target);
                }

                target.Concept = line.Concept;
                target.Nature = line.Nature;
                target.AccountSource = line.AccountSource;
                target.AccountCode = string.IsNullOrWhiteSpace(line.AccountCode)
                    ? null
                    : line.AccountCode.Trim().ToUpperInvariant();
                target.AnnexSource = line.AnnexSource;
                target.DescriptionSource = line.DescriptionSource;
                target.InvertsOnCreditNote = line.InvertsOnCreditNote;
                target.IsDestination = line.IsDestination;
                target.AccountingType = line.AccountingType;
                target.ValeTypeCode = string.IsNullOrWhiteSpace(line.ValeTypeCode)
                    ? null
                    : line.ValeTypeCode.Trim().ToUpperInvariant();
            }

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La plantilla fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return PlantillaAsientoMapper.Map(entity);
        }
    }
}
