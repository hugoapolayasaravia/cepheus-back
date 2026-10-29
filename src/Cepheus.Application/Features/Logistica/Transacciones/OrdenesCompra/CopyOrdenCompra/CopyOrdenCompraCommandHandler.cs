// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/CopyOrdenCompra/CopyOrdenCompraCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CopyOrdenCompra
{
    /// <summary>
    /// Copia cabecera + OrdenCompraDetalle, nace en Pendiente (confirmado).
    /// Igual criterio que CopyCotizacionCommandHandler: NO arrastra
    /// OrdenCompraPedidoOrigen ni AprobadoPor/FechaAprobacion.
    /// </summary>
    public class CopyOrdenCompraCommandHandler : IRequestHandler<CopyOrdenCompraCommand, OrdenCompraResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CorrelativeLength = 5;
        private const string CodePrefix = "5";

        private readonly IUnitOfWork _uow;

        public CopyOrdenCompraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraResponse> Handle(CopyOrdenCompraCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var sourceCode = request.Code.Trim().ToUpperInvariant();

            var source = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .AsNoTracking()
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == sourceCode, cancellationToken);

            if (source is null)
            {
                throw new KeyNotFoundException($"Orden de Compra {plantaCode}/{sourceCode} no encontrada.");
            }

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(plantaCode, cancellationToken);

                var copia = new OrdenCompra
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    TipoCompraCode = source.TipoCompraCode,
                    ComprobantePagoCode = source.ComprobantePagoCode,
                    FechaEntrega = request.NuevaFechaEntrega,
                    ProveedorCode = source.ProveedorCode,
                    CompradorCode = source.CompradorCode,
                    MonedaCode = source.MonedaCode,
                    LugarEnvioCode = source.LugarEnvioCode,
                    FormaPagoCode = source.FormaPagoCode,
                    TramiteCode = source.TramiteCode,
                    Observaciones1 = source.Observaciones1,
                    Observaciones2 = source.Observaciones2,
                    NotaCompraCode = source.NotaCompraCode,
                    UnidadNegocioCode = source.UnidadNegocioCode,
                    EnviarCorreoProveedor = source.EnviarCorreoProveedor,
                    Estado = EstadoOrdenCompra.Pendiente,
                    NoGravableCompra = source.NoGravableCompra,
                    ServicioCompra = source.ServicioCompra,
                    IgvExteriorCompra = source.IgvExteriorCompra
                };

                var item = 1;
                foreach (var d in source.Detalles.OrderBy(d => d.ItemNumber))
                {
                    copia.Detalles.Add(new OrdenCompraDetalle
                    {
                        PlantaCode = plantaCode,
                        OrdenCompraCode = nextCode,
                        ArticuloCode = d.ArticuloCode,
                        ItemNumber = item++,
                        CantidadArticulo = d.CantidadArticulo,
                        PrecioArticulo = d.PrecioArticulo,
                        DescuentoArticulo = d.DescuentoArticulo,
                        TotalArticulo = d.TotalArticulo,
                        SubCentroCostoCode = d.SubCentroCostoCode
                    });
                }

                await OrdenCompraTotalsCalculator.RecalculateAsync(_uow, copia, cancellationToken);
                await _uow.Logistica.Transacciones.OrdenesCompra.AddAsync(copia, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return OrdenCompraMapper.Map(copia);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Orden de Compra para la planta {plantaCode} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(string plantaCode, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .Where(o => o.PlantaCode == plantaCode && o.Code.StartsWith(CodePrefix))
                .OrderByDescending(o => o.Code)
                .Select(o => o.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode.Substring(CodePrefix.Length), out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return CodePrefix + next.ToString().PadLeft(CorrelativeLength, '0');
        }
    }
}