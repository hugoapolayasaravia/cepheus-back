using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Articulos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.UpdateArticulo
{
    public class UpdateArticuloCommandHandler : IRequestHandler<UpdateArticuloCommand, ArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloResponse> Handle(UpdateArticuloCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.Articulos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Artículo {request.Code} no encontrado.");
            }

            var articulo = new Articulo
            {
                Code = request.Code,
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

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.Articulos.Update(articulo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El artículo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateArticulo.CreateArticuloCommandHandler.Map(articulo);
        }
    }
}
