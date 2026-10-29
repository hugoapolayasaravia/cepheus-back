using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Articulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.GetArticuloByCode
{
    public class GetArticuloByCodeQueryHandler : IRequestHandler<GetArticuloByCodeQuery, ArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloResponse> Handle(GetArticuloByCodeQuery request, CancellationToken cancellationToken)
        {
            var articulo = await _uow.Logistica.Maestros.Articulos.Query()
                .AsNoTracking()
                .Where(a => a.Code == request.Code)
                .Select(a => new ArticuloResponse
                {
                    Code = a.Code,
                    Name = a.Name,
                    UnidadMedidaCode = a.UnidadMedidaCode,
                    SubFamiliaCode = a.SubFamiliaCode,
                    SubFamiliaName = a.SubFamilia.Name,
                    UnidadMedidaName = a.UnidadMedida.Name,
                    MinStock = a.MinStock,
                    MaxStock = a.MaxStock,
                    IncomingStock = a.IncomingStock,
                    LeadTimeDays = a.LeadTimeDays,
                    AbcClass = a.AbcClass,
                    TipoArticuloCode = a.TipoArticuloCode,
                    PlanCode = a.PlanCode,
                    ManufacturerCode = a.ManufacturerCode,
                    Observations = a.Observations,
                    SalesTypeCode = a.SalesTypeCode,
                    SalesProductCode = a.SalesProductCode,
                    IsAgreement = a.IsAgreement,
                    AccountingAccountCode = a.AccountingAccountCode,
                    AccountingAttachmentTypeCode = a.AccountingAttachmentTypeCode,
                    PlantOriginCode = a.PlantOriginCode,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    CreatedBy = a.CreatedBy,
                    UpdatedBy = a.UpdatedBy,
                    RowVersion = a.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (articulo is null)
            {
                throw new KeyNotFoundException($"Artículo {request.Code} no encontrado.");
            }

            return articulo;
        }
    }
}
