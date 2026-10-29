using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.GetSubCentroCostoByCode
{
    public class GetSubCentroCostoByCodeQueryHandler : IRequestHandler<GetSubCentroCostoByCodeQuery, SubCentroCostoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSubCentroCostoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubCentroCostoResponse> Handle(GetSubCentroCostoByCodeQuery request, CancellationToken cancellationToken)
        {
            var subCentro = await _uow.Logistica.Maestros.SubCentrosCosto.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SubCentroCostoResponse
                {
                    Code = s.Code,
                    CentroCostoCode = s.CentroCostoCode,
                    Name = s.Name,
                    AccountingAccountCode = s.AccountingAccountCode,
                    AccountingAttachmentTypeCode = s.AccountingAttachmentTypeCode,
                    PlantaCode = s.PlantaCode,
                    ParentCode = s.ParentCode,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (subCentro is null)
            {
                throw new KeyNotFoundException($"SubCentro de costo {request.Code} no encontrado.");
            }

            return subCentro;
        }
    }
}
