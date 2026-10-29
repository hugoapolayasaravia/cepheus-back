using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.UpdateCategoriaProducto
{
    public class UpdateCategoriaProductoCommandValidator : AbstractValidator<UpdateCategoriaProductoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCategoriaProductoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

            RuleFor(x => x.FirthCode)
                .MaximumLength(2).WithMessage("El código Firth no puede exceder los 2 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
