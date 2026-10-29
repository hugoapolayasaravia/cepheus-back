using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.CreateEspecialidad
{
    public class CreateEspecialidadTrabajadorCommandValidator : AbstractValidator<CreateEspecialidadTrabajadorCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateEspecialidadTrabajadorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la especialidad es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una nacionalidad con ese nombre.");
        }


        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.EspecialidadesTrabajador.Query()
                .AnyAsync(n => n.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}