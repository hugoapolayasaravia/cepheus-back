using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.CreateCotizacion
{
    public class CreateCotizacionCommandValidator : AbstractValidator<CreateCotizacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El negocio es obligatorio.")
                .MustAsync(NegocioExists).WithMessage("El negocio indicado no existe.");

            RuleFor(x => x.VendedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El vendedor es obligatorio.")
                .MustAsync(VendedorExists).WithMessage("El vendedor indicado no existe.");

            RuleFor(x => x.Date).NotEmpty().WithMessage("La fecha de la cotización es obligatoria.");

            RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(1);

            RuleFor(x => x.FormaPagoVentaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La forma de pago es obligatoria.")
                .MustAsync(FormaPagoExists).WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.TecnicoCode)
                .MustAsync(TecnicoExists).WithMessage("El técnico indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.TecnicoCode));

            RuleFor(x => x.Type)
                .Must(t => System.Enum.TryParse<TipoCotizacion>(t, true, out _))
                .WithMessage($"El tipo de cotización debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(TipoCotizacion)))}.");

            RuleFor(x => x.MetradoCalculationSystem)
                .Must(v => System.Enum.TryParse<SistemaCalculoMetrado>(v, true, out _))
                .WithMessage($"El sistema de cálculo de metrado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(SistemaCalculoMetrado)))}.")
                .When(x => !string.IsNullOrWhiteSpace(x.MetradoCalculationSystem));

            RuleFor(x => x.MetradoCalculationSystem)
                .NotEmpty().WithMessage("Para el negocio PT debe indicar el sistema de cálculo de metrado (Plantilla a Utilizar).")
                .When(x => x.NegocioCode.Trim().ToUpper() == "PT");

            RuleFor(x => x.WorkDurationMonths).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.GlobalVolume).GreaterThanOrEqualTo(0);
            RuleFor(x => x.IgvRate).GreaterThanOrEqualTo(0);

            RuleFor(x => x.ClienteCode)
                .MustAsync(ClienteExists).WithMessage("El cliente indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ClienteCode));

            RuleFor(x => x.ClientName).NotEmpty().WithMessage("El nombre del cliente es obligatorio.").MaximumLength(150);

            RuleFor(x => x.ClientAddressUbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El ubigeo de la dirección del cliente es obligatorio.")
                .MustAsync(UbigeoExists).WithMessage("El ubigeo de la dirección del cliente no existe.");

            RuleFor(x => x)
                .MustAsync(ObraExists)
                .WithMessage("La obra indicada no existe para el cliente indicado.")
                .When(x => !string.IsNullOrWhiteSpace(x.ObraCode));

            RuleFor(x => x.ObraCode)
                .Empty()
                .WithMessage("Para indicar una obra debe indicar primero el cliente.")
                .When(x => string.IsNullOrWhiteSpace(x.ClienteCode) && !string.IsNullOrWhiteSpace(x.ObraCode));

            RuleFor(x => x.WorkName).NotEmpty().WithMessage("El nombre de la obra es obligatorio.").MaximumLength(150);

            RuleFor(x => x.ProjectStatus)
                .Must(s => System.Enum.TryParse<EstadoProyectoCotizacion>(s, true, out _))
                .WithMessage($"El estado del proyecto debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoProyectoCotizacion)))}.");

            RuleFor(x => x.WorkAddressUbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El ubigeo de la dirección de la obra es obligatorio.")
                .MustAsync(UbigeoExists).WithMessage("El ubigeo de la dirección de la obra no existe.");

            RuleFor(x => x.WorkAddress).NotEmpty().WithMessage("La dirección de la obra es obligatoria.").MaximumLength(150);

            RuleFor(x => x.ContactName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ContactPhone).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().WithMessage("El correo de contacto no es válido.").MaximumLength(50);

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || !x.StartDate.HasValue || x.EndDate >= x.StartDate)
                .WithMessage("La fecha de término de obra no puede ser anterior a la fecha de inicio.");

            RuleFor(x => x.FleteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La Zona es obligatorio.")
                .MustAsync(FleteExists).WithMessage("El flete / Zona indicado no existe.");
        }

        private async Task<bool> NegocioExists(string code, CancellationToken ct)
            => await _uow.Comunes.Negocios.Query().AnyAsync(n => n.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> VendedorExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Vendedores.Query().AnyAsync(v => v.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> FormaPagoExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.FormasPagoVenta.Query().AnyAsync(f => f.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> TecnicoExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Tecnicos.Query().AnyAsync(t => t.TrabajadorCode == code!.Trim().ToUpper(), ct);

        private async Task<bool> ClienteExists(string? code, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Clientes.Query().AnyAsync(c => c.Code == code!.Trim().ToUpper(), ct);

        private async Task<bool> UbigeoExists(string code, CancellationToken ct)
            => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == code.Trim(), ct);

        private async Task<bool> ObraExists(CreateCotizacionCommand command, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Obras.Query()
                .AnyAsync(o => o.ClienteCode == command.ClienteCode!.Trim().ToUpper()
                             && o.Code == command.ObraCode!.Trim().ToUpper(), ct);

        private async Task<bool> FleteExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.Fletes.Query().AnyAsync(f => f.Code == code.Trim().ToUpper(), ct);
    }
}
