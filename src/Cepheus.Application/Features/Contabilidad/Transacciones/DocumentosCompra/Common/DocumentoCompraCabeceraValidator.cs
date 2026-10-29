using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common
{
    public abstract class DocumentoCompraCabeceraValidator<T> : AbstractValidator<T>
        where T : IDocumentoCompraCabecera
    {
        protected DocumentoCompraCabeceraValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.ComprobantePagoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Comunes.ComprobantesPago.Query()
                        .AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El comprobante de pago indicado no existe.");

            RuleFor(x => x.NumeroDocumento)
                .NotEmpty()
                .MaximumLength(15);

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Maestros.Proveedores.Query()
                        .AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El proveedor indicado no existe.");

            RuleFor(x => x.FechaVencimiento)
                .GreaterThanOrEqualTo(x => x.FechaEmision)
                .WithMessage("La fecha de vencimiento no puede ser anterior a la fecha de emisión.");

            RuleFor(x => x.FormaPagoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Catalogos.FormasPago.Query()
                        .AnyAsync(f => f.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) =>
                    await uow.Comunes.Monedas.Query()
                        .AnyAsync(m => m.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La moneda indicada no existe.");

            // Moneda distinta de soles: la fecha del tipo de cambio es obligatoria y debe existir en Comunes.
            RuleFor(x => x.TipoCambioFecha)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("La fecha del tipo de cambio es obligatoria para moneda extranjera.")
                .MustAsync(async (x, f, ct) =>
                {
                    var moneda = x.MonedaCode.Trim().ToUpper();
                    return await uow.Comunes.TiposCambio.Query()
                        .AnyAsync(t => t.MonedaCode == moneda && t.Date == f!.Value, ct);
                })
                .WithMessage("No existe tipo de cambio registrado para la moneda y la fecha indicadas.")
                .When(x => !string.Equals(x.MonedaCode?.Trim(), DocumentoCompraRules.MonedaSoles, StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.NoGravableSol).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ServicioSol).GreaterThanOrEqualTo(0);
            RuleFor(x => x.IgvExteriorSol).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MontoDetraccionSol).GreaterThanOrEqualTo(0);

            RuleFor(x => x.Glosa).NotEmpty().MaximumLength(50);

            RuleFor(x => x.Origen)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(o => System.Enum.TryParse<OrigenDocumentoCompra>(o, true, out var v) && System.Enum.IsDefined(v))
                .WithMessage("El origen indicado no es válido.");

            RuleFor(x => x.Procedencia)
                .Must(p => System.Enum.TryParse<ProcedenciaDocumentoCompra>(p, true, out var v) && System.Enum.IsDefined(v))
                .WithMessage("La procedencia indicada no es válida.")
                .When(x => !string.IsNullOrWhiteSpace(x.Procedencia));

            RuleFor(x => x.TipoBienCode)
                .MustAsync(async (c, ct) =>
                    await uow.Facturacion.Catalogos.TiposBien.Query()
                        .AnyAsync(t => t.Code == c!.Trim().ToUpper(), ct))
                .WithMessage("El tipo de bien indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.TipoBienCode));

            RuleFor(x => x.TipoOperacionCode)
                .MustAsync(async (c, ct) =>
                    await uow.Facturacion.Catalogos.TiposOperacion.Query()
                        .AnyAsync(t => t.Code == c!.Trim().ToUpper(), ct))
                .WithMessage("El tipo de operación indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.TipoOperacionCode));

            RuleFor(x => x.OperacionDetraccion).MaximumLength(10);
            RuleFor(x => x.ConstanciaDetraccion).MaximumLength(10);
            RuleFor(x => x.NumeroDetraccion).MaximumLength(10);

            // Documento origen: ambos campos juntos y debe existir para el mismo proveedor.
            RuleFor(x => x)
                .Must(x => string.IsNullOrWhiteSpace(x.DocumentoOrigenComprobantePagoCode)
                           == string.IsNullOrWhiteSpace(x.DocumentoOrigenNumero))
                .WithMessage("El documento origen requiere tipo de comprobante y número.")
                .OverridePropertyName("DocumentoOrigen");

            RuleFor(x => x)
                .MustAsync(async (x, ct) =>
                {
                    var tdo = x.DocumentoOrigenComprobantePagoCode!.Trim().ToUpper();
                    var num = x.DocumentoOrigenNumero!.Trim();
                    var prv = x.ProveedorCode.Trim().ToUpper();
                    return await uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                        .AnyAsync(d => d.ComprobantePagoCode == tdo && d.NumeroDocumento == num && d.ProveedorCode == prv, ct);
                })
                .WithMessage("El documento origen no existe para este proveedor.")
                .OverridePropertyName("DocumentoOrigen")
                .When(x => !string.IsNullOrWhiteSpace(x.DocumentoOrigenComprobantePagoCode)
                           && !string.IsNullOrWhiteSpace(x.DocumentoOrigenNumero));

            RuleFor(x => x.OrdenCompraAdministracionCode)
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Transacciones.OrdenesCompraAdministracion.Query()
                        .AnyAsync(o => o.Code == c!.Trim().ToUpper(), ct))
                .WithMessage("La orden de compra de administración indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.OrdenCompraAdministracionCode));
        }
    }
}
