using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.CreateCategoriaProducto
{
    public class CreateCategoriaProductoCommandValidator : AbstractValidator<CreateCategoriaProductoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCategoriaProductoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

            RuleFor(x => x.FirthCode)
                .MaximumLength(2).WithMessage("El código Firth no puede exceder los 2 caracteres.");
        }
    }
}
