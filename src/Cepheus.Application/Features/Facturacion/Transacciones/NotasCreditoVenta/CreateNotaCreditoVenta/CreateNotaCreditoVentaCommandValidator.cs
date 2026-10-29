using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.CreateNotaCreditoVenta
{
    public class CreateNotaCreditoVentaCommandValidator : AbstractValidator<CreateNotaCreditoVentaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNotaCreditoVentaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.ExecutingPlantaCode)
                .MustAsync(PlantaExists).WithMessage("La planta ejecutora indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ExecutingPlantaCode));

            RuleFor(x => x.IssueDate).NotEmpty().WithMessage("La fecha de emisión es obligatoria.");

            RuleFor(x => x.ClienteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El cliente es obligatorio.")
                .MustAsync(ClienteExists).WithMessage("El cliente indicado no existe.");

            RuleFor(x => x.ObraCode)
                .NotEmpty().WithMessage("La obra es obligatoria.")
                .MaximumLength(3).WithMessage("El código de obra debe tener 3 caracteres.");

            RuleFor(x => x)
                .MustAsync(ObraExists)
                .WithMessage("La obra indicada no existe para el cliente.")
                .WithName("ObraCode")
                .When(x => !string.IsNullOrWhiteSpace(x.ClienteCode) && !string.IsNullOrWhiteSpace(x.ObraCode));

            RuleFor(x => x.VendedorCode)
                .MustAsync(VendedorExists).WithMessage("El vendedor indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.VendedorCode));

            RuleFor(x => x.FormaPagoVentaCode)
                .MustAsync(FormaPagoExists).WithMessage("La forma de pago indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.FormaPagoVentaCode));

            RuleFor(x => x.DeliveryAddress)
                .MaximumLength(100).WithMessage("La dirección de entrega no puede exceder los 100 caracteres.");

            RuleFor(x => x.DeliveryUbigeoCode)
                .MustAsync(UbigeoExists).WithMessage("El ubigeo de entrega indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.DeliveryUbigeoCode));

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(MonedaExists).WithMessage("La moneda indicada no existe o está inactiva.");

            RuleFor(x => x.OrderCode)
                .MaximumLength(8).WithMessage("La orden de pedido no puede exceder los 8 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La glosa es obligatoria.")
                .MaximumLength(200).WithMessage("La glosa no puede exceder los 200 caracteres.");

            RuleFor(x => x.Type)
                .Must(t => NotaCreditoVentaRules.AllowedTypes.Contains(t!.Trim().ToUpperInvariant()))
                .WithMessage($"El tipo debe ser uno de: {string.Join(", ", NotaCreditoVentaRules.AllowedTypes)}.")
                .When(x => !string.IsNullOrWhiteSpace(x.Type));

            RuleFor(x => x.DocumentTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de comprobante es obligatorio.")
                .MustAsync(ComprobanteExists).WithMessage("El tipo de comprobante indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número del comprobante es obligatorio.")
                .MaximumLength(10).WithMessage("El número del comprobante no puede exceder los 10 caracteres.");

            RuleFor(x => x.MotivoDevolucionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El motivo de devolución es obligatorio.")
                .MustAsync(MotivoDevolucionExists).WithMessage("El motivo de devolución indicado no existe.");

            RuleFor(x => x.CashDocument)
                .MaximumLength(10).WithMessage("El documento de caja no puede exceder los 10 caracteres.");

            RuleFor(x => x.Details)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Debe registrar al menos una línea de detalle.")
                .Must(d => d.Count <= NotaCreditoVentaRules.MaxLines)
                    .WithMessage($"El detalle no puede exceder las {NotaCreditoVentaRules.MaxLines} líneas.")
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

                line.RuleFor(l => l.GuideNumber)
                    .MaximumLength(10).WithMessage("El número de guía no puede exceder los 10 caracteres.");

                line.RuleFor(l => l.Description)
                    .MaximumLength(255).WithMessage("La descripción de la línea no puede exceder los 255 caracteres.");

                line.RuleFor(l => l.LineKey)
                    .GreaterThan(0).WithMessage("La clave de línea debe ser mayor a cero.")
                    .When(l => l.LineKey.HasValue);

                line.RuleFor(l => l.OrderCode)
                    .MaximumLength(8).WithMessage("La orden de trabajo no puede exceder los 8 caracteres.");
            });
        }

        private async Task<bool> PlantaExists(string? code, CancellationToken cancellationToken)
        {
            var c = NotaCreditoVentaRules.Normalize(code!);
            return await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == c, cancellationToken);
        }

        private async Task<bool> ClienteExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await _uow.Facturacion.Maestros.Clientes.Query().AnyAsync(x => x.Code == c, cancellationToken);
        }

        private async Task<bool> ObraExists(CreateNotaCreditoVentaCommand cmd, CancellationToken cancellationToken)
        {
            var cliente = cmd.ClienteCode.Trim();
            var obra = cmd.ObraCode.Trim();
            return await _uow.Facturacion.Maestros.Obras.Query()
                .AnyAsync(o => o.ClienteCode == cliente && o.Code == obra, cancellationToken);
        }

        private async Task<bool> VendedorExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Facturacion.Maestros.Vendedores.Query().AnyAsync(v => v.Code == c, cancellationToken);
        }

        private async Task<bool> FormaPagoExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Facturacion.Catalogos.FormasPagoVenta.Query().AnyAsync(f => f.Code == c, cancellationToken);
        }

        private async Task<bool> UbigeoExists(string? code, CancellationToken cancellationToken)
        {
            var c = code!.Trim();
            return await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == c, cancellationToken);
        }

        private async Task<bool> MonedaExists(string code, CancellationToken cancellationToken)
        {
            var c = NotaCreditoVentaRules.Normalize(code);
            return await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == c && m.IsActive, cancellationToken);
        }

        private async Task<bool> ComprobanteExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await _uow.Comunes.ComprobantesPago.Query().AnyAsync(x => x.Code == c, cancellationToken);
        }

        private async Task<bool> MotivoDevolucionExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await _uow.Logistica.Catalogos.MotivosDevolucionArticulo.Query().AnyAsync(m => m.Code == c, cancellationToken);
        }

        private static bool NoDuplicateLines(List<NotaCreditoVentaDetalleInput> lines)
        {
            var keys = lines
                .Select((l, i) => $"{l.TipoProductoCode?.Trim()}|{l.ProductoCode?.Trim()}|{l.LineKey ?? (i + 1)}")
                .ToList();

            return keys.Distinct().Count() == keys.Count;
        }

        private async Task<bool> ProductsExist(List<NotaCreditoVentaDetalleInput> lines, CancellationToken cancellationToken)
        {
            var pairs = lines
                .Where(l => !string.IsNullOrWhiteSpace(l.TipoProductoCode) && !string.IsNullOrWhiteSpace(l.ProductoCode))
                .Select(l => (Tipo: l.TipoProductoCode.Trim(), Producto: l.ProductoCode.Trim()))
                .Distinct()
                .ToList();

            foreach (var (tipo, producto) in pairs)
            {
                var exists = await _uow.Facturacion.Maestros.Productos.Query()
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
