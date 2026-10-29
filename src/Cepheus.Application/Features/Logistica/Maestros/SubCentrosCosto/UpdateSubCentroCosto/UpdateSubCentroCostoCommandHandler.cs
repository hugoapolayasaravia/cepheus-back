using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.UpdateSubCentroCosto
{
    public class UpdateSubCentroCostoCommandHandler : IRequestHandler<UpdateSubCentroCostoCommand, SubCentroCostoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubCentroCostoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubCentroCostoResponse> Handle(UpdateSubCentroCostoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.SubCentrosCosto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"SubCentro de costo {request.Code} no encontrado.");
            }

            var subCentro = new SubCentroCosto
            {
                Code = request.Code,
                CentroCostoCode = string.IsNullOrWhiteSpace(request.CentroCostoCode) ? null : request.CentroCostoCode.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                AccountingAccountCode = string.IsNullOrWhiteSpace(request.AccountingAccountCode) ? null : request.AccountingAccountCode.Trim(),
                AccountingAttachmentTypeCode = string.IsNullOrWhiteSpace(request.AccountingAttachmentTypeCode) ? null : request.AccountingAttachmentTypeCode.Trim(),
                PlantaCode = request.PlantaCode.Trim().ToUpperInvariant(),
                ParentCode = string.IsNullOrWhiteSpace(request.ParentCode) ? null : request.ParentCode.Trim().ToUpperInvariant(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.SubCentrosCosto.Update(subCentro);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El subcentro de costo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateSubCentroCosto.CreateSubCentroCostoCommandHandler.Map(subCentro);
        }
    }
}
