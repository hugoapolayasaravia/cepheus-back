using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.CreateCotizacion
{
    public class CreateCotizacionCommandHandler : IRequestHandler<CreateCotizacionCommand, CotizacionResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CodeLength = 8;

        private readonly IUnitOfWork _uow;

        public CreateCotizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(CreateCotizacionCommand request, CancellationToken cancellationToken)
        {
            var negocioCode = request.NegocioCode.Trim().ToUpperInvariant();
            var year = request.Date.Year.ToString();
            var month = request.Date.Month.ToString().PadLeft(2, '0');

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(negocioCode, year, cancellationToken);

                var cotizacion = new Cotizacion
                {
                    NegocioCode = negocioCode,
                    Year = year,
                    Month = month,
                    Code = nextCode,

                    VendedorCode = request.VendedorCode.Trim().ToUpperInvariant(),
                    Date = request.Date,
                    CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant(),
                    FormaPagoVentaCode = request.FormaPagoVentaCode.Trim().ToUpperInvariant(),
                    TecnicoCode = Normalize(request.TecnicoCode),

                    AppliesIgv = request.AppliesIgv,
                    Discount = request.Discount,
                    GlobalVolume = request.GlobalVolume,
                    IsEditable = true,
                    Type = System.Enum.Parse<TipoCotizacion>(request.Type, ignoreCase: true),
                    MetradoCalculationSystem = string.IsNullOrWhiteSpace(request.MetradoCalculationSystem)
                        ? null
                        : System.Enum.Parse<SistemaCalculoMetrado>(request.MetradoCalculationSystem, ignoreCase: true),
                    WorkDurationMonths = request.WorkDurationMonths,

                    ClienteCode = Normalize(request.ClienteCode),
                    Ruc = request.Ruc?.Trim(),
                    ClientName = request.ClientName.Trim(),
                    ClientAddress = request.ClientAddress?.Trim(),
                    ClientAddressUbigeoCode = request.ClientAddressUbigeoCode.Trim(),

                    ObraCode = Normalize(request.ObraCode),
                    WorkName = request.WorkName.Trim(),
                    ProjectStatus = System.Enum.Parse<EstadoProyectoCotizacion>(request.ProjectStatus, ignoreCase: true),
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
                    ProcessDate = DateTime.UtcNow,

                    GrossAmount = 0,
                    IgvAmount = 0,
                    NetAmount = 0,

                    Status = EstadoCotizacion.Pendiente,
                    IsPrinted = false,
                    Observations = request.Observations?.Trim()
                };

                await _uow.Facturacion.Transacciones.Cotizaciones.AddAsync(cotizacion, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return CotizacionMapper.Map(cotizacion);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    // Colisión de correlativo por creación simultánea en el mismo
                    // negocio/año: se reintenta generando el siguiente código.
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Cotización para el negocio {negocioCode} - {year} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(string negocioCode, string year, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .Where(c => c.NegocioCode == negocioCode && c.Year == year)
                .OrderByDescending(c => c.Code)
                .Select(c => c.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return next.ToString().PadLeft(CodeLength, '0');
        }

        private static string? Normalize(string? code)
            => string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();
    }
}
