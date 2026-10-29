using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.UpdateArticuloAnexo
{
    public class UpdateArticuloAnexoCommandValidator : ArticuloAnexoValidator<UpdateArticuloAnexoCommand>
    {
        public UpdateArticuloAnexoCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
