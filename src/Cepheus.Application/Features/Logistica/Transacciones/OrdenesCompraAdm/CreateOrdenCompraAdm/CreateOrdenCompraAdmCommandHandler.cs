// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/CreateOrdenCompraAdministracion/CreateOrdenCompraAdministracionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.CreateOrdenCompraAdm
{
    public class CreateOrdenCompraAdmCommandHandler
        : IRequestHandler<CreateOrdenCompraAdmCommand, OrdenCompraAdmResponse>
    {
        private const int MaxConcurrencyRetries = 3;

        private readonly IUnitOfWork _uow;

        public CreateOrdenCompraAdmCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraAdmResponse> Handle(
            CreateOrdenCompraAdmCommand request, CancellationToken cancellationToken)
        {
            var motivo = System.Enum.Parse<MotivoOrdenCompraAdministracion>(request.Motivo, true);

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(cancellationToken);

                var orden = new OrdenCompraAdministracion
                {
                    Code = nextCode,
                    ComprobantePagoCode = request.ComprobantePagoCode.Trim().ToUpperInvariant(),
                    EmitirDocumento = request.EmitirDocumento?.Trim(),
                    FechaEntrega = request.FechaEntrega,
                    ProveedorCode = request.ProveedorCode.Trim().ToUpperInvariant(),
                    CompradorCode = request.CompradorCode.Trim().ToUpperInvariant(),
                    MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                    LugarEnvioCode = request.LugarEnvioCode.Trim().ToUpperInvariant(),
                    FormaPagoCode = request.FormaPagoCode.Trim().ToUpperInvariant(),
                    TramiteCode = request.TramiteCode.Trim().ToUpperInvariant(),
                    Observaciones1 = request.Observaciones1?.Trim(),
                    Observaciones2 = request.Observaciones2?.Trim(),
                    NotaCompraCode = string.IsNullOrWhiteSpace(request.NotaCompraCode)
                        ? null
                        : request.NotaCompraCode.Trim().ToUpperInvariant(),
                    UnidadNegocioCode = request.UnidadNegocioCode.Trim().ToUpperInvariant(),
                    Motivo = motivo,
                    Estado = EstadoOrdenCompraAdministracion.Pendiente,
                    NoGravableCompra = request.NoGravableCompra,
                    ServicioCompra = request.ServicioCompra,
                    IgvExteriorCompra = request.IgvExteriorCompra
                };

                var item = 1;
                foreach (var line in request.Detalles)
                {
                    orden.Detalles.Add(new OrdenCompraAdministracionDetalle
                    {
                        OrdenCompraAdministracionCode = nextCode,
                        ItemNumber = item++,
                        Glosa = line.Glosa.Trim(),
                        UnidadMedidaCode = line.UnidadMedidaCode.Trim().ToUpperInvariant(),
                        Cantidad = line.Cantidad,
                        Precio = line.Precio,
                        Descuento = line.Descuento,
                        Total = OrdenCompraAdmTotalsCalculator.CalculateLineTotal(
                            line.Cantidad, line.Precio, line.Descuento),
                        SubCentroCostoCode = line.SubCentroCostoCode.Trim().ToUpperInvariant(),
                        CantidadEntregada = 0
                    });
                }

                await OrdenCompraAdmTotalsCalculator.RecalculateAsync(_uow, orden, cancellationToken);

                await _uow.Logistica.Transacciones.OrdenesCompraAdministracion.AddAsync(orden, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return OrdenCompraAdmMapper.Map(orden);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    _uow.ClearTracking();
                }
            }

            throw new InvalidOperationException(
                "No se pudo generar el correlativo de la Orden de Compra de Administración " +
                "por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Logistica.Transacciones.OrdenesCompraAdministracion.Query()
                .AsNoTracking()
                .OrderByDescending(o => o.Code)
                .Select(o => o.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return next.ToString().PadLeft(OrdenCompraAdmRules.CorrelativeLength, '0');
        }
    }
}
