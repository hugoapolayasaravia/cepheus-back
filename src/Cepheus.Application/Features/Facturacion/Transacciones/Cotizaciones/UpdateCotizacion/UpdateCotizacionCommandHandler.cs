using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.UpdateCotizacion
{
    public class UpdateCotizacionCommandHandler : IRequestHandler<UpdateCotizacionCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCotizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(UpdateCotizacionCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (current is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            if (current.Status != EstadoCotizacion.Pendiente || !current.IsEditable)
                throw new InvalidOperationException("Solo se puede editar una cotización Pendiente y modificable.");

            var updated = new Cotizacion
            {
                // Clave e inmutables
                NegocioCode = current.NegocioCode,
                Year = current.Year,
                Month = current.Month,
                Code = current.Code,
                MetradoCalculationSystem = string.IsNullOrWhiteSpace(request.MetradoCalculationSystem)
                    ? null
                    : System.Enum.Parse<Cepheus.Domain.Facturacion.Enum.SistemaCalculoMetrado>(request.MetradoCalculationSystem, true),
                ProcessDate = current.ProcessDate,
                Status = current.Status,
                IsEditable = current.IsEditable,
                IsPrinted = current.IsPrinted,
                OriginNegocioCode = current.OriginNegocioCode,
                OriginYear = current.OriginYear,
                OriginMonth = current.OriginMonth,
                OriginCode = current.OriginCode,
                ApprovedBy = current.ApprovedBy,
                ApprovedAt = current.ApprovedAt,
                CancelReason = current.CancelReason,
                CanceledBy = current.CanceledBy,
                CanceledAt = current.CanceledAt,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                // Editables
                VendedorCode = request.VendedorCode.Trim().ToUpperInvariant(),
                Date = request.Date,
                CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant(),
                FormaPagoVentaCode = request.FormaPagoVentaCode.Trim().ToUpperInvariant(),
                TecnicoCode = Normalize(request.TecnicoCode),
                AppliesIgv = request.AppliesIgv,
                Discount = request.Discount,
                GlobalVolume = request.GlobalVolume,
                Type = System.Enum.Parse<TipoCotizacion>(request.Type, ignoreCase: true),
                WorkDurationMonths = request.WorkDurationMonths,
                ClienteCode = Normalize(request.ClienteCode),
                Ruc = request.Ruc?.Trim(),
                ClientName = request.ClientName.Trim(),
                ClientAddress = request.ClientAddress?.Trim(),
                ClientAddressUbigeoCode = request.ClientAddressUbigeoCode.Trim(),
                ObraCode = Normalize(request.ObraCode),
                WorkName = request.WorkName.Trim(),
                ProjectStatus = System.Enum.Parse<EstadoProyectoCotizacion>(request.ProjectStatus, true),
                WorkAddressUbigeoCode = request.WorkAddressUbigeoCode.Trim(),
                WorkAddress = request.WorkAddress.Trim(),
                ContactName = request.ContactName.Trim(),
                ContactPhone = request.ContactPhone.Trim(),
                ContactEmail = request.ContactEmail.Trim(),
                Reference = request.Reference?.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                DispatchDate = request.DispatchDate,
                FleteCode = request.FleteCode.Trim().ToUpperInvariant(),
                IgvRate = request.IgvRate,
                Observations = request.Observations?.Trim(),

                GrossAmount = current.GrossAmount,
                IgvAmount = current.IgvAmount,
                NetAmount = current.NetAmount,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Transacciones.Cotizaciones.Update(updated);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cotización fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            // IGV/descuento pudieron cambiar: recalcular totales con el detalle vigente.
            await CotizacionTotalesRecalculator.RecalculateAsync(_uow, updated.NegocioCode, updated.Year, updated.Month, updated.Code, cancellationToken);

            var reloaded = await _uow.Facturacion.Transacciones.Cotizaciones.Query().AsNoTracking()
                .FirstAsync(c => c.NegocioCode == updated.NegocioCode && c.Year == updated.Year
                              && c.Month == updated.Month && c.Code == updated.Code, cancellationToken);

            return CotizacionMapper.Map(reloaded);
        }

        private static string? Normalize(string? code)
            => string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();
    }
}
