using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.UpdateCotizacion
{
    public class UpdateCotizacionCommandValidator : AbstractValidator<UpdateCotizacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCotizacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");

            RuleFor(x => x.VendedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Facturacion.Maestros.Vendedores.Query().AnyAsync(v => v.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El vendedor indicado no existe.");

            RuleFor(x => x.FormaPagoVentaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Facturacion.Catalogos.FormasPagoVenta.Query().AnyAsync(f => f.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La forma de pago indicada no existe.");

            RuleFor(x => x.TecnicoCode)
                .MustAsync(async (c, ct) => await _uow.Facturacion.Maestros.Tecnicos.Query().AnyAsync(t => t.TrabajadorCode == c!.Trim().ToUpper(), ct))
                .WithMessage("El técnico indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.TecnicoCode));

            RuleFor(x => x.FleteCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Facturacion.Catalogos.Fletes.Query().AnyAsync(f => f.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El flete indicado no existe.");

            RuleFor(x => x.ClienteCode)
                .MustAsync(async (c, ct) => await _uow.Facturacion.Maestros.Clientes.Query().AnyAsync(cl => cl.Code == c!.Trim().ToUpper(), ct))
                .WithMessage("El cliente indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.ClienteCode));

            RuleFor(x => x)
                .MustAsync(async (x, ct) => await _uow.Facturacion.Maestros.Obras.Query()
                    .AnyAsync(o => o.ClienteCode == x.ClienteCode!.Trim().ToUpper() && o.Code == x.ObraCode!.Trim().ToUpper(), ct))
                .WithMessage("La obra indicada no existe para el cliente indicado.")
                .When(x => !string.IsNullOrWhiteSpace(x.ObraCode) && !string.IsNullOrWhiteSpace(x.ClienteCode));

            RuleFor(x => x.ObraCode)
                .Empty().WithMessage("Para indicar una obra debe indicar primero el cliente.")
                .When(x => string.IsNullOrWhiteSpace(x.ClienteCode));

            RuleFor(x => x.ClientAddressUbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == c.Trim(), ct))
                .WithMessage("El ubigeo de la dirección del cliente no existe.");

            RuleFor(x => x.WorkAddressUbigeoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (c, ct) => await _uow.Comunes.Ubigeos.Query().AnyAsync(u => u.Code == c.Trim(), ct))
                .WithMessage("El ubigeo de la dirección de la obra no existe.");

            RuleFor(x => x.ProjectStatus)
                .Must(s => System.Enum.TryParse<EstadoProyectoCotizacion>(s, true, out _))
                .WithMessage("El estado del proyecto no es válido.");

            RuleFor(x => x.MetradoCalculationSystem)
                .Must(v => System.Enum.TryParse<SistemaCalculoMetrado>(v, true, out _))
                .WithMessage($"El sistema de cálculo de metrado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(SistemaCalculoMetrado)))}.")
                .When(x => !string.IsNullOrWhiteSpace(x.MetradoCalculationSystem));

            RuleFor(x => x.MetradoCalculationSystem)
                .NotEmpty().WithMessage("Para el negocio PT debe indicar el sistema de cálculo de metrado (Plantilla a Utilizar).")
                .When(x => x.NegocioCode.Trim().ToUpper() == "PT");

            RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(1);
            RuleFor(x => x.ClientName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.WorkName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.WorkAddress).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ContactName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ContactPhone).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(50);
            RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.GlobalVolume).GreaterThanOrEqualTo(0);
            RuleFor(x => x.IgvRate).GreaterThanOrEqualTo(0);
            RuleFor(x => x.WorkDurationMonths).GreaterThanOrEqualTo(0);

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || !x.StartDate.HasValue || x.EndDate >= x.StartDate)
                .WithMessage("La fecha de término de obra no puede ser anterior a la fecha de inicio.");
        }
    }
}
