using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.CreatePlantillaAsiento
{
    public class CreatePlantillaAsientoCommandHandler
        : IRequestHandler<CreatePlantillaAsientoCommand, PlantillaAsientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlantillaAsientoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlantillaAsientoResponse> Handle(
            CreatePlantillaAsientoCommand request, CancellationToken cancellationToken)
        {
            await PlantillaAsientoRules.EnsureSingleActiveAsync(_uow, request.Process, null, cancellationToken);

            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Contabilidad.Maestros.PlantillasAsiento.Query().Select(p => p.Code),
                3,
                "PlantillasAsiento",
                cancellationToken);

            var entity = new PlantillaAsiento
            {
                Code = code,
                Name = request.Name.Trim(),
                Process = request.Process,
                TipoAsientoCode = request.TipoAsientoCode.Trim().ToUpperInvariant(),
                PurchaseAccountCode = string.IsNullOrWhiteSpace(request.PurchaseAccountCode)
                    ? null
                    : request.PurchaseAccountCode.Trim().ToUpperInvariant(),
                ComprobantePagoCode = string.IsNullOrWhiteSpace(request.ComprobantePagoCode)
                    ? null
                    : request.ComprobantePagoCode.Trim().ToUpperInvariant(),
                DescriptionText = string.IsNullOrWhiteSpace(request.DescriptionText) ? null : request.DescriptionText.Trim(),
                IsActive = true
            };

            foreach (var line in request.Lines)
            {
                entity.Lines.Add(new PlantillaAsientoLinea
                {
                    TemplateCode = code,
                    Order = line.Order,
                    Concept = line.Concept,
                    Nature = line.Nature,
                    AccountSource = line.AccountSource,
                    AccountCode = string.IsNullOrWhiteSpace(line.AccountCode)
                        ? null
                        : line.AccountCode.Trim().ToUpperInvariant(),
                    AnnexSource = line.AnnexSource,
                    DescriptionSource = line.DescriptionSource,
                    InvertsOnCreditNote = line.InvertsOnCreditNote,
                    IsDestination = line.IsDestination,
                    AccountingType = line.AccountingType,
                    ValeTypeCode = string.IsNullOrWhiteSpace(line.ValeTypeCode)
                        ? null
                        : line.ValeTypeCode.Trim().ToUpperInvariant()
                });
            }

            await _uow.Contabilidad.Maestros.PlantillasAsiento.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return PlantillaAsientoMapper.Map(entity);
        }
    }
}
