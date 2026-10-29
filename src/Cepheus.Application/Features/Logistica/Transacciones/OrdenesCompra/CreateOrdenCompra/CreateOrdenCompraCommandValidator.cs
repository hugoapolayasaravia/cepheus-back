using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CreateOrdenCompra
{
    public class CreateOrdenCompraCommandValidator
        : AbstractValidator<CreateOrdenCompraCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateOrdenCompraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Comunes.Plantas.Query()
                        .AnyAsync(
                            p => p.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("La planta indicada no existe.");

            RuleFor(x => x.TipoCompraCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Catalogos.TiposCompra.Query()
                        .AnyAsync(
                            t => t.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("El tipo de compra indicado no existe.");

            RuleFor(x => x.ComprobantePagoCode)
                .MustAsync(async (code, ct) =>
                    await _uow.Comunes.ComprobantesPago
                        .Query()
                        .AnyAsync(c => c.Code == code, ct))
                .WithMessage("El comprobante de pago indicado no existe.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(x.ComprobantePagoCode));

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Maestros.Proveedores.Query()
                        .AnyAsync(
                            p => p.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.CompradorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Catalogos.Compradores.Query()
                        .AnyAsync(
                            p => p.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("El comprador indicado no existe.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Comunes.Monedas.Query()
                        .AnyAsync(
                            m => m.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.LugarEnvioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Catalogos.LugaresEnvio.Query()
                        .AnyAsync(
                            l => l.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("El lugar de envío indicado no existe.");

            RuleFor(x => x.FormaPagoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Catalogos.FormasPago.Query()
                        .AnyAsync(
                            f => f.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.TramiteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Catalogos.Tramites.Query()
                        .AnyAsync(
                            t => t.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage("El trámite indicado no existe.");

            RuleFor(x => x.NotaCompraCode)
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Catalogos.NotasCompra
                        .Query()
                        .AnyAsync(
                            n => n.Code == c!.Trim().ToUpper(),
                            ct))
                .WithMessage("La nota indicada no existe.")
                .When(x =>
                    !string.IsNullOrWhiteSpace(x.NotaCompraCode));

            RuleFor(x => x.UnidadNegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                        .AnyAsync(
                            u => u.Code == c.Trim().ToUpper(),
                            ct))
                .WithMessage(
                    "La unidad de negocio indicada no existe.");

            RuleFor(x => x.FechaEntrega)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage(
                    "La fecha de entrega no puede ser anterior a la fecha del sistema.");

            RuleFor(x => x.Detalles)
                .NotEmpty()
                .WithMessage(
                    "La Orden de Compra debe tener al menos una línea.");

            RuleForEach(x => x.Detalles)
                .ChildRules(line =>
                {
                    line.RuleFor(l => l.ArticuloCode)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .MustAsync(async (c, ct) =>
                            await _uow.Logistica.Maestros.Articulos.Query()
                                .AnyAsync(
                                    a => a.Code == c.Trim().ToUpper(),
                                    ct))
                        .WithMessage(
                            "El artículo indicado no existe.");

                    line.RuleFor(l => l.CantidadArticulo)
                        .GreaterThan(0)
                        .WithMessage(
                            "La cantidad debe ser mayor a 0.");

                    line.RuleFor(l => l.PrecioArticulo)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage(
                            "El precio no puede ser negativo.");

                    line.RuleFor(l => l.DescuentoArticulo)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage(
                            "El descuento no puede ser negativo.");

                    line.RuleFor(l => l.SubCentroCostoCode)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .MustAsync(async (c, ct) =>
                            await _uow.Logistica.Maestros.SubCentrosCosto
                                .Query()
                                .AnyAsync(
                                    s => s.Code == c.Trim().ToUpper(),
                                    ct))
                        .WithMessage(
                            "El subcentro de costo indicado no existe.");

                    line.RuleForEach(l => l.Origenes)
                        .ChildRules(o =>
                        {
                            o.RuleFor(x => x.PedidoCode)
                                .Cascade(CascadeMode.Stop)
                                .NotEmpty()
                                .WithMessage(
                                    "El pedido de origen es obligatorio.");

                            o.RuleFor(x => x.PedidoItemNumber)
                                .GreaterThan(0)
                                .WithMessage(
                                    "El número de línea debe ser mayor a 0.");

                            o.RuleFor(x => x.CantidadTomada)
                                .GreaterThan(0)
                                .WithMessage(
                                    "La cantidad tomada debe ser mayor a 0.");
                        });
                });

            RuleFor(x => x)
                .MustAsync(PedidoOrigenesExist)
                .WithMessage(
                    "Alguna línea de Pedido de origen no existe en esa planta.");

            RuleFor(x => x)
                .MustAsync(PedidoCantidadesValidas)
                .WithMessage(
                    "La cantidad tomada supera la cantidad disponible del Pedido.");
        }

        private async Task<bool> PedidoOrigenesExist(
            CreateOrdenCompraCommand cmd,
            CancellationToken ct)
        {
            if (cmd.Detalles is null || cmd.Detalles.Count == 0)
                return true;

            var plantaCode = cmd.PlantaCode
                .Trim()
                .ToUpperInvariant();

            foreach (var line in cmd.Detalles)
            {
                if (line.Origenes is null || line.Origenes.Count == 0)
                    continue;

                foreach (var origen in line.Origenes)
                {
                    var pedidoCode = origen.PedidoCode
                        .Trim()
                        .ToUpperInvariant();

                    var exists =
                        await _uow.Logistica.Transacciones.PedidoDetalles
                            .Query()
                            .AnyAsync(
                                pd =>
                                    pd.PlantaCode == plantaCode &&
                                    pd.PedidoCode == pedidoCode &&
                                    pd.ItemNumber ==
                                        origen.PedidoItemNumber,
                                ct);

                    if (!exists)
                        return false;
                }
            }

            return true;
        }

        private async Task<bool> PedidoCantidadesValidas(
            CreateOrdenCompraCommand cmd,
            CancellationToken ct)
        {
            if (cmd.Detalles is null || cmd.Detalles.Count == 0)
                return true;

            var plantaCode = cmd.PlantaCode
                .Trim()
                .ToUpperInvariant();

            var cantidadesSolicitadas = cmd.Detalles
                .Where(d => d.Origenes is { Count: > 0 })
                .SelectMany(d => d.Origenes!)
                .GroupBy(o => new
                {
                    PedidoCode = o.PedidoCode
                        .Trim()
                        .ToUpperInvariant(),

                    o.PedidoItemNumber
                })
                .Select(g => new
                {
                    g.Key.PedidoCode,
                    g.Key.PedidoItemNumber,
                    CantidadTomada =
                        g.Sum(x => x.CantidadTomada)
                });

            foreach (var item in cantidadesSolicitadas)
            {
                var pedidoDetalle =
                    await _uow.Logistica.Transacciones.PedidoDetalles
                        .Query()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            pd =>
                                pd.PlantaCode == plantaCode &&
                                pd.PedidoCode == item.PedidoCode &&
                                pd.ItemNumber ==
                                    item.PedidoItemNumber,
                            ct);

                if (pedidoDetalle is null)
                    return false;

                var cantidadDisponible =
                    pedidoDetalle.CantidadArticulo -
                    pedidoDetalle.CantidadEnCompra;

                if (item.CantidadTomada > cantidadDisponible)
                    return false;
            }

            return true;
        }
    }
}