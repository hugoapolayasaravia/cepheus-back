using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente
{
    public class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateClienteCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PersonType)
                .IsInEnum().WithMessage("El tipo de persona no es válido.");

            RuleFor(x => x.DocumentTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                .MustAsync(DocumentTypeExists).WithMessage("El tipo de documento indicado no existe.");

            RuleFor(x => x.DocumentNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .MaximumLength(20).WithMessage("El número de documento no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueDocumentNumber).WithMessage("Ya existe un cliente con ese número de documento.");

            RuleFor(x => x.Name)
                .MaximumLength(120).WithMessage("El nombre no puede exceder los 120 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un cliente con ese nombre.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(100).WithMessage("La dirección no puede exceder los 100 caracteres.");

            RuleFor(x => x.UbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El ubigeo es obligatorio.")
                .Length(6).WithMessage("El código de ubigeo debe tener 6 caracteres.")
                .MustAsync(UbigeoExists).WithMessage("El ubigeo indicado no existe.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .MaximumLength(50).WithMessage("El teléfono no puede exceder los 50 caracteres.");

            RuleFor(x => x.ParentClientCode)
                .Cascade(CascadeMode.Stop)
                .Length(5).WithMessage("El código del cliente matriz debe tener 5 caracteres.")
                .MustAsync(ClienteExists).WithMessage("El cliente matriz indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentClientCode));

            RuleFor(x => x.TipoClienteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de cliente es obligatorio.")
                .MustAsync(TipoClienteExists).WithMessage("El tipo de cliente indicado no existe.");

            RuleFor(x => x.ClasificacionClienteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La clasificación de cliente es obligatoria.")
                .MustAsync(ClasificacionClienteExists).WithMessage("La clasificación de cliente indicada no existe.");

            RuleFor(x => x.LegalRepresentativeName).MaximumLength(50);
            RuleFor(x => x.LegalRepresentativePhone).MaximumLength(50);
            RuleFor(x => x.LegalRepresentativeDni).MaximumLength(8);

            RuleFor(x => x.ContactName)
                .NotEmpty().WithMessage("El nombre de contacto es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre de contacto no puede exceder los 50 caracteres.");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("El teléfono de contacto es obligatorio.")
                .MaximumLength(50).WithMessage("El teléfono de contacto no puede exceder los 50 caracteres.");

            RuleFor(x => x.ContactEmail)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El correo de contacto es obligatorio.")
                .MaximumLength(50).WithMessage("El correo de contacto no puede exceder los 50 caracteres.")
                .EmailAddress().WithMessage("El correo de contacto no tiene un formato válido.");

            RuleFor(x => x.Observations).MaximumLength(500);

            RuleFor(x => x.GlobalCreditAmount)
                .GreaterThanOrEqualTo(0).WithMessage("El monto de línea global no puede ser negativo.")
                .When(x => x.GlobalCreditAmount.HasValue);

            RuleFor(x => x.FormaPagoVentaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La forma de pago de ventas es obligatoria.")
                .Length(2).WithMessage("El código de forma de pago de ventas debe tener 2 caracteres.")
                .MustAsync(FormaPagoVentaExists).WithMessage("La forma de pago de ventas indicada no existe.");

            RuleFor(x => x.CurrencyCode)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(3).WithMessage("El código de moneda no puede exceder los 3 caracteres.")
                .MustAsync(CurrencyExists).WithMessage("La moneda indicada no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.CurrencyCode));
        }

        private async Task<bool> DocumentTypeExists(string code, CancellationToken ct)
            => await _uow.Comunes.TiposDocumento.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueDocumentNumber(string documentNumber, CancellationToken ct)
            => !await _uow.Facturacion.Maestros.Clientes.Query()
                .AnyAsync(c => c.DocumentNumber == documentNumber.Trim(), ct);

        private async Task<bool> BeUniqueName(string? name, CancellationToken ct)
            => !await _uow.Facturacion.Maestros.Clientes.Query()
                .AnyAsync(c => c.Name != null && c.Name.ToLower() == name!.Trim().ToLower(), ct);

        private async Task<bool> UbigeoExists(string code, CancellationToken ct)
            => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == code.Trim(), ct);

        private async Task<bool> ClienteExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Clientes.Query().AnyAsync(c => c.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> TipoClienteExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.TiposCliente.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ClasificacionClienteExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.ClasificacionesCliente.Query().AnyAsync(c => c.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> FormaPagoVentaExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.FormasPagoVenta.Query().AnyAsync(f => f.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> CurrencyExists(string? code, CancellationToken ct)
            => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == code!.Trim().ToUpper(), ct);
    }
}
