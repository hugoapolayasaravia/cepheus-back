using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.CreateNotaDebitoVenta
{
    public class CreateNotaDebitoVentaCommandValidator : AbstractValidator<CreateNotaDebitoVentaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNotaDebitoVentaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.ExecutingPlantaCode)
                .MustAsync(PlantaExists).WithMessage("La planta ejecutora indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ExecutingPlantaCode));

            RuleFor(x => x.IssueDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La fecha de emisión es obligatoria.")
                .Must(d => d.Date <= DateTime.Today).WithMessage("La fecha de emisión no puede ser futura.");

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

            RuleFor(x => x.DocumentTypeCode)
                .MaximumLength(2).WithMessage("El tipo de documento debe tener 2 caracteres.");

            RuleFor(x => x.DocumentNumber)
                .MaximumLength(10).WithMessage("El número del documento no puede exceder los 10 caracteres.");

            RuleFor(x => x.Details)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Debe registrar al menos una línea de detalle.")
                .Must(d => d.Count <= NotaDebitoVentaRules.MaxLines)
                    .WithMessage($"El detalle no puede exceder las {NotaDebitoVentaRules.MaxLines} líneas.")
                .Must(NotaDebitoVentaDetailsValidation.NoDuplicateLines)
                    .WithMessage("Hay líneas duplicadas (mismo producto y clave de línea).")
                .MustAsync((lines, ct) => NotaDebitoVentaDetailsValidation.ProductsExistAsync(_uow, lines, ct))
                    .WithMessage("Alguno de los productos indicados no existe.");

            RuleForEach(x => x.Details).SetValidator(new NotaDebitoVentaDetalleInputValidator());
        }

        private async Task<bool> PlantaExists(string? code, CancellationToken cancellationToken)
        {
            var c = NotaDebitoVentaRules.Normalize(code!);
            return await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == c, cancellationToken);
        }

        private async Task<bool> ClienteExists(string code, CancellationToken cancellationToken)
        {
            var c = code.Trim();
            return await _uow.Facturacion.Maestros.Clientes.Query().AnyAsync(x => x.Code == c, cancellationToken);
        }

        private async Task<bool> ObraExists(CreateNotaDebitoVentaCommand cmd, CancellationToken cancellationToken)
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
            var c = NotaDebitoVentaRules.Normalize(code);
            return await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == c && m.IsActive, cancellationToken);
        }
    }
}
