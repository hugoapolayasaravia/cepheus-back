using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common
{
    /// <summary>
    /// Reglas comunes de cabecera, detalle y guías de la solicitud (Create y Update).
    /// </summary>
    public abstract class SolicitudNotaCreditoInputValidator<T> : AbstractValidator<T>
        where T : ISolicitudNotaCreditoInput
    {
        protected readonly IUnitOfWork Uow;

        protected SolicitudNotaCreditoInputValidator(IUnitOfWork uow)
        {
            Uow = uow;

            RuleFor(x => x.NegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El negocio es obligatorio.")
                .MustAsync(NegocioExists).WithMessage("El negocio indicado no existe.");

            RuleFor(x => x.IssueDate)
                .NotEmpty().WithMessage("La fecha de emisión es obligatoria.");

            RuleFor(x => x.ClienteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El cliente es obligatorio.")
                .MustAsync(ClienteExists).WithMessage("El cliente indicado no existe.");

            RuleFor(x => x.DocumentTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de comprobante es obligatorio.")
                .MustAsync(ComprobanteExists).WithMessage("El tipo de comprobante indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número del comprobante es obligatorio.")
                .MaximumLength(10).WithMessage("El número del comprobante no puede exceder los 10 caracteres.");

            RuleFor(x => x.MotivoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El motivo es obligatorio.")
                .MustAsync(MotivoExistsAndActive).WithMessage("El motivo indicado no existe o está inactivo.");

            RuleFor(x => x)
                .MustAsync(MotivoIsCurrent)
                .WithMessage("El motivo indicado no está vigente a la fecha de emisión.")
                .WithName("MotivoCode")
                .When(x => !string.IsNullOrWhiteSpace(x.MotivoCode));

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(MonedaExistsAndActive).WithMessage("La moneda indicada no existe o está inactiva.");

            RuleFor(x => x.Observation)
                .NotEmpty().WithMessage("La observación es obligatoria.")
                .MaximumLength(200).WithMessage("La observación no puede exceder los 200 caracteres.");

            RuleFor(x => x.Details)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Debe registrar al menos una línea de detalle.")
                .Must(d => d.Count <= SolicitudNotaCreditoRules.MaxLines)
                    .WithMessage($"El detalle no puede exceder las {SolicitudNotaCreditoRules.MaxLines} líneas.")
                .Must(NoDuplicateLines)
                    .WithMessage("Hay líneas duplicadas (mismo producto y clave de línea).")
                .MustAsync(ProductsExist)
                    .WithMessage("Alguno de los productos indicados no existe.");

            RuleForEach(x => x.Details).ChildRules(line =>
            {
                line.RuleFor(l => l.TipoProductoCode)
                    .NotEmpty().WithMessage("El tipo de producto de la línea es obligatorio.")
                    .MaximumLength(2).WithMessage("El tipo de producto debe tener 2 caracteres.");

                line.RuleFor(l => l.ProductoCode)
                    .NotEmpty().WithMessage("El producto de la línea es obligatorio.")
                    .MaximumLength(4).WithMessage("El código de producto debe tener 4 caracteres.");

                line.RuleFor(l => l.Quantity)
                    .GreaterThan(0).WithMessage("La cantidad de la línea debe ser mayor a cero.");

                line.RuleFor(l => l.UnitPrice)
                    .GreaterThanOrEqualTo(0).WithMessage("El precio de la línea no puede ser negativo.");

                line.RuleFor(l => l.LineKey)
                    .GreaterThan(0).WithMessage("La clave de línea debe ser mayor a cero.")
                    .When(l => l.LineKey.HasValue);

                line.RuleFor(l => l.OrderCode)
                    .MaximumLength(8).WithMessage("La orden de trabajo no puede exceder los 8 caracteres.");
            });

            RuleFor(x => x.Guides!)
                .Must(g => g.Select(i => i.GuideNumber?.Trim()).Distinct().Count() == g.Count)
                .WithMessage("Hay guías de venta repetidas.")
                .WithName("Guides")
                .When(x => x.Guides is not null);

            RuleForEach(x => x.Guides!).ChildRules(guide =>
            {
                guide.RuleFor(g => g.GuideNumber)
                    .NotEmpty().WithMessage("El número de guía es obligatorio.")
                    .MaximumLength(10).WithMessage("El número de guía no puede exceder los 10 caracteres.");

                guide.RuleFor(g => g.Quantity)
                    .GreaterThan(0).WithMessage("La cantidad de la guía debe ser mayor a cero.");
            }).When(x => x.Guides is not null);
        }

        private async Task<bool> NegocioExists(string code, CancellationToken cancellationToken)
        {
            var c = SolicitudNotaCreditoRules.Normalize(code);
            return await Uow.Comunes.Negocios.Query().AnyAsync(n => n.Code == c, cancellationToken);
        }

        private async Task<bool> ClienteExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await Uow.Facturacion.Maestros.Clientes.Query().AnyAsync(x => x.Code == c, cancellationToken);
        }

        private async Task<bool> ComprobanteExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await Uow.Comunes.ComprobantesPago.Query().AnyAsync(x => x.Code == c, cancellationToken);
        }

        private async Task<bool> MotivoExistsAndActive(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await Uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AnyAsync(x => x.Code == c && x.IsActive, cancellationToken);
        }

        private async Task<bool> MotivoIsCurrent(T input, CancellationToken cancellationToken)
        {
            var c = input.MotivoCode.Trim();
            var date = input.IssueDate.Date;

            var motivo = await Uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AsNoTracking()
                .Where(x => x.Code == c)
                .Select(x => new { x.StartDate, x.EndDate })
                .FirstOrDefaultAsync(cancellationToken);

            if (motivo is null)
            {
                return true; // la regla de existencia ya informa el error
            }

            return (motivo.StartDate == null || motivo.StartDate.Value.Date <= date)
                && (motivo.EndDate == null || motivo.EndDate.Value.Date >= date);
        }

        private async Task<bool> MonedaExistsAndActive(string code, CancellationToken cancellationToken)
        {
            var c = SolicitudNotaCreditoRules.Normalize(code);
            return await Uow.Comunes.Monedas.Query().AnyAsync(x => x.Code == c && x.IsActive, cancellationToken);
        }

        private static bool NoDuplicateLines(List<SolicitudNotaCreditoDetalleInput> lines)
        {
            var keys = lines
                .Select((l, i) => $"{l.TipoProductoCode?.Trim()}|{l.ProductoCode?.Trim()}|{l.LineKey ?? (i + 1)}")
                .ToList();

            return keys.Distinct().Count() == keys.Count;
        }

        private async Task<bool> ProductsExist(List<SolicitudNotaCreditoDetalleInput> lines, CancellationToken cancellationToken)
        {
            var pairs = lines
                .Where(l => !string.IsNullOrWhiteSpace(l.TipoProductoCode) && !string.IsNullOrWhiteSpace(l.ProductoCode))
                .Select(l => (Tipo: l.TipoProductoCode.Trim(), Producto: l.ProductoCode.Trim()))
                .Distinct()
                .ToList();

            foreach (var (tipo, producto) in pairs)
            {
                var exists = await Uow.Facturacion.Maestros.Productos.Query()
                    .AnyAsync(p => p.TipoProductoCode == tipo && p.Code == producto, cancellationToken);

                if (!exists)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
