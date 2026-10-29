using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.CreateNotaCotizacionPlantilla
{
    public class CreateNotaCotizacionPlantillaCommandValidator : AbstractValidator<CreateNotaCotizacionPlantillaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNotaCotizacionPlantillaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El negocio es obligatorio.")
                .Length(2).WithMessage("El código del negocio debe tener 2 caracteres.")
                .MustAsync(NegocioExiste).WithMessage("El negocio indicado no existe.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción de la nota es obligatoria.");

            RuleFor(x => x.Option)
                .IsInEnum().WithMessage("La opción de la nota no es válida.");
        }

        private async Task<bool> NegocioExiste(string negocioCode, CancellationToken cancellationToken)
            => await _uow.Comunes.Negocios.Query()
                .AnyAsync(n => n.Code == negocioCode, cancellationToken);
    }
}
