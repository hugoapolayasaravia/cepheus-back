using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.UpdateAnalisisVenta
{
    public class UpdateAnalisisVentaCommandValidator : AbstractValidator<UpdateAnalisisVentaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAnalisisVentaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del análisis de ventas es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del análisis de ventas es obligatorio.")
                .MaximumLength(40).WithMessage("El nombre no puede exceder los 40 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro análisis de ventas con ese nombre.");

            RuleFor(x => x.ShortName)
                .MaximumLength(8).WithMessage("El nombre corto no puede exceder los 8 caracteres.");

            RuleFor(x => x.SegmentoVentasCode)
                .Cascade(CascadeMode.Stop)
                .Length(2).WithMessage("El código de segmento de ventas debe tener 2 caracteres.")
                .MustAsync(SegmentoVentasExists).WithMessage("El segmento de ventas indicado no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.SegmentoVentasCode));

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateAnalisisVentaCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.AnalisisVentas.Query()
                .AnyAsync(a => a.Code != command.Code && a.Name.ToLower() == name.Trim().ToLower(), cancellationToken);

        private async Task<bool> SegmentoVentasExists(string? code, CancellationToken cancellationToken)
            => await _uow.Facturacion.Catalogos.SegmentosVentas.Query()
                .AnyAsync(s => s.Code == code!.Trim().ToUpper(), cancellationToken);
    }
}
