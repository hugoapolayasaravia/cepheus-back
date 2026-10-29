using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Articulos.Common;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.CreateArticulo
{
    public class CreateArticuloCommandHandler : IRequestHandler<CreateArticuloCommand, ArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloResponse> Handle(CreateArticuloCommand request, CancellationToken cancellationToken)
        {
            var subFamiliaCode = request.SubFamiliaCode.Trim().ToUpperInvariant();

            var code = await SequentialCodeGenerator.NextChildAsync(
                _uow.Logistica.Maestros.Articulos.Query().Select(a => a.Code),
                prefix: subFamiliaCode,
                suffixLength: 3,
                entityLabel: "Artículos",
                cancellationToken);

            var articulo = new Articulo
            {
                Code = code,
                Name = request.Name.Trim(),
                UnidadMedidaCode = request.UnidadMedidaCode.Trim().ToUpperInvariant(),
                SubFamiliaCode = request.SubFamiliaCode.Trim().ToUpperInvariant(),
                MinStock = request.MinStock,
                MaxStock = request.MaxStock,
                IncomingStock = request.IncomingStock,
                LeadTimeDays = request.LeadTimeDays,
                AbcClass = request.AbcClass,
                TipoArticuloCode = request.TipoArticuloCode.Trim().ToUpperInvariant(),
                PlanCode = request.PlanCode.Trim().ToUpperInvariant(),
                ManufacturerCode = string.IsNullOrWhiteSpace(request.ManufacturerCode) ? null : request.ManufacturerCode.Trim(),
                Observations = request.Observations?.Trim() ?? string.Empty,
                SalesTypeCode = string.IsNullOrWhiteSpace(request.SalesTypeCode) ? null : request.SalesTypeCode.Trim(),
                SalesProductCode = string.IsNullOrWhiteSpace(request.SalesProductCode) ? null : request.SalesProductCode.Trim(),
                IsAgreement = request.IsAgreement,
                AccountingAccountCode = string.IsNullOrWhiteSpace(request.AccountingAccountCode) ? null : request.AccountingAccountCode.Trim(),
                AccountingAttachmentTypeCode = string.IsNullOrWhiteSpace(request.AccountingAttachmentTypeCode) ? null : request.AccountingAttachmentTypeCode.Trim(),
                PlantOriginCode = string.IsNullOrWhiteSpace(request.PlantOriginCode) ? null : request.PlantOriginCode.Trim().ToUpperInvariant(),
                IsActive = true
            };

            await _uow.Logistica.Maestros.Articulos.AddAsync(articulo, cancellationToken);

            //var tiposVale = await _uow.Logistica.Catalogos.TiposVale
            //    .Query()
            //    .Where(tv => tv.IsActive)
            //    .ToListAsync(cancellationToken);

            //// 3. Crear la relación Artículo <-> TipoVale
            //foreach (var tipoVale in tiposVale)
            //{
            //    var tipoValeArticulo = new TipoValeArticulo
            //    {
            //        TipoValeCode = tipoVale.Code,
            //        ArticuloCode = articulo.Code
            //    };

            //    await _uow.Logistica.Catalogos.TiposValeArticulo
            //        .AddAsync(tipoValeArticulo, cancellationToken);
            //}

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(articulo);
        }

        internal static ArticuloResponse Map(Articulo articulo) => new()
        {
            Code = articulo.Code,
            Name = articulo.Name,
            UnidadMedidaCode = articulo.UnidadMedidaCode,
            SubFamiliaCode = articulo.SubFamiliaCode,
            MinStock = articulo.MinStock,
            MaxStock = articulo.MaxStock,
            IncomingStock = articulo.IncomingStock,
            LeadTimeDays = articulo.LeadTimeDays,
            AbcClass = articulo.AbcClass,
            TipoArticuloCode = articulo.TipoArticuloCode,
            PlanCode = articulo.PlanCode,
            ManufacturerCode = articulo.ManufacturerCode,
            Observations = articulo.Observations,
            SalesTypeCode = articulo.SalesTypeCode,
            SalesProductCode = articulo.SalesProductCode,
            IsAgreement = articulo.IsAgreement,
            AccountingAccountCode = articulo.AccountingAccountCode,
            AccountingAttachmentTypeCode = articulo.AccountingAttachmentTypeCode,
            PlantOriginCode = articulo.PlantOriginCode,
            IsActive = articulo.IsActive,
            CreatedAt = articulo.CreatedAt,
            UpdatedAt = articulo.UpdatedAt,
            RowVersion = articulo.RowVersion
        };
    }
}
