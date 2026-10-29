using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.CreateSubCentroCosto
{
    public class CreateSubCentroCostoCommandHandler : IRequestHandler<CreateSubCentroCostoCommand, SubCentroCostoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubCentroCostoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubCentroCostoResponse> Handle(CreateSubCentroCostoCommand request, CancellationToken cancellationToken)
        {
            var centroCostoCode = string.IsNullOrWhiteSpace(request.CentroCostoCode)
                ? null
                : request.CentroCostoCode.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(centroCostoCode))
            {
                throw new ArgumentException(
                    "El Centro de Costo es obligatorio para crear un SubCentro de Costo.");
            }


            var code = await SequentialCodeGenerator.NextChildAsync(
                _uow.Logistica.Maestros.SubCentrosCosto.Query().Select(s => s.Code),
                centroCostoCode,
                suffixLength: 3,
                entityLabel: "SubCentros de Costo",
                cancellationToken);

            var subCentro = new SubCentroCosto
            {
                Code = code,
                CentroCostoCode = string.IsNullOrWhiteSpace(request.CentroCostoCode) ? null : request.CentroCostoCode.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                AccountingAccountCode = string.IsNullOrWhiteSpace(request.AccountingAccountCode) ? null : request.AccountingAccountCode.Trim(),
                AccountingAttachmentTypeCode = string.IsNullOrWhiteSpace(request.AccountingAttachmentTypeCode) ? null : request.AccountingAttachmentTypeCode.Trim(),
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                ParentCode = string.IsNullOrWhiteSpace(request.ParentCode) ? null : request.ParentCode.Trim().ToUpperInvariant(),
                IsActive = true
            };

            await _uow.Logistica.Maestros.SubCentrosCosto.AddAsync(subCentro, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(subCentro);
        }

        internal static SubCentroCostoResponse Map(SubCentroCosto subCentro) => new()
        {
            Code = subCentro.Code,
            CentroCostoCode = subCentro.CentroCostoCode,
            Name = subCentro.Name,
            AccountingAccountCode = subCentro.AccountingAccountCode,
            AccountingAttachmentTypeCode = subCentro.AccountingAttachmentTypeCode,
            PlantaCode = subCentro.PlantaCode,
            ParentCode = subCentro.ParentCode,
            IsActive = subCentro.IsActive,
            CreatedAt = subCentro.CreatedAt,
            UpdatedAt = subCentro.UpdatedAt,
            RowVersion = subCentro.RowVersion
        };
    }
}
